using System;
using System.Diagnostics;
using DotNetOpenAuth.OAuth2;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Util;
using Google.Apis.Services;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

//download file
using System.Net;
using Google.Apis.Authentication;
//

using System.Data.SQLite;
using System.Data;

using System.Security.Cryptography;

namespace signed_hasher
{
    public struct mimetype
    {
        public const string folder = "application/vnd.google-apps.folder";
        public const string file = "application/vnd.google-apps.file";
        public const string unknown = "application/vnd.google-apps.unknown";
        public const string plaintext = "text/plain";
    }

    class gdrive
    {
        static DriveService service;
        public static string accesstoken = null; // one time accesstoken

        public static bool init(string CLIENT_ID, string CLIENT_SECRET)
        {
            try
            {
                // Register the authenticator and create the service
                var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description, CLIENT_ID, CLIENT_SECRET);
                var auth = new OAuth2Authenticator<NativeApplicationClient>(provider, GetAuthorization);
                var serviceobj = new DriveService(new BaseClientService.Initializer()
                {
                    Authenticator = auth
                });

                service = serviceobj;

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + Environment.NewLine + e.StackTrace, "Exception");
                return false;
            }
        }

        private static IAuthorizationState GetAuthorization(NativeApplicationClient arg)
        {
            string authocode = null;

            // Get the auth URL:
            IAuthorizationState state = new AuthorizationState(new[] { DriveService.Scopes.Drive.GetStringValue() });
            state.Callback = new Uri(NativeApplicationClient.OutOfBandCallbackUrl);
            Uri authUri = arg.RequestUserAuthorization(state);

            // Request authorization from the user (by opening a browser window):
            Process.Start(authUri.ToString());
            InputBox.Show("Authorization Code", "Enter Authorization Code", ref authocode);
            string authCode = authocode;
            accesstoken = authocode;
            // Retrieve the access token by using the authorization code:
            return arg.ProcessUserAuthorization(authCode, state);
        }

        public static File insertFile(String title, String description, String mimeType, String filepath,String foldername)
        {
            // File's metadata.
            File body = new File();
            body.Title = title;
            body.Description = description;
            body.MimeType = mimeType;

            // Set the parent folder.
            if (!String.IsNullOrEmpty(foldername))
            {
                File parentfolder = getfolderid(foldername);
                body.Parents = new List<ParentReference>() { new ParentReference() { Id = parentfolder.Id } };
            }

            // File's content.
            byte[] byteArray = System.IO.File.ReadAllBytes(filepath);
            System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);

            try
            {
                FilesResource.InsertMediaUpload request = service.Files.Insert(body, stream, mimeType);
                request.Upload();

                File file = request.ResponseBody;

                // Uncomment the following line to print the File ID.
                 Console.WriteLine("File ID: " + file.Id);

                return file;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("Login Required"))
                    MessageBox.Show("Please Enter Authorization Code from Web Browser");

                Console.WriteLine("An error occurred: " + e.Message);
                return null;
            }
        }

        public static List<File> listfiles(string foldername)
        {
            File myfolder = getfolderid(foldername);
            List<File> filesinfodler = new List<File>();

            if (myfolder != null)
            {
                ChildrenResource.ListRequest request = service.Children.List(myfolder.Id);

                do
                {
                    try
                    {
                        ChildList children = request.Fetch();

                        foreach (ChildReference child in children.Items)
                        {
                            //Console.WriteLine("File Id: " + child.Kind+child.SelfLink);
                            File file = service.Files.Get(child.Id).Fetch();
                            Console.WriteLine("File : "+file.OriginalFilename);
                            filesinfodler.Add(file);
                        }
                        request.PageToken = children.NextPageToken;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("An error occurred: " + e.Message);
                        request.PageToken = null;
                    }
                } while (!String.IsNullOrEmpty(request.PageToken));

                return filesinfodler;
            }
            else
            {
                MessageBox.Show("Folder not found");
                return null;
            }
        }

        public static File getfolderid(string foldername)
        {
            ChildrenResource.ListRequest request = service.Children.List("root");
            do
            {
                try
                {
                    ChildList children = request.Fetch();

                    foreach (ChildReference child in children.Items)
                    {
                        //Console.WriteLine("File Id: " + child.Kind+child.SelfLink);
                        var file=service.Files.Get(child.Id).Fetch();

                        if (file.MimeType == mimetype.folder && file.Title == foldername && file.ExplicitlyTrashed!=true)
                            return file;
                    }
                    request.PageToken = children.NextPageToken;
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                    request.PageToken = null;
                    return null;
                }
            } while (!String.IsNullOrEmpty(request.PageToken));

            return null;
        }

        private static File updateFile(String fileId, String newTitle,String newDescription, String newMimeType, String filepath, bool newRevision)
        {
            try
            {
                // First retrieve the file from the API.
                File file = service.Files.Get(fileId).Fetch();

                // File's new metadata.
                file.Title = newTitle;
                file.Description = newDescription;
                file.MimeType = newMimeType;

                // File's new content.
                byte[] byteArray = System.IO.File.ReadAllBytes(filepath);
                System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);

                // Send the request to the API.
                FilesResource.UpdateMediaUpload request = service.Files.Update(file, fileId, stream, newMimeType);
                request.NewRevision = newRevision;
                request.Upload();

                File updatedFile = request.ResponseBody;
                return updatedFile;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
                return null;
            }
        }

        public static void syncfiles(string localfolder, string gfolder)
        {
            List<File> files=listfiles(gfolder);

            foreach(string file in System.IO.Directory.GetFiles(localfolder,"*.sql"))
            {
                bool matched = false;
                if (files != null)
                {
                    foreach (File myfile in files)
                    {
                        if (new System.IO.FileInfo(file).Name == myfile.OriginalFilename) // same file in local & gdrive folder
                        {
                            matched = true;
                            byte[] hash = null;
                            string hashstring = null;
                            MD5CryptoServiceProvider hasher = new MD5CryptoServiceProvider();
                            hash = hasher.ComputeHash(System.IO.File.ReadAllBytes(file));
                            hashstring = System.BitConverter.ToString(hash);
                            hashstring = hashstring.Replace("-", "");

                            if (hashstring == myfile.Md5Checksum.ToUpper())
                            {
                                Console.WriteLine("File: " + file + " is uptodate on gDrive");
                            }
                            else
                            {
                                DialogResult dres = MessageBox.Show("File: " + file + " changed.Do you want me to Download & overwrite file from Gdrive (Button YES) or Upload new File to Gdrive(Button NO) or Do Nothing (Button Cancel)", "File MD5 Mismatch", MessageBoxButtons.YesNoCancel);

                                if (dres == DialogResult.Yes)
                                {
                                    System.IO.File.Delete(file);
                                    System.IO.Stream filestream = DownloadFile(myfile);

                                    using (var localfile = System.IO.File.Create(file))
                                    {
                                        filestream.CopyTo(localfile);
                                        localfile.Flush();
                                        localfile.Close();
                                    }
                                }
                                else if (dres == DialogResult.No)
                                {
                                    updateFile(myfile.Id, myfile.Title, myfile.Description, myfile.MimeType, file, true); // true to create same file with different versions both old & new preserved
                                }
                                else
                                {
                                    MessageBox.Show("No Change done on File");
                                }
                            }
                            break;
                        }
                    }
                }
                
                if(matched==false) //local files dont exists on Gdrive upload them
                {
                    gdrive.insertFile(new System.IO.FileInfo(file).Name, "database dump", mimetype.plaintext, file, gfolder);
                }
            }

            // now if file only present on google drive not on localfolder then download & update databse
            foreach (File myfile in files)
            {
                if (!System.IO.File.Exists("dbdump//"+myfile.OriginalFilename))
                {
                    Console.WriteLine("File: " + myfile.OriginalFilename + " dont exists so fetching....");
                    
                    System.IO.Stream filestream = DownloadFile(myfile);

                    using (var localfile = System.IO.File.Create("dbdump//"+myfile.OriginalFilename))
                    {
                        filestream.CopyTo(localfile);
                        localfile.Flush();
                        localfile.Close();
                    }
                }
            }
        }

        public static void getfilehasherdb()
        {
            List<File> files = retrieveAllFiles();

            foreach (File file in files)
            {
                if (string.IsNullOrEmpty(file.OriginalFilename) && file.FileExtension=="sql")
                {

                }
            }
        }

        public static List<File> retrieveAllFiles()
        {
            List<File> result = new List<File>();
            FilesResource.ListRequest request = service.Files.List();
            do
            {
                try
                {
                    FileList files = request.Fetch();
                    result.AddRange(files.Items);
                    request.PageToken = files.NextPageToken;
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                    request.PageToken = null;
                }
            } while (!String.IsNullOrEmpty(request.PageToken));

            foreach (File file in result)
                if(!string.IsNullOrEmpty(file.OriginalFilename))
                    Console.WriteLine(file.OriginalFilename);

            return result;
        }

        public static System.IO.Stream DownloadFile(File file)
        {
            if (!String.IsNullOrEmpty(file.DownloadUrl))
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(file.DownloadUrl));
                    service.Authenticator.ApplyAuthenticationToRequest(request);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return response.GetResponseStream();
                    }
                    else
                    {
                        Console.WriteLine(
                            "An error occurred: " + response.StatusDescription);
                        return null;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                    return null;
                }
            }
            else
            {
                // The file doesn't have any content stored on Drive.
                return null;
            }
        }

        public static File createfolder(string foldertitle,string desc)
        {
            Google.Apis.Drive.v2.Data.File folder = new Google.Apis.Drive.v2.Data.File();
            folder.Title = foldertitle;
            folder.Description = desc;
            folder.MimeType = mimetype.folder;
            folder.Editable = true;
            File file = service.Files.Insert(folder).Fetch();
            return file;
        }
    }

    public class CustomAuthenticator : IAuthenticator
    {
        public String AccessToken { get; set; }

        public void ApplyAuthenticationToRequest(HttpWebRequest request)
        {
            request.Headers[HttpRequestHeader.Authorization] = "Bearer " + AccessToken;
        }
    }

    public class dbmanager
    {
        public static SQLiteConnection con;
        static SQLiteCommand com;
        static SQLiteDataReader dbread;

        static string dbpath = "hasher.db";
        static int result;

        static int count = 0;

        public static bool init()
        {
            con = new SQLiteConnection(String.Format("Data Source={0};", dbpath));
            //con = new SQLiteConnection(String.Format("Data Source={0};Password=ankush", dbpath));  // 2. Then use
            con.Open();
            //con.ChangePassword("ankush"); //1. first encrypt
            Console.WriteLine("Connected to database");

            // PRAGMA synchronous = OFF // turn of waiting of flushing data to io,OS will do that.. but can be fatal if powerfailure or OS crash
            //SQLiteCommand sqlcommand;

            /*sqlcommand = new SqliteCommand("PRAGMA synchronous = OFF", dbmanager.con);
            sqlcommand.ExecuteNonQuery();*/

            // PRAGMA count_changes=OFF // i dont want how many rows changed

            /*sqlcommand = new SQLiteCommand("PRAGMA count_changes = OFF", dbmanager.con);
            sqlcommand.ExecuteNonQuery();*/

            // pragma journal_mode = memory; // keep log in memory

            /*sqlcommand = new SqliteCommand("PRAGMA journal_mode = MEMORY", dbmanager.con);
            sqlcommand.ExecuteNonQuery();*/

            return true;
        }

        public static bool create_db_table()
        {
            init();

            int result;

            using (SQLiteTransaction sqltrans = con.BeginTransaction()) // login table
            {
                com = new SQLiteCommand(con);
                com.CommandText = "create table [filehash] ([uid] INTEGER PRIMARY KEY AUTOINCREMENT,[filename] TEXT NOT NULL,[hash] TEXT NOT NULL,[length] TEXT NOT NULL,[date] TEXT NOT NULL,[createdby] TEXT NOT NULL)";
                result = com.ExecuteNonQuery();
                sqltrans.Commit();

                if (result == 1)
                    Console.WriteLine("Added table FileHash");
                else
                    Console.WriteLine("error in adding login table");
            }

            //con.Close();
            if (result == 1)
            {
                Console.WriteLine("Database tables created");
                return true;
            }
            else
            {
                Console.WriteLine("Database tables creation failed!!!!");
                return false;
            }
        }

        public static bool execute_query(string query)
        {
            //globallog.info(infotype.ALL_DEBUG_INFORMATION, "Query: " + query);

            using (SQLiteTransaction sqltrans = con.BeginTransaction())
            {
                com = new SQLiteCommand(con);
                com.CommandText = query;
                result = com.ExecuteNonQuery();
                sqltrans.Commit();

                if (result == 1)
                {
                    Console.WriteLine( "Query: " + query + " Result: " + result);
                    return true;
                }
                else
                {
                    Console.WriteLine("Query: " + query + " Result: " + result);
                    return false;
                }
            }
        }

        public static bool alreadyexists(string filename)
        {
            count++;
            Console.WriteLine("Alreadyexists " + count);

            SQLiteDataReader hash = null;
            hash = execute_readquery("select hash from filehash where filename=\"" + filename + "\"");

            if (hash != null) //means there is result
            {
                while (hash.Read())
                {
                    if (string.IsNullOrEmpty(hash.GetString(0)))
                        return false;
                    else
                        return true;
                }
            }

            return false;
        }

        public static SQLiteDataReader execute_readquery(string query)
        {
            com = new SQLiteCommand(con);
            com.CommandText = query;
            //com.ExecuteNonQuery();
            dbread = com.ExecuteReader();
            Console.WriteLine( "Query: " + query);
            return dbread;
        }

        public static bool closedb()
        {
            if (con != null)
            {
                con.Close();
                Console.WriteLine("database closed");
                return true;
            }

            Console.WriteLine("database cant be closed now");
            return false;
        }

        public static string getfiledata(string filename) // get script list according to user rights
        {
            SQLiteDataReader hash;

            try
            {
                hash = execute_readquery("select hash from filehash where filename=\"" + filename + "\"");

                if (hash != null) //means there is result
                {
                    while (hash.Read())
                    {
                        if (hash.GetString(0) != null)
                        {
                            return hash.GetString(0);
                            //Console.WriteLine("user found: "+users.GetString(0));
                        }
                    }
                }

                return "not found";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "not found";
            }
        }

    }

    #region inputbox

    public class InputBox
    {
        public static DialogResult Show(string title, string promptText, ref string value)
        {
            return Show(title, promptText, ref value, null);
        }

        public static DialogResult Show(string title, string promptText, ref string value,InputBoxValidation validation)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;
            if (validation != null)
            {
                form.FormClosing += delegate(object sender, FormClosingEventArgs e)
                {
                    if (form.DialogResult == DialogResult.OK)
                    {

                    }
                };
            }
            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }
    }
    public delegate string InputBoxValidation(string errorMessage);

    #endregion
}
