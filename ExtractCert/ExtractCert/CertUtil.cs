using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ExtractCert
{
    class CertUtil
    {
        const string SANOID = "2.5.29.17";
        FileStream fs1;
        public bool SaveTCPCert(string hostname, int port, string certPath, bool viewOnly, bool saveChain)
        {
            try
            {
                using (TcpClient client = new TcpClient(hostname, port))
                {
                    var certValidation = new RemoteCertificateValidationCallback(delegate (object snd, X509Certificate certificate, X509Chain chainLocal, SslPolicyErrors sslPolicyErrors)
                    {
                        return true;
                    });

                    using (var sslStream = new SslStream(client.GetStream(), true, certValidation))
                    {
                        sslStream.AuthenticateAsClient(hostname);
                        var srvCert = sslStream.RemoteCertificate;
                        X509Certificate2 cert = new X509Certificate2(srvCert);
                        string pemCert = ConvertToPem(cert);

                        if (!viewOnly)
                        {
                            PrintColored("Subject: ", cert.Subject);

                            foreach (var ext in cert.Extensions)
                            {
                                if (ext.Oid.Value.ToString() == SANOID)
                                {
                                    PrintColored("Subject Alternative Name:", string.Empty);
                                    AsnEncodedData asndata = new AsnEncodedData(ext.Oid, ext.RawData);
                                    Console.WriteLine(asndata.Format(true));
                                    break;
                                }
                            }

                            PrintColored("Issuer: ", cert.IssuerName.Name);
                            PrintColored("Thumbprint: ", cert.Thumbprint);
                            PrintColored("Serial#: ", cert.SerialNumber);
                            PrintColored("Valid From: ", cert.NotBefore.ToString());
                            PrintColored("Valid To: ", cert.NotAfter.ToString());
                            PrintColored("Signature Algorithm: ", cert.SignatureAlgorithm.FriendlyName);

                            if (saveChain)
                            {
                                X509Chain chain = new X509Chain();
                                chain.Build(cert);
                                string chainPemCert = string.Empty;
                                int i = 0;
                                i = chain.ChainElements.Count;

                                PrintColored("\r\n---> Cert hierarchy (chain): ", string.Empty);

                                foreach (X509ChainElement el in chain.ChainElements)
                                {
                                    chainPemCert = ConvertToPem(el.Certificate);
                                    PrintColored("\tSubject " + i + ": ", el.Certificate.Subject);

                                    fs1 = new FileStream(certPath.Replace(".cer", "_" + i + ".cer"), FileMode.OpenOrCreate, FileAccess.Write);
                                    fs1.Write(Encoding.UTF8.GetBytes(chainPemCert), 0, chainPemCert.Length);
                                    fs1.Close();
                                    fs1.Dispose();

                                    i--;
                                }
                            } else
                            {
                                fs1 = new FileStream(certPath, FileMode.OpenOrCreate, FileAccess.Write);
                                fs1.Write(Encoding.UTF8.GetBytes(pemCert), 0, pemCert.Length);
                                fs1.Close();
                                fs1.Dispose();
                            }
                        }
                        else
                        {
                            DisplayCert(cert);
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("SaveTCPCert: {0}\r\n{1}", e.Message, e.StackTrace);
                return false;
            }
        }

        public bool SaveWebCert(string hostname, int port, string certPath, bool viewOnly, bool saveChain)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create("https://" + hostname + ":" + port);
                request.AllowAutoRedirect = false;
                HttpWebResponse response = (HttpWebResponse) request.GetResponse();
                response.Close();
                X509Certificate2 cert = new X509Certificate2(request.ServicePoint.Certificate);
                string pemCert = ConvertToPem(cert);

                if (!viewOnly)
                {
                    Console.WriteLine("Subject: {0}", cert.Subject);
                    foreach (var ext in cert.Extensions)
                    {
                        if (ext.Oid.Value.ToString() == SANOID)
                        {
                            PrintColored("Subject Alternative Name:", string.Empty);
                            AsnEncodedData asndata = new AsnEncodedData(ext.Oid, ext.RawData);
                            Console.WriteLine(asndata.Format(true));
                            break;
                        }
                    }
                    PrintColored("Issuer: ", cert.IssuerName.Name);
                    PrintColored("Thumbprint: ", cert.Thumbprint);
                    PrintColored("Serial#: ", cert.SerialNumber);
                    PrintColored("Valid From: ", cert.NotBefore.ToString());
                    PrintColored("Valid To: ", cert.NotAfter.ToString());
                    PrintColored("Signature Algorithm: ", cert.SignatureAlgorithm.FriendlyName);

                    if (saveChain)
                    {
                        X509Chain chain = new X509Chain();
                        chain.Build(cert);
                        string chainPemCert = string.Empty;
                        int i = 0;
                        i = chain.ChainElements.Count;

                        PrintColored("\r\n---> Cert hierarchy (chain): ", string.Empty);

                        foreach (X509ChainElement el in chain.ChainElements)
                        {
                            chainPemCert = ConvertToPem(el.Certificate);

                            PrintColored("\tSubject " + i + ": ", el.Certificate.Subject);

                            fs1 = new FileStream(certPath.Replace(".cer", "_" + i + ".cer"), FileMode.OpenOrCreate, FileAccess.Write);
                            fs1.Write(Encoding.UTF8.GetBytes(chainPemCert), 0, chainPemCert.Length);
                            fs1.Close();
                            fs1.Dispose();

                            i--;
                        }
                    } else
                    {
                        fs1 = new FileStream(certPath, FileMode.OpenOrCreate, FileAccess.Write);
                        fs1.Write(Encoding.UTF8.GetBytes(pemCert), 0, pemCert.Length);
                        fs1.Close();
                        fs1.Dispose();
                    }
                } else
                {
                    DisplayCert(cert);
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("SaveWebCert: {0}\r\n{1}", e.Message, e.StackTrace);
                return false;
            }
        }

        private static string ConvertToPem(X509Certificate cert)
        {
            StringBuilder builder = new StringBuilder();

            try
            {
                builder.AppendLine("-----BEGIN CERTIFICATE-----");
                builder.AppendLine(Convert.ToBase64String(cert.Export(X509ContentType.Cert), Base64FormattingOptions.InsertLineBreaks));
                builder.AppendLine("-----END CERTIFICATE-----");
            }
            catch(Exception e)
            {
                Console.WriteLine("ExportToPEM: {0}\r\n{1}", e.Message, e.StackTrace);
            }

            return builder.ToString();
        }

        private void DisplayCert(X509Certificate2 cert)
        {
            try
            {
                PrintColored("Subject: ", cert.Subject);
                foreach (var ext in cert.Extensions)
                {
                    if (ext.Oid.Value.ToString() == SANOID)
                    {
                        PrintColored("Subject Alternative Name:\r\n", string.Empty);
                        AsnEncodedData asndata = new AsnEncodedData(ext.Oid, ext.RawData);
                        Console.WriteLine(asndata.Format(true));
                    }
                }

                PrintColored("Issuer: ", cert.IssuerName.Name);
                PrintColored("Thumbprint: ", cert.Thumbprint);
                PrintColored("Serial#: ", cert.SerialNumber);
                PrintColored("Valid From: ", cert.NotBefore.ToString());
                PrintColored("Valid To: ", cert.NotAfter.ToString());
                PrintColored("Signature Algorithm: ", cert.SignatureAlgorithm.FriendlyName);
                Console.WriteLine("\r\nExtensions:\r\n");

                foreach (var ext in cert.Extensions)
                {
                    if (ext.Oid.Value.ToString() != SANOID)
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("---> {0} {1}", ext.Oid.Value, ext.Oid.FriendlyName);
                        Console.ResetColor();
                        AsnEncodedData asndata = new AsnEncodedData(ext.Oid, ext.RawData);
                        Console.WriteLine(asndata.Format(true));
                    }
                }

                X509Chain chain = new X509Chain();
                chain.Build(cert);
                int i = 1;
                PrintColored("---> Cert hierarchy (chain): ", string.Empty);

                foreach (X509ChainElement el in chain.ChainElements)
                {
                    PrintColored("\tSubject " + i + ": ", el.Certificate.Subject);

                    i++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("DisplayCert: {0}\r\n{1}", e.Message, e.StackTrace);
            }
        }

        private void PrintColored(string msg, string value)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(msg);
            Console.ResetColor();
            Console.WriteLine(value);
        }
    }
}
