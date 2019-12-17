using System;

namespace IPSniffer
{
    class Program
    {
        static void Main(string[] args)
        {
            SnifferProgram prog;
            string proto = string.Empty;
            string port = string.Empty;
            string localIP = string.Empty;

            if (args.Length > 1)
            {
                for (int i=0; i<args.Length; i++)
                {
                    switch(args[i])
                    {
                        case "-localIP":
                        case "-localip":
                            if (args[i + 1] != string.Empty)
                            {
                                localIP = args[i + 1];
                            }
                            break;
                        case "-P":
                        case "-p":
                            if (args[i + 1] != string.Empty)
                            {
                                proto = args[i + 1];
                            }
                            break;
                        case "-PORT":
                        case "-port":
                            if (args[i + 1] != string.Empty)
                            {
                                port = args[i + 1];
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                if (localIP == string.Empty)
                {
                    Console.WriteLine("Specify local IP with -localIP parameter");
                    Console.WriteLine();
                    ShowHelp();
                    return;
                }
            }

            if (proto == string.Empty && port == string.Empty)
            {
                prog = new SnifferProgram(localIP);
            }
            else
            {
                prog = new SnifferProgram(localIP, proto, port);
            }

            prog.StartCapturing();
        }
        private static void ShowHelp()
        {
            Console.WriteLine("Usage: IPSniffer.exe -localIP <listen_ip> [-p tcp|udp] [-port 1-65535]");
            return;
        }
    }
}