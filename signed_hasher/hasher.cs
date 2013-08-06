using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Xml;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using System.Threading;
using System.Net;
using CustomUIControls;
using Alphaleonis.Win32.Filesystem; //long path support upto 32K
using System.Security.Permissions;

/*using Mono.Security.Authenticode;
using Mono.Security.X509;               //<-- only for createpfx2 using mono to create pfx is broken
using Mono.Security.X509.Extensions;*/
using System.Collections;

using SharpShell;
using Apex.WinForms.Interop;
using Apex.WinForms.Shell;
using SharpShell.Attributes;
using SharpShell.Diagnostics;
using SharpShell.ServerRegistration;
using SharpShell.SharpPropertySheet;
using System.Reflection;
using System.ComponentModel.Composition.Hosting;
using CERTENROLLLib;
using System.Runtime.InteropServices;

//required for ssl
using System.Net.Security;
using System.Net.Sockets;
using System.Collections.Concurrent;
using System.Security.Authentication;
//
using System.Diagnostics;

using Alphaleonis.Win32.Security;
using System.Security.AccessControl;
using System.Security.Principal;

namespace signed_hasher
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")] //security by obsecurity. this may stop some wellknown attacks
    public partial class hasher : Form
    {
        private string foldertohash,watchfolder;
        private static string certpath;
        private static string pass;
        private static X509Certificate2 selectedcert;

        int matched = 0;
        int notmatched = 0;
        private Label lmatched, lnotmatched;
        public static DataGridView grid;
        public static TextBox filter;

        TaskbarNotifier notify;

        List<ServerEntry> shellextensions;

        public static bool fromsignserver = false;
        public static chatclient client;
        
        public hasher()
        {
            InitializeComponent();
            this.AllowDrop = true; //dragdrop support & In properties Behaviour : Allow Drop true ( for form or any controll )

            grid = datagrid;
            filter = txtfilter;
            lmatched = lblmatched;
            lnotmatched = lblnotmatched;

            ProgressBar.CheckForIllegalCrossThreadCalls = false;

            notify = new TaskbarNotifier();
            notify.SetBackgroundBitmap(new Bitmap(GetType(), "skin.bmp"), Color.FromArgb(255, 0, 255));
            notify.SetCloseBitmap(new Bitmap(GetType(), "close.bmp"), Color.FromArgb(255, 0, 255), new Point(442, 8));
            notify.TitleRectangle = new Rectangle(70, 9, 100, 10);
            notify.ContentRectangle = new Rectangle(70, 15, 397, 115);
            notify.ContentClick += new EventHandler(ContentClick);
            notify.ContentClickable = true;
            notify.EnableSelectionRectangle = true;
            notify.KeepVisibleOnMousOver = true;
            notify.ReShowOnMouseOver = true;

            shellextensions = new List<ServerEntry>();

        }

        private void btnbrowse_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            foldertohash = folderBrowserDialog1.SelectedPath;
            txtbrowse.Text = foldertohash;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            txtbrowse.BackColor = Color.White;

            Thread t_computehash = new Thread(generate);
            t_computehash.Name = "Hasher thread..";
            lblmatched.Text = "Matched: ";
            lblnotmatched.Text = "Not Matched: ";
            grid.Rows.Clear();
            if (!String.IsNullOrEmpty(txtbrowse.Text))
            {
                if (certpath == null) //cert file not selected
                {
                    selectedcert = selectcert(); //select cert from store
                    t_computehash.Start();
                }
                else if (checkcert(certpath, pass)) // check before starting lengthy checksum process
                {
                    t_computehash.Start();
                }
            }
            else
            {
                MessageBox.Show("Information incorrect. Make sure you selected folder,private key file & entered password");
                btnbrowse.Focus();
                txtbrowse.BackColor = Color.LightBlue;
            }
        }

        private void generate()
        {
            chatclient.storecount = 0;
            chatclient.sentcount = 0;

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            int count = 0;

            string[] filesz=null;

            if (!string.IsNullOrEmpty(txtfilter.Text))
                filesz=txtfilter.Text.Split(',').SelectMany(filter => System.IO.Directory.GetFiles(foldertohash, filter, System.IO.SearchOption.AllDirectories)).ToArray();
            else
                filesz = Directory.GetFiles(foldertohash, "*.*", System.IO.SearchOption.AllDirectories);

            int filecount = filesz.Length;
            
            hashprogress.Step = 1;
            hashprogress.Maximum = filecount;
            hashprogress.Value = 0;

            foreach (string file in filesz)
            {

                Uri ufile = new Uri(file);
                Uri folder = new Uri(foldertohash);
                string relativePath = Uri.UnescapeDataString(folder.MakeRelativeUri(ufile).ToString().Replace('/',System.IO.Path.DirectorySeparatorChar));

                if (hashsignfile(file))
                {
                    grid.Rows.Add(relativePath, "Hashed");
                    count++;
                    lblmatched.Text = "Hashed: " + count;
                    hashprogress.PerformStep();
                }
            }
        }

        #region crypto

        private static bool hashsignfile(string file)
        {
            string hash = hashfile(file);
            FileInfo info = new FileInfo(file);
            Directory.SetCurrentDirectory(info.Directory.FullName);

            if (info.Extension != ".sha")
            {
                System.IO.MemoryStream mxml = new System.IO.MemoryStream();

                XmlWriter xwriter = XmlWriter.Create(mxml);
                xwriter.WriteStartDocument();
                xwriter.WriteStartElement("DigiSig");
                xwriter.WriteElementString("hash", hash);
                xwriter.WriteElementString("size", info.Length.ToString());
                xwriter.WriteElementString("signtime", DateTime.Now.ToString());
                xwriter.WriteElementString("creator", System.Security.Principal.WindowsIdentity.GetCurrent().Name);
                xwriter.WriteElementString("Thumbprint",selectedcert.Thumbprint);

                xwriter.WriteEndElement();
                xwriter.WriteEndDocument();
                xwriter.Flush();

                try
                {
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

                    if (fromsignserver)
                    {
                        client.sendmsg(info.Name + ";" + FileVersionInfo.GetVersionInfo(file).FileVersion + ";" + hash + ";" + info.Length.ToString() + ";" + DateTime.Now.ToString() + ";" + System.Security.Principal.WindowsIdentity.GetCurrent().Name, msgtype.storehash);
                        chatclient.sentcount++;
                    }

                    
                    bool rowalready = dbmanager.alreadyexists(file);

                    if (!rowalready)
                    {
                        dbmanager.execute_query("insert into filehash values(null" + ",\"" + file + "\",\"" + hash + "\",\"" + info.Length.ToString() + "\",\"" + DateTime.Now.ToString() + "\",\"" + System.Security.Principal.WindowsIdentity.GetCurrent().Name + "\")");
                        Console.WriteLine(file + " added");
                        //added = dbmanager.execute_query("insert into filehash values(null" + ",\"" + file + "\",null,\"" + hash + "\",\"" + length + "\",\"" + datatime + "\",\"" + user + "\")");
                    }
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + e.TargetSite);
                }
            }
            else // hash already present
            {
                if (fromsignserver)
                {
                    client.sendmsg(info.Name + ";" + FileVersionInfo.GetVersionInfo(file).FileVersion + ";" + hash + ";" + info.Length.ToString() + ";" + DateTime.Now.ToString() + ";" + System.Security.Principal.WindowsIdentity.GetCurrent().Name, msgtype.storehash);
                    chatclient.sentcount++;
                }
            }
            return false;
        }

        private static string readhashes(string path)
        {
            System.IO.FileStream xfile = null;
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
                    Console.WriteLine("Document Verification Failed");
                    xfile.Close();
                    return "error";
                }
                return "notfound";
        }

        public static bool VerifyDocument(string path)
        {
            try
            {
                /*CspParameters parms = new CspParameters(1);
                parms.KeyContainerName = selectedcert;
                RSACryptoServiceProvider csp = new RSACryptoServiceProvider(parms);*/
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
                Console.WriteLine(e.Message + e.TargetSite);
                return false;
            }
        }

        private bool checkcert(string cername, string password)
        {
            try
            {
                X509Certificate2 cert = new X509Certificate2(cername, password);
                selectedcert = cert;
                /*System.Security.Cryptography.X509Certificates.X509Store store = new System.Security.Cryptography.X509Certificates.X509Store(StoreName.My, StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadWrite); // well i dont wanna add it to store 
                store.Add(cert);
                store.Close();*/

                chkfromstore.CheckState = CheckState.Checked;

                return true;
            }
            catch (CryptographicException ce)
            {
                if (ce.Message.Contains("password is not correct"))
                    MessageBox.Show("Private key file password incorrect");
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show("Checkcert Function: "+e.Message);
                return false;
            }
        }

        private static X509Certificate2 selectcert() //select cert from store & set selectedcert
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

       /* private static void createpfx2(string password)
        {
            X509CertificateBuilder cb = new X509CertificateBuilder(3);
            cb.SerialNumber = Guid.NewGuid().ToByteArray();
            cb.IssuerName = "CN=Ankyasoft";
            cb.NotBefore = DateTime.Now;
            cb.NotAfter = new DateTime(2029, 12, 31);
            cb.SubjectName = "CN=FileSigner";
            cb.SubjectPublicKey = RSA.Create();

            RSA subjectKey = (RSA)RSA.Create();
            subjectKey.KeySize = 2048;
            string p12file = Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).FullName + "\\hasher.pfx"; //cause we using setcurrentdirectory so pfx may get created in unwanted folder

            BasicConstraintsExtension bce = new BasicConstraintsExtension();
            bce.CertificateAuthority = true;
            ExtendedKeyUsageExtension eku = new ExtendedKeyUsageExtension();
            SubjectAltNameExtension alt = null;

            // extensions
            if (bce != null)
                cb.Extensions.Add(bce);
            if (eku != null)
                cb.Extensions.Add(eku);
            if (alt != null)
                cb.Extensions.Add(alt);
            // signature
            cb.Hash = "SHA1";

            RSA issuerKey = (RSA)RSA.Create();

            PrivateKey key = new PrivateKey();
            key.RSA = issuerKey;

            byte[] rawcert = cb.Sign(issuerKey);

            PKCS12 p12 = new PKCS12();
            p12.Password = password;
            

            ArrayList list = new ArrayList();
            // we use a fixed array to avoid endianess issues 
            // (in case some tools requires the ID to be 1).
            list.Add(new byte[4] { 1, 0, 0, 0 });
            Hashtable attributes = new Hashtable(1);
            attributes.Add(PKCS9.localKeyId, list);

            p12.AddCertificate(new Mono.Security.X509.X509Certificate(rawcert), attributes);
            p12.AddPkcs8ShroudedKeyBag(subjectKey, attributes);
            p12.SaveToFile(p12file);
        } */

        public static void createpfx(string password)
        {
            // create DN for subject 
            var dnsubject = new CX500DistinguishedName();
            dnsubject.Encode("CN=FileHasher", X500NameFlags.XCN_CERT_NAME_STR_NONE);

            // create a new private key for the certificate
            CX509PrivateKey privateKey = new CX509PrivateKey();
            privateKey.ProviderName = "Microsoft Base Cryptographic Provider v1.0";
            //privateKey.ContainerName = "Hasher Private Key"; //causing exception object with same name exists
            privateKey.LegacyCsp = false;
            privateKey.MachineContext = false;
            privateKey.Length = 2048;
            privateKey.KeySpec = X509KeySpec.XCN_AT_SIGNATURE; // use is not limited
            privateKey.KeyUsage = X509PrivateKeyUsageFlags.XCN_NCRYPT_ALLOW_ALL_USAGES; 
            privateKey.ExportPolicy = X509PrivateKeyExportFlags.XCN_NCRYPT_ALLOW_PLAINTEXT_EXPORT_FLAG;
            privateKey.KeyProtection = X509PrivateKeyProtection.XCN_NCRYPT_UI_FORCE_HIGH_PROTECTION_FLAG;
            privateKey.Create();
            
            // Use the stronger SHA512 hashing algorithm
            var hashobj = new CObjectId();
            hashobj.InitializeFromAlgorithmName(ObjectIdGroupId.XCN_CRYPT_HASH_ALG_OID_GROUP_ID,
                ObjectIdPublicKeyFlags.XCN_CRYPT_OID_INFO_PUBKEY_ANY,
                AlgorithmFlags.AlgorithmFlagsNone, "SHA512");

            // add extended key usage if you want - look at MSDN for a list of possible OIDs
            /*var oid = new CObjectId();
            oid.InitializeFromValue("1.3.6.1.5.5.7.3.1"); // SSL server
            var oidlist = new CObjectIds();
            oidlist.Add(oid);
            var eku = new CX509ExtensionEnhancedKeyUsage();
            eku.InitializeEncode(oidlist);*/

            // Create the self signing request
            var cert = new CX509CertificateRequestCertificate();

            cert.InitializeFromPrivateKey(X509CertificateEnrollmentContext.ContextUser, privateKey, "");
            cert.Subject = dnsubject;
            cert.Issuer = dnsubject; // the issuer and the subject are the same
            cert.NotBefore = new DateTime(2013,1,1);
            // this cert expires immediately. Change to whatever makes sense for you
            cert.NotAfter = new DateTime(2029, 12, 31);
            //cert.X509Extensions.Add((CX509Extension)eku); // add the EKU
            cert.HashAlgorithm = hashobj; // Specify the hashing algorithm
            cert.Encode(); // encode the certificate

            // Do the final enrollment process
            var enroll = new CX509Enrollment();
            enroll.InitializeFromRequest(cert); // load the certificate
            enroll.CertificateFriendlyName = "File Hashing Certificate"; // Optional: add a friendly name
            enroll.CertificateDescription = "Signed Hasher Certificate";

            string csr = enroll.CreateRequest(); // Output the request in base64

            // and install it back as the response
            enroll.InstallResponse(InstallResponseRestrictionFlags.AllowUntrustedCertificate,csr, EncodingType.XCN_CRYPT_STRING_BASE64, password); // no password // Also installs cert to store
            // output a base64 encoded PKCS#12 so we can import it back to the .Net security classes
            var base64encoded = enroll.CreatePFX(password,PFXExportOptions.PFXExportChainWithRoot); // no password, this is for internal consumption

            //export pfx to file
            var fs = new System.IO.FileStream("hasher.pfx", System.IO.FileMode.Create);
            fs.Write(Convert.FromBase64String(base64encoded), 0, Convert.FromBase64String(base64encoded).Length);
            fs.Close();

            // instantiate the target class with the PKCS#12 data (and the empty password)
            var newcert=new System.Security.Cryptography.X509Certificates.X509Certificate2(
                System.Convert.FromBase64String(base64encoded), password,
                // mark the private key as exportable (this is usually what you want to do)
                System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.Exportable
            );

            //export public key to file
            System.IO.File.WriteAllBytes("hasher.cer", newcert.Export(X509ContentType.Cert));

            /*System.Security.Cryptography.X509Certificates.X509Store store = new System.Security.Cryptography.X509Certificates.X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            store.Remove(newcert); // remove from store i dont want it there..
            store.Close();*/
        }

        private static string hashfile(string file)
        {
            byte[] hash;
            string hashdata=null;
            SHA256CryptoServiceProvider hasher = new SHA256CryptoServiceProvider();
            System.IO.FileStream filedata=null;

            try
            {
                //FileStream sfile = new FileStream(file, System.IO.FileMode.Open);
                filedata = File.Open(file, FileMode.Open, FileAccess.Read); // to tackle with out of memory exception hashing 1gb+ file
                hash = hasher.ComputeHash(filedata);
                //sfile.Close();
                hashdata = System.BitConverter.ToString(hash);
                hashdata = hashdata.Replace("-", "");
            }
            catch (UnauthorizedAccessException)
            {
                if (MessageBox.Show("You are not allowed to access File: " + file + Environment.NewLine + "Do you want me to try to get access by modifying ACL", "Access Denied", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    getaccess(file);
                }
            }
            catch (NullReferenceException) //AlphaFS dont throw unauthorised access exception so if filestream is null we need to change ACL
            {
                if (MessageBox.Show("You are not allowed to access File: " + file + Environment.NewLine + "Do you want me to try to get access by modifying ACL", "Access Denied", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    getaccess(file);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("HashFunction: " + e.Message);
            }
            finally
            {
                if (filedata != null)
                        filedata.Close();
            }
            return hashdata;
        }

        public static void getaccess(string filepath)
        {
            try
            {
                FileInfo myfileinfo = new FileInfo(filepath);
                FileSecurity security = myfileinfo.GetAccessControl();
                WindowsIdentity self = System.Security.Principal.WindowsIdentity.GetCurrent();
                FileSystemAccessRule rule = new FileSystemAccessRule(self.Name,System.Security.AccessControl.FileSystemRights.FullControl,AccessControlType.Allow);
                security.AddAccessRule(rule);
                File.SetAccessControl(filepath, security,AccessControlSections.All);

                DirectoryInfo parentdir = myfileinfo.Directory;
                DirectorySecurity myDirectorySecurity = parentdir.GetAccessControl();
                myDirectorySecurity.AddAccessRule(new FileSystemAccessRule(self.Name, System.Security.AccessControl.FileSystemRights.FullControl, AccessControlType.Allow));
                parentdir.SetAccessControl(myDirectorySecurity);
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception while changing file permissions " + e.Message);
            }
        }

        #endregion

        private void check(string path)
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

                        if (fromsignserver) // verify from andro signserver
                            client.sendmsg(new FileInfo(files[i]).Name +";"+ FileVersionInfo.GetVersionInfo(files[i]).FileVersion,msgtype.returnhash);

                        if (hashfile(files[i]) == hash)
                        {
                            no = grid.Rows.Add(files[i], "ok",hash);
                            grid.Rows[no].DefaultCellStyle.ForeColor = Color.Green;
                            matched++;
                            lmatched.Text = "Matched: " + matched;
                            string dbhash = dbmanager.getfiledata(files[i]);
                            if(dbhash==hash)
                                hasher.grid.Rows[no].Cells[1].Value = "Verified";
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

        private void hasher_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            this.Hide();

            if (File.Exists("hasher.db"))
            {
                //syncdb();
                dbmanager.init();
            }
            else
            {
                dbmanager.create_db_table();
                dbmanager.closedb();
                //syncdb();
                dbmanager.init();
            }

            new ToolTip().SetToolTip(btnsync, "Sync database with Google Drive");
            new ToolTip().SetToolTip(btnbrowse, "Select Folder to Check or Hash");
            new ToolTip().SetToolTip(btnGenerate, "Create Checksums");
            new ToolTip().SetToolTip(btncheck, "Check Hashes");
            new ToolTip().SetToolTip(btnwatchfolder, "Select your Download folder");
            new ToolTip().SetToolTip(btnhelp, "View Help");
            new ToolTip().SetToolTip(btnnewcert, "Create new PFX certificate");
            new ToolTip().SetToolTip(btncert, "Browse to your PFX certificate File");
            new ToolTip().SetToolTip(txtclientid, "Google Drive Client ID");
            new ToolTip().SetToolTip(txtclisecret, "Google Drive Client Secret");
            new ToolTip().SetToolTip(txtfilter, "Only hash specific extension files eg. *.txt (all text files).You can also add multiple extensions eg.*.dll,*.exe");
            new ToolTip().SetToolTip(chkiconoverlay, "Windows Explorer plugin to check files at glance");
            new ToolTip().SetToolTip(chkboxpropertysheet, "Windows Explorer plugin to Check & hash folders from Folder's Property Page");

            // restore settings
            txtfilter.Text = Properties.Settings.Default.filter;
            txtwatchfol.Text = Properties.Settings.Default.watchfolder;
            watchfolder = txtwatchfol.Text;
            if (Properties.Settings.Default.iconoverlay == true)
                chkiconoverlay.Checked = true;
            if (Properties.Settings.Default.propertysheet == true)
                chkboxpropertysheet.Checked = true;

            if (!string.IsNullOrEmpty(txtwatchfol.Text) && Directory.Exists(txtwatchfol.Text)) //start previous watchman
            {
                fileSystemWatcher.EnableRaisingEvents = false;
                fileSystemWatcher.Path = txtwatchfol.Text;
                fileSystemWatcher.EnableRaisingEvents = true;
            }
            //
        }

        private void hasher_resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                trayicon.Visible = true;
                trayicon.ShowBalloonTip(500);
                this.Hide();
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                trayicon.Visible = false;

                this.Show();
            }

        }

        private void trayicon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void btncert_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "PrivateKey (*.pfx)|*.pfx|PublicKey (*.cer)|*.cer";
            openFileDialog1.ShowDialog();
            txtcerpath.Text = openFileDialog1.FileName;
            certpath = openFileDialog1.FileName;
            pass = txtpass.Text;
        }

        private void btncheck_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtbrowse.Text))
            {
                txtbrowse.BackColor = Color.White;

                if (!string.IsNullOrEmpty(certpath) && !string.IsNullOrEmpty(pass))
                {
                    certpath = openFileDialog1.FileName;
                    pass = txtpass.Text;
                    if (checkcert(certpath, pass))
                    {
                        datagrid.Rows.Clear();
                        check(foldertohash);
                    }
                }
                else
                {
                    if (certpath == null && selectedcert==null)
                        selectedcert = selectcert();

                    datagrid.Rows.Clear();
                    check(foldertohash);
                }
            }
            else
            {
                MessageBox.Show("Information incorrect. Make sure you selected folder,public key file");
                btnbrowse.Focus();
                txtbrowse.BackColor = Color.LightBlue;
            }
        }

        private void hashprogress_Click(object sender, EventArgs e)
        {

        }

        private void onfilecreate(object sender, System.IO.FileSystemEventArgs e)
        {
            Console.WriteLine("File Created: " + e.ChangeType + e.FullPath);
            Thread.Sleep(2000);

            if (File.GetAttributes(e.FullPath) != FileAttributes.Hidden && !(new FileInfo(e.FullPath).Extension == ".sha"))
            {
                showtraydialog("File Created", e.FullPath);
            }
            else if(new FileInfo(e.FullPath).Extension == ".sha")
            {
                showtraydialog("File Signed", e.FullPath,500,2000,500);
            }
        }

        private void showtraydialog(string title, string msg, int showdelay=1000, int staydelay=5000, int hidedelay=2000)
        {
            notify.Show(title, msg, showdelay, staydelay, hidedelay);
        }

        private void ContentClick(object obj, EventArgs ea)
        {
            TaskbarNotifier nobj = (TaskbarNotifier)obj;

            if (string.IsNullOrEmpty(certpath) | string.IsNullOrEmpty(pass))
            {
                selectedcert=selectcert();
                chkfromstore.CheckState = CheckState.Checked;
                txtcerpath.Enabled = false;
                txtpass.Enabled = false;
                hashsignfile(nobj.ContentText);
            }
            else
            {
                checkcert(certpath, pass);
                hashsignfile(nobj.ContentText);
            }
            //MessageBox.Show("Content was Clicked");
        }

        private void onchange(object sender, System.IO.FileSystemEventArgs e)
        { 
            Console.WriteLine("File " + e.FullPath + e.ChangeType);
        }

        private void btnwatchfolder_Click(object sender, EventArgs e)
        {
            try
            {
                folderBrowserDialog1.ShowDialog();
                watchfolder = folderBrowserDialog1.SelectedPath;
                txtwatchfol.Text = watchfolder;
                fileSystemWatcher.EnableRaisingEvents = false;
                fileSystemWatcher.Path = watchfolder;
                fileSystemWatcher.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void chkfromstore_CheckedChanged(object sender, EventArgs e)
        {
            if (chkfromstore.Checked)
            {
                txtcerpath.Enabled = false;
                txtpass.Enabled = false;
                btncert.Enabled = false;
            }
            else
            {
                txtcerpath.Enabled = true;
                txtpass.Enabled = true;
                btncert.Enabled = true;
            }
        }

        private void btnnewcert_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtpass.Text))
            {
                MessageBox.Show("New PFX certificate must be protected with password, Please enter password.", "Password Required");
                txtpass.BackColor = Color.LightBlue;
                txtpass.Focus();
                return;
            }
            txtpass.BackColor = Color.White;
            createpfx(txtpass.Text);
            X509Certificate2 cert = new X509Certificate2(System.IO.Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).FullName + "\\hasher.pfx", txtpass.Text); // add certificate to store

            /*if (MessageBox.Show("Warning: Actually its not safe to have certificate in Store"+
                ",malware running with Admin privilage can easily extract your private key. So its suggested that"+
                " always use it from pfx file", "Install to Store?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                System.Security.Cryptography.X509Certificates.X509Store store = new System.Security.Cryptography.X509Certificates.X509Store(StoreName.My, StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadWrite);
                store.Add(cert);
                store.Close();
                //File.Delete(Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).FullName + "\\hasher.pfx");
                
                chkfromstore.CheckState = CheckState.Checked;
            }*/
            selectedcert = cert;
        }

        private void toolStripMenuItemshow_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void toolStripMenuItemstopwatch_Click(object sender, EventArgs e)
        {
            try
            {
                if (toolStripMenuItemstopwatch.Text == "Stop Watching")
                {
                    fileSystemWatcher.EnableRaisingEvents = false;
                    toolStripMenuItemstopwatch.Text = "Start Watching";
                }
                else
                {
                    fileSystemWatcher.EnableRaisingEvents = true;
                    toolStripMenuItemstopwatch.Text = "Stop Watching";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Toolstripmenuitem Exception: "+ex.Message);
            }
        }

        private void toolStripMenuItemexit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void txtpass_TextChanged(object sender, EventArgs e)
        {
            pass = txtpass.Text;
        }

        private void chkshow_CheckedChanged(object sender, EventArgs e)
        {
            if (chkshow.CheckState == CheckState.Checked)
                this.Size = new Size(500, 736);
            else
                this.Size = new Size(500, 597);
        }

        #region shellextension

        private void chkboxpropertysheet_CheckedChanged(object sender, EventArgs e)
        {
            if (chkboxpropertysheet.CheckState == CheckState.Checked)
            {
                foreach (ServerEntry entry in shellextensions)
                {
                    if (entry.ServerType == ServerType.ShellPropertySheet && entry.ServerPath.Contains("Hasher")) //already loaded
                        return;
                }

                AddServer("HasherPropertySheet.dll");
            }
            else
            {
                foreach (ServerEntry entry in shellextensions)
                {
                    if (entry.ServerType == ServerType.ShellPropertySheet && entry.ServerPath.Contains("Hasher")) //already loaded
                        remove(entry);
                }
            }
        }

        private void chkiconoverlay_CheckedChanged(object sender, EventArgs e)
        {
            if (chkiconoverlay.CheckState == CheckState.Checked)
            {
                foreach (ServerEntry entry in shellextensions)
                {
                    if (entry.ServerType == ServerType.ShellIconOverlayHandler && entry.ServerPath.Contains("Hasher")) //already loaded
                        return;
                }

                AddServer("HasherFileIconOverlayHandler.dll");
            }
            else
            {
                foreach (ServerEntry entry in shellextensions)
                {
                    if (entry.ServerType == ServerType.ShellIconOverlayHandler && entry.ServerPath.Contains("Hasher")) //already loaded
                        remove(entry);  
                }
            }
        }

        public void remove(ServerEntry entry)
        {
            ServerRegistrationManager.UnregisterServer(entry.Server, RegistrationType.OS32Bit);
            ServerRegistrationManager.UninstallServer(entry.Server, RegistrationType.OS32Bit);

            ServerRegistrationManager.UnregisterServer(entry.Server, RegistrationType.OS64Bit);
            ServerRegistrationManager.UninstallServer(entry.Server, RegistrationType.OS64Bit);

            ExplorerManager.RestartExplorer();
        }

        public void AddServer(string path)
        {
            //  Load any servers from the assembly.
            var serverEntries = ServerManagerApi.LoadServers(path);

            foreach (ServerEntry entry in serverEntries)
            {
                ServerRegistrationManager.InstallServer(entry.Server, RegistrationType.OS32Bit, true);
                ServerRegistrationManager.RegisterServer(entry.Server, RegistrationType.OS32Bit);
                
                ServerRegistrationManager.InstallServer(entry.Server, RegistrationType.OS64Bit, true);
                ServerRegistrationManager.RegisterServer(entry.Server, RegistrationType.OS64Bit);

                shellextensions.Add(entry);
            }

            ExplorerManager.RestartExplorer();
        }

        #endregion

        private void btnhelp_Click(object sender, EventArgs e)
        {
            AboutBox1 about = new AboutBox1();
            about.Show();
        }

        private void chkfromserver_CheckedChanged(object sender, EventArgs e)
        {
            if (chkfromserver.CheckState == CheckState.Checked)
            {
                fromsignserver = true;
                client = new chatclient("192.168.43.1", 8086, "FileHasher", "FileHasher"); 
                client.newscript+=client_newscript;
            }
            else
            {
                fromsignserver = true;
                if (client != null)
                    client.abort();
            }
        }

        private void client_newscript(object sender, string data)
        {
            string[] compo = data.Split(';');
            string filename = compo[0];
            string signedxml = compo[1];

            Console.WriteLine(filename + Environment.NewLine + signedxml);
        }

        private void lblnotmatched_Click(object sender, EventArgs e)
        {

        }

        private void hasher_closing(object sender, FormClosingEventArgs e)
        {
            if(!string.IsNullOrEmpty(txtclientid.Text) && !string.IsNullOrEmpty(txtclisecret.Text))
            syncdb();

            //save settings
            Properties.Settings.Default.filter = txtfilter.Text;
            Properties.Settings.Default.watchfolder = txtwatchfol.Text;
            Properties.Settings.Default.iconoverlay = chkiconoverlay.Checked;
            Properties.Settings.Default.propertysheet = chkboxpropertysheet.Checked;
            Properties.Settings.Default.Save();
            //
        }

        public void syncdb()
        {
            if (!Directory.Exists("dbdump"))
                Directory.CreateDirectory("dbdump");

            MessageBox.Show("Syncking with Your Google Drive database Dump.Connect to Internet & Press Any Button to Start", "Please Wait");

            if (File.Exists("sqlite3.exe"))
            {
                if (File.Exists("dbdump//" + DateTime.Now.ToShortDateString() + ".sql"))
                    File.Delete("dbdump//" + DateTime.Now.ToShortDateString() + ".sql"); // remove old one

                Process p = System.Diagnostics.Process.Start("cmd.exe", "/c sqlite3.exe hasher.db .dump >>dbdump/" + DateTime.Now.ToShortDateString() + ".sql");

                while (!p.HasExited)
                {
                    Thread.Sleep(1000);
                }

                gdrive.init(txtclientid.Text, txtclisecret.Text);

                if (gdrive.getfolderid("hasherdb") == null) //folder dont exists //bcoz google dont prevent you from creating same named two directories
                    gdrive.createfolder("hasherdb", "File Hasher Database Folder");

                //gdrive.listfiles("hasherdb");
                gdrive.syncfiles("dbdump", "hasherdb");

                if (File.Exists("cat.exe"))
                {
                    foreach(string file in Directory.GetFiles("dbdump","*.sql"))
                        System.Diagnostics.Process.Start("cmd.exe", "/c cat.exe "+file+" hasher.db");
                }
                else
                {
                    MessageBox.Show("Fatal Error cat.exe not found,so I cannnot update databse. Download & place new cat.exe(gnucoreutils) in my folder", "Fatal Error");
                }
            }
            else
            {
                MessageBox.Show("Fatal Error sqlite3.exe not found,so I cannnot dump databse & sync with Google Drive. Download & place new sqlite3.exe in my folder", "Fatal Error");
            }
        }

        private void btnsync_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtclientid.Text) && !string.IsNullOrEmpty(txtclisecret.Text))
            {
                txtclientid.BackColor = Color.White;
                txtclisecret.BackColor = Color.White;
                syncdb();
            }
            else
            {
                MessageBox.Show("Enter Client ID & Client Secret");
                txtclientid.Focus();
                txtclientid.BackColor = Color.LightBlue;
                txtclisecret.BackColor = Color.LightBlue;
            }
        }

        private void hasher_dragdrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                if (Directory.Exists(file)) //coz same event for file & directory. This one is for folder
                {
                    string[] filez = Directory.GetFiles(file,"*.*",System.IO.SearchOption.AllDirectories);

                    foreach (string fileitem in filez)
                    {
                        if(!File.Exists(fileitem+".sha")) // dont have already created hash file
                            hashsignfile(file);
                    }
                }
                else if(File.Exists(file)) // this one is for file events
                {
                    if (!File.Exists(file + ".sha")) // dont have already created hash file
                        hashsignfile(file);
                }
            }
        }

        private void hasher_dragenter(object sender, DragEventArgs e)
        {
            if (selectedcert == null && (string.IsNullOrEmpty(certpath) & string.IsNullOrEmpty(pass)))
                selectedcert = selectcert();

            if (e.Data.GetDataPresent(DataFormats.FileDrop)) 
                e.Effect = DragDropEffects.Copy;
        }
    }

    #region chatclient

    public class chatclient
    {
        int port;
        string serverip;
        TcpClient client;
        private static ManualResetEvent ok = new ManualResetEvent(true); // coz threads may connect server out of sequence this causes incorrect file names

        byte[] readbuffer, writebuffer;
        //string last_msg;
        List<Thread> transfers;

        private Hashtable certificateErrors = new Hashtable();
        SslStream pipe;

        public string machineName, serverName;

        public delegate void script_handler(object sender, string file);
        public event script_handler newscript;

        public delegate void script_list_handler(object sender, string file);
        public event script_list_handler newscriptlist;

        ConcurrentQueue<string> msg;
        //List<string> msg2;

        public static int sentcount = 0;
        public static int storecount = 0;

        System.Timers.Timer clock = new System.Timers.Timer();

        //NetworkStream pipe;

        #region concurrency_control_message

        ConcurrentQueue<byte[]> writePendingData = new ConcurrentQueue<byte[]>();
        bool sendingData = false;

        void EnqueueDataForWrite(SslStream sslStream, byte[] buffer)
        {
            if (buffer == null)
                return;

            writePendingData.Enqueue(buffer);

            lock (writePendingData)
            {
                if (sendingData)
                {
                    return;
                }
                else
                {
                    sendingData = true;
                }
            }

            Write(sslStream);
        }

        void Write(SslStream sslStream)
        {
            byte[] buffer = null;
            try
            {
                if (writePendingData.Count > 0 && writePendingData.TryDequeue(out buffer))
                {
                    sslStream.BeginWrite(buffer, 0, buffer.Length, WriteCallback, sslStream);
                }
                else
                {
                    lock (writePendingData)
                    {
                        sendingData = false;
                    }
                }
            }
            catch (Exception)
            {
                // handle exception then
                lock (writePendingData)
                {
                    sendingData = false;
                }
            }
        }

        void WriteCallback(IAsyncResult ar)
        {
            SslStream sslStream = (SslStream)ar.AsyncState;
            try
            {
                sslStream.EndWrite(ar);
                //Console.WriteLine("sent msg..");
            }
            catch (Exception)
            {
                // handle exception                
            }

            Write(sslStream);
        }

        #endregion

        public chatclient(string ip, int port,string machinename, string servername)
        {
            msg = new ConcurrentQueue<string>();
            clock.Elapsed += clock_Elapsed;
            clock.Interval = 5000;
            clock.Enabled = true;

            this.machineName = machinename;
            this.serverName = servername;
            transfers = new List<Thread>();
            writebuffer = new byte[1024 * 10]; //10K
            readbuffer = new byte[1024 * 10]; //10k
            this.port = port;
            serverip = ip;
            //client = new TcpClient(serverip, port);
            client = new TcpClient();
            Console.WriteLine("Trying to connect server" + ip);
            try
            {
                client.BeginConnect(serverip, port, new AsyncCallback(connect_server), null);
            }
            catch (Exception e)
            {
                Console.WriteLine("Begin Connect " + e.Message);
                Console.WriteLine("\n Connection lost with server");
                Thread.Sleep(1000 * 60 * 2); //wait 2 min
                client.BeginConnect(serverip, port, new AsyncCallback(connect_server), null);
            }
        }

        void clock_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            string actiondata = null;
            clock.Enabled = false;
            //string[] tempdata = null;

            while(msg.TryDequeue(out actiondata))
            {
                action.actiondata data = action.xmlunwarpper(actiondata);
                //Console.WriteLine("type:" + data.action_type + " data: " + data.action_data);

                #region message
                if (data.action_type == actiontype.message) // means a message not other data..
                {
                    Console.WriteLine("Client says: {0}", data.action_data);
                }
                #endregion

                #region storedok
                if (data.action_type == actiontype.storedok) //store hash
                {
                    Console.WriteLine(data.action_data);
                    chatclient.storecount++;
                    //on_newscript(this, data.action_data);
                }
                #endregion

                #region gothash

                if (data.action_type == actiontype.hash)
                {
                    string []compo=data.action_data.Split(';');
                    string file = compo[0];
                    string hash = compo[1];

                    for (int i = 0; i < hasher.grid.Rows.Count; i++)
                    {
                        if (hasher.grid.Rows[i] != null) // this for last empty row causing ArgumentNull Exception
                        {
                            if (hasher.grid.Rows[i].Cells[0].Value.ToString().Contains(file) && hasher.grid.Rows[i].Cells[2].Value.ToString() == hash)
                            {
                                //hasher.grid.Rows[i].DefaultCellStyle.ForeColor = Color.Blue;
                                hasher.grid.Rows[i].Cells[1].Value = "Verified";
                                break; // ok next plz
                            }
                        }
                    }
                }

                #endregion
            }

            if(storecount==sentcount && sentcount!=0)
                MessageBox.Show("Android Server stored all hashes");

            clock.Enabled = true;
        }

        public bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            return false;
        }

        public void importcert(string path)
        {
            X509Certificate2 cert = new X509Certificate2(path);

            if (cert != null)
            {
                var store = new X509Store(StoreName.Root, StoreLocation.LocalMachine); // StoreName.Root
                store.Open(OpenFlags.ReadWrite);
                if (!store.Certificates.Contains(cert))
                {
                    store.Add(cert);
                }
            }
        }

        public void connect_server(IAsyncResult res)
        {
            try
            {
                client.EndConnect(res);
                Console.WriteLine("Client Connected to server");
                Console.WriteLine("\n Connected to server");
                pipe = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                pipe.AuthenticateAsClient(serverName);

                System.Media.SystemSounds.Hand.Play(); //audio notification

                pipe.BeginRead(readbuffer, 0, readbuffer.Length, new AsyncCallback(new_data), null);
            }
            catch (SocketException s)
            {
                Console.WriteLine(s.Message);
                //Thread.Sleep(1000 * 60 * 2); //wait 2 min
                Thread.Sleep(1000); //<<-- andro downloader test
                Console.WriteLine("\n Server Disconnected");
                System.Media.SystemSounds.Exclamation.Play(); //audio notification
                client.BeginConnect(serverip, port, new AsyncCallback(connect_server), null); // try again
            }
            catch (AuthenticationException e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
                Console.WriteLine("Authentication failed - closing the connection.");
                MessageBox.Show("Fatal Error: Server Certificate Invalid!!");
                client.Close();
                return;
            }
            /*catch (Exception e)
            {
                Console.WriteLine(e.Message);
                client.BeginConnect(serverip, port, new AsyncCallback(connect_server), null);
            }*/
        }

        public void new_data(IAsyncResult res)
        {
            //MessageBox.Show("new data"+pipe.DataAvailable);
            //Console.WriteLine("reading data from server..");
            try
            {
                int readbytes = pipe.EndRead(res); // how much bytes read

                StringBuilder new_msg = new StringBuilder();
                Decoder decoder = Encoding.UTF8.GetDecoder();
                char[] chars = new char[decoder.GetCharCount(readbuffer, 0, readbytes)];
                decoder.GetChars(readbuffer, 0, readbytes, chars, 0);
                new_msg.Append(chars);
                //Console.WriteLine("Client Debug print(new mag): "+new_msg);

                msg.Enqueue(new_msg.ToString());
                pipe.BeginRead(readbuffer, 0, readbuffer.Length, new AsyncCallback(new_data), null); //read for next
            }
            catch (System.IO.IOException ioe) // connection broken or server crashed
            {
                Console.WriteLine(ioe.Message);
                client = new TcpClient();
                Console.WriteLine("Trying to connect server" + serverip);
                System.Media.SystemSounds.Exclamation.Play(); //audio notification
                Thread.Sleep(1000 * 60 * 2); //wait 2 min
                client.BeginConnect(serverip, port, new AsyncCallback(connect_server), null);
            }
        }

        public void sendmsg(string msg, int type)
        {
            //last_sent_msg = msg;

            switch (type)
            {
                case msgtype.text:
                    //writebuffer = new byte[1024 * 10]; //10K
                    writebuffer = Encoding.UTF8.GetBytes(action.xmlwrapper(actiontype.message, msg));
                    EnqueueDataForWrite(pipe, writebuffer);
                    break;

                case msgtype.storehash:
                    writebuffer = Encoding.UTF8.GetBytes(action.xmlwrapper(actiontype.storehash, msg));
                    EnqueueDataForWrite(pipe, writebuffer);
                    break;

                case msgtype.returnhash:
                    writebuffer = Encoding.UTF8.GetBytes(action.xmlwrapper(actiontype.returnhash, msg));
                    EnqueueDataForWrite(pipe, writebuffer);
                    break;
            }
        }

        protected void on_newscript(object sender, string file)
        {
            if (newscript != null)
            {
                newscript(this, file);
            }
        }

        protected void on_newscriptlist(object sender, string file)
        {
            if (newscriptlist != null)
            {
                newscriptlist(this, file);
            }
        }

        public void abort() // just in case when client wants to abort everything in situations like Shutdown,restart events
        {
            client.Close();
        }
    }

    public struct msgtype
    {
        public const int text = 1;
        public const int storehash = 2;
        public const int returnhash = 3;
    }

    public struct actiontype
    {
        public const int message = 1;
        public const int hash = 2;
        public const int storedok = 3;
        public const int storehash = 4;
        public const int returnhash = 5;
    }

    public struct infotype
    {
        public const int INFORMATION = 1;
        public const int ALL_DEBUG_INFORMATION = 2;
        public const int WARNING = 3;
        public const int ERROR = 4;
        public const int FATAL_ERROR = 5;
    }

    public class action
    {
        public struct actiondata
        {
            public int action_type;
            public string action_data;
        }
        public static string xmlwrapper(int type, string action_data)
        {
            switch (type)
            {
                case actiontype.message:
                    return "<message>" + action_data + "</message>";

                case actiontype.storehash:
                    return "<storehash>" + action_data + "</storehash>";

                case actiontype.returnhash:
                    return "<returnhash>" + action_data + "</returnhash>";

                default:
                    return "unknown actiontype";
            }
        }

        public static actiondata xmlunwarpper(string data)
        {
            actiondata adata = new actiondata();

            string[] part = System.Text.RegularExpressions.Regex.Split(data, "(?<=>)|(?=<)");

            switch (part[1])
            {
                case "<message>":
                    adata.action_type = actiontype.message;
                    break;

                case "<hash>":
                    adata.action_type = actiontype.hash;
                    break;

                case "<storedok>":
                    adata.action_type = actiontype.storedok;
                    break;
            }

            adata.action_data = part[2];
            return adata;
        }
    }

    #endregion

    #region shell_classes

    public class ServerEntry
    {
        /// <summary>
        /// Gets or sets the name of the server.
        /// </summary>
        /// <value>
        /// The name of the server.
        /// </value>
        public string ServerName { get; set; }

        /// <summary>
        /// Gets or sets the server path.
        /// </summary>
        /// <value>
        /// The server path.
        /// </value>
        public string ServerPath { get; set; }

        /// <summary>
        /// Gets or sets the type of the server.
        /// </summary>
        /// <value>
        /// The type of the server.
        /// </value>
        public ServerType ServerType { get; set; }

        /// <summary>
        /// Gets or sets the class id.
        /// </summary>
        /// <value>
        /// The class id.
        /// </value>
        public Guid ClassId { get; set; }

        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        /// <value>
        /// The server.
        /// </value>
        public ISharpShellServer Server { get; set; }

        /// <summary>
        /// Gets the security status.
        /// </summary>
        /// <returns></returns>
        public string GetSecurityStatus()
        {
            AssemblyName asmName = AssemblyName.GetAssemblyName(ServerPath);
            var key = asmName.GetPublicKey();
            return key != null && key.Length > 0 ? "Signed" : "Not Signed";
        }
    }

    public static class ServerManagerApi
    {
        /// <summary>
        /// Loads all SharpShell servers from an assembly.
        /// </summary>
        /// <param name="path">The path to the assembly.</param>
        /// <returns>A ServerEntry for each SharpShell server in the assembly.</returns>
        public static IEnumerable<ServerEntry> LoadServers(string path)
        {
            //  Storage for the servers.
            var servers = new List<ServerEntry>();

            try
            {
                //  Create an assembly catalog for the assembly and a container from it.
                var catalog = new AssemblyCatalog(Path.GetFullPath(path));
                var container = new CompositionContainer(catalog);

                //  Get all exports of type ISharpShellServer.
                var serverTypes = container.GetExports<ISharpShellServer>();

                //  Go through each servertype (creating the instance from the lazy).
                foreach (var serverTypeInstance in serverTypes.Select(st => st.Value))
                {
                    //  Yield a server entry for the server type.
                    servers.Add(new ServerEntry
                    {
                        ServerName = serverTypeInstance.DisplayName,
                        ServerPath = path,
                        ServerType = serverTypeInstance.ServerType,
                        ClassId = serverTypeInstance.ServerClsid,
                        Server = serverTypeInstance
                    });

                }
            }
            catch (Exception)
            {
                //  It's almost certainly not a COM server.
                MessageBox.Show("The file '" + Path.GetFileName(path) + "' is not a SharpShell Server.", "Warning");
            }

            //  Return the servers.
            return servers;
        }
    }

    #endregion

    #region Authenticode_Checker

    internal static class AuthenticodeTools
    {
        [DllImport("Wintrust.dll", PreserveSig = true, SetLastError = false)]
        private static extern uint WinVerifyTrust(IntPtr hWnd, IntPtr pgActionID, IntPtr pWinTrustData);
        private static uint WinVerifyTrust(string fileName)
        {

            Guid wintrust_action_generic_verify_v2 = new Guid("{00AAC56B-CD44-11d0-8CC2-00C04FC295EE}");
            uint result = 0;
            using (WINTRUST_FILE_INFO fileInfo = new WINTRUST_FILE_INFO(fileName,
                                                                        Guid.Empty))
            using (UnmanagedPointer guidPtr = new UnmanagedPointer(Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Guid))),
                                                                   AllocMethod.HGlobal))
            using (UnmanagedPointer wvtDataPtr = new UnmanagedPointer(Marshal.AllocHGlobal(Marshal.SizeOf(typeof(WINTRUST_DATA))),
                                                                      AllocMethod.HGlobal))
            {
                WINTRUST_DATA data = new WINTRUST_DATA(fileInfo);
                IntPtr pGuid = guidPtr;
                IntPtr pData = wvtDataPtr;
                Marshal.StructureToPtr(wintrust_action_generic_verify_v2,
                                       pGuid,
                                       true);
                Marshal.StructureToPtr(data,
                                       pData,
                                       true);
                result = WinVerifyTrust(IntPtr.Zero,
                                        pGuid,
                                        pData);

            }
            return result;

        }
        public static bool IsTrusted(string fileName)
        {
            return WinVerifyTrust(fileName) == 0;
        }


    }

    internal struct WINTRUST_FILE_INFO : IDisposable
    {

        public WINTRUST_FILE_INFO(string fileName, Guid subject)
        {

            cbStruct = (uint)Marshal.SizeOf(typeof(WINTRUST_FILE_INFO));

            pcwszFilePath = fileName;



            if (subject != Guid.Empty)
            {

                pgKnownSubject = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Guid)));

                Marshal.StructureToPtr(subject, pgKnownSubject, true);

            }

            else
            {

                pgKnownSubject = IntPtr.Zero;

            }

            hFile = IntPtr.Zero;

        }

        public uint cbStruct;

        [MarshalAs(UnmanagedType.LPTStr)]

        public string pcwszFilePath;

        public IntPtr hFile;

        public IntPtr pgKnownSubject;



        #region IDisposable Members



        public void Dispose()
        {

            Dispose(true);

        }



        private void Dispose(bool disposing)
        {

            if (pgKnownSubject != IntPtr.Zero)
            {

                Marshal.DestroyStructure(this.pgKnownSubject, typeof(Guid));

                Marshal.FreeHGlobal(this.pgKnownSubject);

            }

        }



        #endregion

    }

    enum AllocMethod
    {
        HGlobal,
        CoTaskMem
    };
    enum UnionChoice
    {
        File = 1,
        Catalog,
        Blob,
        Signer,
        Cert
    };
    enum UiChoice
    {
        All = 1,
        NoUI,
        NoBad,
        NoGood
    };
    enum RevocationCheckFlags
    {
        None = 0,
        WholeChain
    };
    enum StateAction
    {
        Ignore = 0,
        Verify,
        Close,
        AutoCache,
        AutoCacheFlush
    };
    enum TrustProviderFlags
    {
        UseIE4Trust = 1,
        NoIE4Chain = 2,
        NoPolicyUsage = 4,
        RevocationCheckNone = 16,
        RevocationCheckEndCert = 32,
        RevocationCheckChain = 64,
        RecovationCheckChainExcludeRoot = 128,
        Safer = 256,
        HashOnly = 512,
        UseDefaultOSVerCheck = 1024,
        LifetimeSigning = 2048
    };
    enum UIContext
    {
        Execute = 0,
        Install
    };

    [StructLayout(LayoutKind.Sequential)]

    internal struct WINTRUST_DATA : IDisposable
    {

        public WINTRUST_DATA(WINTRUST_FILE_INFO fileInfo)
        {

            this.cbStruct = (uint)Marshal.SizeOf(typeof(WINTRUST_DATA));

            pInfoStruct = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(WINTRUST_FILE_INFO)));

            Marshal.StructureToPtr(fileInfo, pInfoStruct, false);

            this.dwUnionChoice = UnionChoice.File;



            pPolicyCallbackData = IntPtr.Zero;

            pSIPCallbackData = IntPtr.Zero;



            dwUIChoice = UiChoice.NoUI;

            fdwRevocationChecks = RevocationCheckFlags.None;

            dwStateAction = StateAction.Ignore;

            hWVTStateData = IntPtr.Zero;

            pwszURLReference = IntPtr.Zero;

            dwProvFlags = TrustProviderFlags.Safer;



            dwUIContext = UIContext.Execute;

        }



        public uint cbStruct;

        public IntPtr pPolicyCallbackData;

        public IntPtr pSIPCallbackData;

        public UiChoice dwUIChoice;

        public RevocationCheckFlags fdwRevocationChecks;

        public UnionChoice dwUnionChoice;

        public IntPtr pInfoStruct;

        public StateAction dwStateAction;

        public IntPtr hWVTStateData;

        private IntPtr pwszURLReference;

        public TrustProviderFlags dwProvFlags;

        public UIContext dwUIContext;



        #region IDisposable Members



        public void Dispose()
        {

            Dispose(true);

        }



        private void Dispose(bool disposing)
        {

            if (dwUnionChoice == UnionChoice.File)
            {

                WINTRUST_FILE_INFO info = new WINTRUST_FILE_INFO();

                Marshal.PtrToStructure(pInfoStruct, info);

                info.Dispose();

                Marshal.DestroyStructure(pInfoStruct, typeof(WINTRUST_FILE_INFO));

            }



            Marshal.FreeHGlobal(pInfoStruct);

        }



        #endregion

    }

    internal sealed class UnmanagedPointer : IDisposable
    {

        private IntPtr m_ptr;

        private AllocMethod m_meth;

        internal UnmanagedPointer(IntPtr ptr, AllocMethod method)
        {

            m_meth = method;

            m_ptr = ptr;

        }



        ~UnmanagedPointer()
        {

            Dispose(false);

        }



        #region IDisposable Members

        private void Dispose(bool disposing)
        {

            if (m_ptr != IntPtr.Zero)
            {

                if (m_meth == AllocMethod.HGlobal)
                {

                    Marshal.FreeHGlobal(m_ptr);

                }

                else if (m_meth == AllocMethod.CoTaskMem)
                {

                    Marshal.FreeCoTaskMem(m_ptr);

                }

                m_ptr = IntPtr.Zero;

            }



            if (disposing)
            {

                GC.SuppressFinalize(this);

            }

        }



        public void Dispose()
        {

            Dispose(true);

        }



        #endregion



        public static implicit operator IntPtr(UnmanagedPointer ptr)
        {

            return ptr.m_ptr;

        }

    }

    #endregion
}
