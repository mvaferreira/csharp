using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

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
                            new Database().ShowClients();
                            break;
                        case "-exportcsv":
                            ExportCSV(new Database().ExportClients());
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

        private static void ExportCSV(List<string> clients)
        {
            FileStream fs1;

            try
            {
                if (null != clients)
                {
                    if (clients.Count > 0)
                    {
                        fs1 = new FileStream(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\ClientLogging.csv",
                                             FileMode.OpenOrCreate,
                                             FileAccess.Write);

                        foreach (string client in clients)
                        {
                            fs1.Write(Encoding.UTF8.GetBytes(client), 0, client.Length);
                        }

                        fs1.Flush();
                        fs1.Close();
                        fs1.Dispose();
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("ExportCSV: {0}\n{1}", e.Message, e.StackTrace);
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("ClientLogging.exe [-view] [-exportcsv]");
        }
    }
}