using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace ClientLogging
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                for (int i = 0; i <= args.Length - 1; i++)
                {
                    switch (args[i])
                    {
                        case "-view":
                            Database db = new Database();
                            db.ShowClients();
                            break;
                        default:
                            ShowHelp();
                            break;
                    }
                }
            }
            else
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

        private static void ShowHelp()
        {
            Console.WriteLine("ClientLogging.exe [-view]");
        }
    }
}