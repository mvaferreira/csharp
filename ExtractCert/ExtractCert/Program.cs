using System;

namespace ExtractCert
{
    class Program
    {
        static void Main(string[] args)
        {
            string type = string.Empty;
            string hostname = string.Empty;
            string port = string.Empty;
            string fullPath = string.Empty;
            bool viewOnly = false;
            bool saveChain = false;
            int maxParams = args.Length - 1;

            if (maxParams >= 4)
            {
                for (int i = 0; i <= maxParams; i++)
                {
                    switch (args[i])
                    {
                        case "-web":
                            type = "web";
                            break;
                        case "-tcp":
                            type = "tcp";
                            break;
                        case "-h":
                            if (maxParams > i)
                            {
                                hostname = args[i + 1];
                            }
                            break;
                        case "-p":
                            if (maxParams > i)
                            {
                                port = args[i + 1];
                            }
                            break;
                        case "-view":
                            viewOnly = true;
                            break;
                        case "-savechain":
                            saveChain = true;
                            break;
                        case "-certpath":
                            if (maxParams > i)
                            {
                                fullPath = args[i + 1];
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                ShowHelp();
                return;
            }

            try
            {
                if (saveChain && fullPath == string.Empty)
                {
                    Console.WriteLine("ExtractCert.exe: -savechain specified, must specify certificate path with -certpath.\r\n");
                    ShowHelp();
                    return;
                }

                if (type != string.Empty && hostname != string.Empty && port != string.Empty)
                {
                    if (!viewOnly && fullPath == string.Empty)
                    {
                        viewOnly = true;
                    }

                    CertUtil certUtil = new CertUtil();

                    if (type == "web")
                    {
                        if (certUtil.SaveWebCert(hostname, Convert.ToInt32(port), fullPath, viewOnly, saveChain))
                        {
                            if (!viewOnly) Console.WriteLine("\r\n\r\nExtractCert: Certificate saved successfully.");
                        }
                    }
                    else if (type == "tcp")
                    {
                        if (certUtil.SaveTCPCert(hostname, Convert.ToInt32(port), fullPath, viewOnly, saveChain))
                        {
                            if (!viewOnly) Console.WriteLine("\r\n\r\nExtractCert: Certificate saved successfully.");
                        }
                    }
                } else
                {
                    ShowHelp();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Main: {0}\r\n{1}", e.Message, e.StackTrace);
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Usage: ExtractCert.exe <-web|-tcp> <-h hostname> <-p port> [<-view>|<-certpath c:\\temp\\cert.cer>] [-savechain]" +
                               "\r\n\r\nSave mode:" +
                               "\r\nExtractCert.exe -web -h www.microsoft.com -p 443 -certpath c:\\temp\\microsoft.cer" +
                               "\r\nExtractCert.exe -tcp -h dc1.contoso.com -p 636 -certpath c:\\temp\\ldaps.cer" +
                               "\r\n\r\nExtractCert.exe -web -h www.microsoft.com -p 443 -certpath c:\\temp\\microsoft.cer -savechain" +
                               "\r\nExtractCert.exe -tcp -h dc1.contoso.com -p 636 -certpath c:\\temp\\ldaps.cer -savechain" +
                               "\r\n\r\nView mode:" +
                               "\r\nExtractCert.exe -web -h www.microsoft.com -p 443 -view" +
                               "\r\nExtractCert.exe -tcp -h dc1.contoso.com -p 636 -view");
            return;
        }
    }
}