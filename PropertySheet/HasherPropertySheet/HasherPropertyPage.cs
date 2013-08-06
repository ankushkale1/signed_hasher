using System;
using System.Linq;
using System.Windows.Forms;
using SharpShell.SharpPropertySheet;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using System.Threading;
using System.Net;

using Alphaleonis.Win32.Filesystem; //long path support upto 32K
using System.Security.Permissions;

using Mono.Security.Authenticode;
using Mono.Security.X509;
using Mono.Security.X509.Extensions;
using System.Collections;

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace signed_hasher
{
    [ComVisible(true)]
    public partial class HasherPropertyPage : SharpPropertyPage
    {
        private static System.Security.Cryptography.X509Certificates.X509Certificate2 selectedcert;
        private string foldertohash;
        int matched = 0;
        int notmatched = 0;
        private Label lmatched, lnotmatched;
        private static DataGridView grid;

        //static System.Diagnostics.EventLog appLog;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileTimesPropertyPage"/> class.
        /// </summary>
        public HasherPropertyPage()
        {
            InitializeComponent();
            /*appLog = new System.Diagnostics.EventLog();

            try
            {
                EventLog.CreateEventSource("FileHasherPropertyPage", "Application");
                //System.Windows.Forms.MessageBox.Show("not exists");
                appLog = new System.Diagnostics.EventLog();
                appLog.Source = "FileHasherPropertyPage";

            }
            catch (Exception e)
            {
                appLog = new System.Diagnostics.EventLog();
                appLog.Source = "FileHasherPropertyPage";
                System.Windows.Forms.MessageBox.Show(e.Message + " " + e.StackTrace);
            }    
            */
            grid = datagrid;
            lmatched = lblmatched;
            lnotmatched = lblnotmatched;
            ProgressBar.CheckForIllegalCrossThreadCalls = false;

            //  Set the page title.
            PageTitle = "Hasher";

            //  Note: You can also set the icon to be used:
            //  PageIcon = Properties.Resources.SomeIcon;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            Thread t_computehash = new Thread(generate);
            t_computehash.Name = "Hasher thread..";
            lblmatched.Text = "Matched: ";
            lblnotmatched.Text = "Not Matched: ";
            grid.Rows.Clear();

            if (selectedcert!=null)
            {
                selectedcert = selectcert(); //select cert from store
                t_computehash.Start();
            }
        }

        private void generate()
        {
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                int count = 0;

                string[] filesz = Directory.GetFiles(@foldertohash, "*.*", System.IO.SearchOption.AllDirectories);
                int filecount = filesz.Length;
                filesz = null;
                hashprogress.Step = 1;
                hashprogress.Maximum = filecount;
                hashprogress.Value = 0;

                foreach (string file in Directory.GetFiles(@foldertohash, "*.*", System.IO.SearchOption.AllDirectories))
                {

                    Uri ufile = new Uri(file);
                    Uri folder = new Uri(@foldertohash);
                    string relativePath = Uri.UnescapeDataString(folder.MakeRelativeUri(ufile).ToString().Replace('/', System.IO.Path.DirectorySeparatorChar));

                    if (hashsignfile(file))
                    {
                        grid.Rows.Add(relativePath, "Hashed");
                        count++;
                        lblmatched.Text = "Hashed: " + count;
                        hashprogress.PerformStep();
                    }
                }
            }
            catch (Exception e)
            {
                //appLog.WriteEntry(e.Message + e.StackTrace, System.Diagnostics.EventLogEntryType.Warning);
                //MessageBox.Show(e.Message);
            }
        }

        #region crypto

        private static bool hashsignfile(string file)
        {
            try
            {
                if (selectedcert == null)
                    selectedcert = selectcert();

                string hash = hashfile(file);

                FileInfo info = new FileInfo(file);
                Directory.SetCurrentDirectory(info.Directory.FullName);

                System.IO.MemoryStream mxml = new System.IO.MemoryStream();

                XmlWriter xwriter = XmlWriter.Create(mxml);
                xwriter.WriteStartDocument();
                xwriter.WriteStartElement("DigiSig");
                xwriter.WriteElementString("hash", hash);
                xwriter.WriteElementString("size", info.Length.ToString());
                xwriter.WriteElementString("signtime", DateTime.Now.ToString());
                xwriter.WriteElementString("creator", System.Security.Principal.WindowsIdentity.GetCurrent().Name);

                xwriter.WriteEndElement();
                xwriter.WriteEndDocument();
                xwriter.Flush();
            
                mxml.Position = 0;
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(mxml);

                /*CspParameters parms = new CspParameters(1);
                parms.KeyContainerName = selectedcert;
                RSACryptoServiceProvider csp = new RSACryptoServiceProvider(parms);*/
                
                RSACryptoServiceProvider csp = (RSACryptoServiceProvider)selectedcert.PrivateKey;

                Reference r = new Reference("");
                r.AddTransform(new XmlDsigEnvelopedSignatureTransform(false));

                SignedXml sxml = new SignedXml(xmldoc);
                sxml.SigningKey = csp;
                sxml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigCanonicalizationUrl;
                sxml.AddReference(r);
                sxml.ComputeSignature();

                XmlElement sig = sxml.GetXml();
                xmldoc.DocumentElement.AppendChild(sig);

                XmlTextWriter writer = new XmlTextWriter(file + ".sha", UTF8Encoding.UTF8);
                writer.Formatting = Formatting.Indented;
                xmldoc.WriteTo(writer);
                writer.Flush();
                writer.Close();
                //appLog.WriteEntry("Hashed File: "+file, System.Diagnostics.EventLogEntryType.Information);
                return true;
            }
            catch (Exception e)
            {
                //appLog.WriteEntry(e.Message + e.StackTrace, System.Diagnostics.EventLogEntryType.Warning);
                //Console.WriteLine(e.Message + e.TargetSite);
                return false;
            }
        }

        private static string readhashes(string path)
        {
            System.IO.FileStream xfile = null;

            try
            {
                // first check is xml is tampered?
                if (VerifyDocument(path)) // if document valid then continue
                {
                    xfile = File.Open(path, FileMode.Open);
                    XDocument doc = XDocument.Load(xfile);

                    foreach (XElement element in doc.Elements())
                        if (element.Name == "DigiSig")
                        {
                            foreach (XElement ele in element.Elements())
                            {
                                if (ele.Name == "hash")
                                {
                                    xfile.Close();
                                    return ele.Value;
                                }
                            }
                        }
                }
                else
                {
                    //appLog.WriteEntry("Doc verification fail: "+path, System.Diagnostics.EventLogEntryType.Warning);
                    //Console.WriteLine("Document Verification Failed");
                    xfile.Close();
                    return "error";
                }
                return "notfound";
            }
            catch (Exception e)
            {
                //appLog.WriteEntry(e.Message + e.StackTrace, System.Diagnostics.EventLogEntryType.Warning);
                //MessageBox.Show(e.Message);
                xfile.Close();
                return "notfound";
            }
        }

        public static bool VerifyDocument(string path)
        {
            try
            {
                /*CspParameters parms = new CspParameters(1);
                parms.KeyContainerName = selectedcert;
                RSACryptoServiceProvider csp = new RSACryptoServiceProvider(parms);*/
                if (selectedcert == null)
                    selectedcert = selectcert();
                RSACryptoServiceProvider csp = (RSACryptoServiceProvider)selectedcert.PublicKey.Key;

                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(path);

                SignedXml sxml = new SignedXml(xmldoc);

                XmlNode dsig = xmldoc.GetElementsByTagName("Signature", SignedXml.XmlDsigNamespaceUrl)[0];
                sxml.LoadXml((XmlElement)dsig);

                return sxml.CheckSignature(csp);
            }
            catch (Exception e)
            {
                //appLog.WriteEntry(e.Message + e.StackTrace, System.Diagnostics.EventLogEntryType.Warning);
                //Console.WriteLine(e.Message + e.TargetSite);
                return false;
            }
        }

        private static System.Security.Cryptography.X509Certificates.X509Certificate2 selectcert() //select cert from store & set selectedcert
        {
            try
            {
                System.Security.Cryptography.X509Certificates.X509Store mystore = new System.Security.Cryptography.X509Certificates.X509Store(StoreLocation.CurrentUser);
                mystore.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                X509Certificate2Collection certCollection = (X509Certificate2Collection)mystore.Certificates;
                X509Certificate2Collection foundCollection = (X509Certificate2Collection)certCollection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
                X509Certificate2Collection selectedcollection = X509Certificate2UI.SelectFromCollection(foundCollection, "Select a Certificate.", "Select a Certificate from the following list to get information on that certificate", X509SelectionFlag.SingleSelection);
                if (selectedcollection.Count > 0)
                {
                    X509Certificate2 certz = selectedcollection[0];
                    return certz;
                }
                return null;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }

        private static string hashfile(string file)
        {
            byte[] hash;
            string hashdata = null;
            SHA256CryptoServiceProvider hasher = new SHA256CryptoServiceProvider();
            System.IO.FileStream filedata=null;

            try
            {
                //FileStream sfile = new FileStream(file, System.IO.FileMode.Open);
                filedata = File.Open(file, FileMode.Open); // to tackle with out of memory exception hashing 1gb+ file
                hash = hasher.ComputeHash(filedata);
                //sfile.Close();
                hashdata = System.BitConverter.ToString(hash);
                hashdata = hashdata.Replace("-", "");
            }
            catch (Exception e)
            {
                MessageBox.Show("In funct Hashfile: "+e.Message);
            }
            finally
            {
                if (filedata != null)
                    filedata.Close();
            }
            return hashdata;
        }

        #endregion

        private void check(string path)
        {
            try
            {
                int totalfiles = Directory.GetFiles(path, "*.*", System.IO.SearchOption.AllDirectories).Count();
                string[] files = Directory.GetFiles(path, "*.*", System.IO.SearchOption.AllDirectories);

                lmatched.Text = "Matched: 0";
                lnotmatched.Text = "Not Matched: 0";
                int no;

                hashprogress.Maximum = totalfiles;
                hashprogress.Step = 1;
                hashprogress.Value = 0;

                matched = 0;
                notmatched = 0;

                //string parent = Directory.GetParent(path).FullName;

                for (int i = 0; i < totalfiles; i++)
                {
                    if (!(new FileInfo(files[i]).Extension == ".sha"))
                    {
                        if (File.Exists(files[i] + ".sha"))
                        {
                            string hash = readhashes(files[i] + ".sha");
                            if (hashfile(files[i]) == hash)
                            {
                                no = grid.Rows.Add(files[i], "ok");
                                grid.Rows[no].DefaultCellStyle.ForeColor = Color.Green;
                                matched++;
                                lmatched.Text = "Matched: " + matched;
                            }
                            else
                            {
                                no = grid.Rows.Add(files[i], "error");
                                grid.Rows[no].DefaultCellStyle.ForeColor = Color.Red;
                                notmatched++;
                                lnotmatched.Text = "Not Matched: " + notmatched;
                            }
                            hashprogress.PerformStep();
                        }
                        else
                        {
                            no = grid.Rows.Add(files[i], "Hash File not found");
                            grid.Rows[no].DefaultCellStyle.ForeColor = Color.Gray;
                        }
                    }
                }

                hashprogress.Value = hashprogress.Maximum;
            }
            catch (Exception e)
            {
                MessageBox.Show("in funct check "+e.Message);
            }
        }

        private void btncheck_Click(object sender, EventArgs e)
        {
            if (selectedcert!=null)
                selectedcert = selectcert();

            datagrid.Rows.Clear();
            check(foldertohash);
        }

        /// <summary>
        /// Called when the page is initialised.
        /// </summary>
        /// <param name="parent">The parent property sheet.</param>
        protected override void OnPropertyPageInitialised(SharpPropertySheet parent)
        {
            //  Store the file path.
            foldertohash = @parent.SelectedItemPaths.First();
            //MessageBox.Show(foldertohash);
            //  Load the file times into the dialog.
            //LoadFileTimes();
        }

        /// <summary>
        /// Called when apply is pressed on the property sheet, or the property
        /// sheet is dismissed with the OK button.
        /// </summary>
        protected override void OnPropertySheetApply()
        {
            //  Save the changes.
            //SaveFileTimes();
        }

        /// <summary>
        /// Called when OK is pressed on the property sheet.
        /// </summary>
        protected override void OnPropertySheetOK()
        {
            //  Save the changes.
            //SaveFileTimes();
        }

        private void FileTimesPropertyPage_Load(object sender, EventArgs e)
        {

        }
    }
}
