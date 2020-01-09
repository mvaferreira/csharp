using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ClientLogging
{
    public enum Protocol
    {
        TCP = 6,
        UDP = 17,
        Unknown = -1
    };

    public class Sniffer
    {
        Socket _socket;
        Thread thrStartCapturing = null;
        readonly byte[] _bIn = new byte[4] { 1, 0, 0, 0 };
        readonly byte[] _bOut = new byte[4];
        readonly byte[] _bBuffer = new byte[262144];
        readonly bool _stopCapturing = false;
        string ip = string.Empty;
        IP ipHeader;
        Database db = new Database();

        public Sniffer(string ip)
        {
            this.ip = ip;
        }

        public void StartCapturing()
        {
            try
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);
                _socket.Bind(new IPEndPoint(IPAddress.Parse(ip), 0));
                _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);
                _socket.IOControl(IOControlCode.ReceiveAll, _bIn, _bOut);

                thrStartCapturing = new Thread(StartReceiving)
                {
                    Name = "Capture Thread"
                };

                thrStartCapturing.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("StartCapturing: {0}\n{1}", e.Message, e.StackTrace);
            }
        }

        private void StartReceiving()
        {
            while (!_stopCapturing)
            {
                try
                {
                    int size = _socket.ReceiveBufferSize;
                    int bytesReceived = _socket.Receive(_bBuffer, 0, _bBuffer.Length, SocketFlags.None);
                    var str = string.Empty;

                    if (bytesReceived > 0)
                    {
                        //IP
                        ipHeader = new IP(_bBuffer, bytesReceived);
                        ParseData(ipHeader);
                    }

                    Array.Clear(_bBuffer, 0, _bBuffer.Length);
                }
                catch(Exception e)
                {
                    Console.WriteLine("StartReceiving: {0}\n{1}", e.Message, e.StackTrace);
                }
            }
        }

        private void ParseData(IP ipHeader)
        {
            try
            {
                switch (ipHeader.ProtocolType)
                {
                    //TCP
                    case Protocol.TCP:
                        TCP tcpHeader = new TCP(ipHeader.Data, ipHeader.MessageLength);
                        if (ipHeader.SourceAddress.ToString() != ip && tcpHeader.Flags == "0x02 (SYN)")
                        {
                            db.AddClient(Dns.GetHostEntry(ipHeader.SourceAddress).HostName ?? ipHeader.SourceAddress.ToString(),
                                         ipHeader.SourceAddress.ToString(),
                                         tcpHeader.SourcePort, ipHeader.DestinationAddress.ToString(),
                                         tcpHeader.DestinationPort,
                                         "TCP",
                                         DateTime.Now.ToString());
                        }
                        break;

                    //UDP
                    case Protocol.UDP:
                        UDP udpHeader = new UDP(ipHeader.Data, (int)ipHeader.MessageLength);
                        if (ipHeader.SourceAddress.ToString() != ip && ipHeader.DestinationAddress.ToString() == ip)
                        {
                            //ignore udp datagrams sent to ports higher than 49152,
                            //since this isn't connection oriented, assume those datagrams are replies
                            //from servers, making this a client.
                            if (Convert.ToInt32(udpHeader.DestinationPort) < 49152)
                            {
                                db.AddClient(Dns.GetHostEntry(ipHeader.SourceAddress).HostName ?? ipHeader.SourceAddress.ToString(),
                                             ipHeader.SourceAddress.ToString(),
                                             udpHeader.SourcePort,
                                             ipHeader.DestinationAddress.ToString(),
                                             udpHeader.DestinationPort,
                                             "UDP",
                                             DateTime.Now.ToString());
                            }
                        }
                        break;

                    case Protocol.Unknown:
                        break;
                }
            }
            catch(SocketException) { }
            catch(Exception e)
            {
                Console.WriteLine("ParseData: {0}\n{1}", e.Message, e.StackTrace);
            }
        }
    }
}