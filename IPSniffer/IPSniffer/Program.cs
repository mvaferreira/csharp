using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace IPSniffer
{
    class Program
    {
        static void Main(string[] args)
        {
            string proto = string.Empty;
            string port = string.Empty;
            string localIP = string.Empty;
            int maxParams = args.Length - 1;

            if (maxParams >= 0)
            {
                for (int i=0; i <= maxParams; i++)
                {
                    switch(args[i])
                    {
                        case "-H":
                        case "-h":
                            ShowHelp();
                            Environment.Exit(0);
                            break;
                        case "-localIP":
                        case "-localip":
                            if (maxParams > i)
                            {
                                localIP = args[i + 1];
                            }
                            break;
                        case "-P":
                        case "-p":
                            if (maxParams > i)
                            {
                                proto = args[i + 1];
                            }
                            break;
                        case "-PORT":
                        case "-port":
                            if (maxParams > i)
                            {
                                port = args[i + 1];
                            }
                            break;
                        default:
                            break;
                    }
                }

                //Checking if valid parameters specified.
                if (localIP != string.Empty)
                {
                    if (localIP != string.Empty && proto != string.Empty && port != string.Empty)
                    {
                        new Sniffer(localIP, proto, port).StartCapturing();
                    }
                    else
                    {
                        new Sniffer(localIP).StartCapturing();
                    }
                }
                else
                {
                    //-localIP switch specified, but no IP address added.
                    ShowHelp();
                    Environment.Exit(-1);
                }
            }
            else
            {
                //No parameter specified.
                if (localIP == string.Empty)
                {
                    var IPv4Addresses = Dns.GetHostEntry(Dns.GetHostName())
                    .AddressList.Where(al => al.AddressFamily == AddressFamily.InterNetwork)
                    .AsEnumerable();

                    foreach (IPAddress ip in IPv4Addresses)
                    {
                        new Sniffer(ip.ToString()).StartCapturing();
                    }
                }
            }
        }
        private static void ShowHelp()
        {
            Console.WriteLine("Usage: IPSniffer.exe [-h] [-localIP <listen_ip>] [-p tcp|udp] [-port 1-65535]");
        }
    }
}