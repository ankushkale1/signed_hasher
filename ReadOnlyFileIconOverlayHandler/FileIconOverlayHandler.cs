using System;
using System.Collections.Generic;
using Alphaleonis.Win32.Filesystem;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using SharpShell.Interop;
using SharpShell.SharpIconOverlayHandler;
using System.Xml.Linq;
using System.Security.Cryptography;
using System.Xml;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;

namespace ReadOnlyFileIconOverlayHandler
{

    [ComVisible(true)]
    public class FileIconOverlayHandler : SharpIconOverlayHandler
    {
        static System.Diagnostics.EventLog appLog;

        public FileIconOverlayHandler()
        {
            //System.Windows.Forms.MessageBox.Show("Init");ws
            try
            {
                if (!EventLog.SourceExists("IconOverlayHasher"))
                {
                    EventLog.CreateEventSource("IconOverlayHasher", "Application");
                    //System.Windows.Forms.MessageBox.Show("not exists");
                    appLog = new System.Diagnostics.EventLog();
                    appLog.Source = "IconOverlayHasher";
                }
                else
                {
                    //System.Windows.Forms.MessageBox.Show("exists");
                    appLog = new System.Diagnostics.EventLog();
                    appLog.Source = "IconOverlayHasher";
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.StackTrace);
            }     
        }

        public enum icontype
        {
            hashok = 0,
            hashwrong = 1,
            nohash = 3
        }

        static X509Certificate2 selectedcert;
        int type;

        protected override int GetPriority()
        {
            //  The read only icon overlay is very low priority.
            return 0;
        }

        protected override bool CanShowOverlay(string path, FILE_ATTRIBUTE attributes)
        {
            try
            {
                //  Get the file attributes.
                //var fileAttributes = new FileInfo(path);

                if (File.Exists(path + ".sha"))
                {
                    FileInfo info = new FileInfo(path);
                    long mb = (info.Length / 1024) / 1024;
                    appLog.WriteEntry("File: " + path + " size: " + mb + " MB");

                    if (mb > 500 && System.Windows.Forms.MessageBox.Show(info.Name+" is over 500MB do you want to check file?", "FileHasher", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                       return false;

                    string hash = readhashes(path + ".sha");
                    if (hashfile(path) == hash)
                    {
                        type = (int)icontype.hashok;
                        appLog.WriteEntry("Hash OK: " + path, System.Diagnostics.EventLogEntryType.Information);
                        //System.Windows.Forms.MessageBox.Show(type.ToString());
                        return true;
                    }
                    else
                    {
                        appLog.WriteEntry("Hash Fail: " + path, System.Diagnostics.EventLogEntryType.Information);
                        //type = (int)icontype.hashwrong;
                        //System.Windows.Forms.MessageBox.Show(type.ToString());
                        //return true;
                        return false;
                    }
                }
                else
                {
                    //type = (int)icontype.nohash;
                    //System.Windows.Forms.MessageBox.Show(type.ToString());
                    // return true;
                    return false;
                }

                return false;
            }
            catch (Exception e)
            {
                //type = (int)icontype.nohash;
                //return true;
                appLog.WriteEntry(e.Message + e.StackTrace, System.Diagnostics.EventLogEntryType.Warning);
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
                    appLog.WriteEntry("Doc verification failed: " + path, System.Diagnostics.EventLogEntryType.Information);
                    //Console.WriteLine("Document Verification Failed");
                    xfile.Close();
                    return "error";
                }
                return "notfound";
            }
            catch (Exception e)
            {
                appLog.WriteEntry(e.Message + e.StackTrace, System.Diagnostics.EventLogEntryType.Warning);
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
                appLog.WriteEntry(e.Message + e.StackTrace, System.Diagnostics.EventLogEntryType.Warning);
                //Console.WriteLine(e.Message + e.TargetSite);
                return false;
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
                filedata = File.Open(file, FileMode.Open, FileAccess.Read); // to tackle with out of memory exception hashing 1gb+ file
                hash = hasher.ComputeHash(filedata);
                //sfile.Close();
                hashdata = System.BitConverter.ToString(hash);
                hashdata = hashdata.Replace("-", "");

            }
            catch (Exception e)
            {
                appLog.WriteEntry(e.Message + e.StackTrace, System.Diagnostics.EventLogEntryType.Warning);
                //Console.WriteLine(e.Message);
            }
            finally
            {
                if(filedata!=null)
                    filedata.Close();
            }
            return hashdata;
        }

        private static X509Certificate2 selectcert() //select cert from store & set selectedcert
        {
            try
            {
                if (File.Exists("c:\\hasher.cer"))
                {
                    X509Certificate2 cert = new X509Certificate2("c:\\hasher.cer");
                    appLog.WriteEntry("Selected Cert: " + cert.Subject, System.Diagnostics.EventLogEntryType.Information);
                    return cert;
                }
                else
                {
                    System.Security.Cryptography.X509Certificates.X509Store mystore = new System.Security.Cryptography.X509Certificates.X509Store(StoreLocation.CurrentUser);
                    mystore.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                    X509Certificate2Collection certCollection = (X509Certificate2Collection)mystore.Certificates;
                    X509Certificate2Collection foundCollection = (X509Certificate2Collection)certCollection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
                    X509Certificate2Collection selectedcollection = X509Certificate2UI.SelectFromCollection(foundCollection, "Select a Certificate.", "Select a Certificate from the following list to get information on that certificate", X509SelectionFlag.SingleSelection);
                    if (selectedcollection.Count > 0)
                    {
                        X509Certificate2 certz = selectedcollection[0];
                        appLog.WriteEntry("Selected Cert: " + certz.Subject, System.Diagnostics.EventLogEntryType.Information);
                        return certz;
                    }
                    appLog.WriteEntry("No Cert Selected", System.Diagnostics.EventLogEntryType.Warning);
                    return null;
                }
            }
            catch (Exception e)
            {
                appLog.WriteEntry(e.Message + e.StackTrace, System.Diagnostics.EventLogEntryType.Warning);
                return null;
            }
        }

        protected override System.Drawing.Icon GetOverlayIcon()
        {
            //System.Windows.Forms.MessageBox.Show(type.ToString());

            return Properties.Resources.Check;

            /*if (type == (int)icontype.hashok)
            {
                type = -1;
                return Properties.Resources.Check;
            }
            else //if (type == (int)icontype.hashwrong)
            {
                type = -1;
                return Properties.Resources.Cross;
            }*/
            /*else
            {
                type = -1;
                return Properties.Resources.Help;
            }*/
 
        }
    }
}
