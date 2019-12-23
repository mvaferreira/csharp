using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace IPSniffer
{
    public enum Protocol
    {
        ICMP = 1,
        IGMP = 2,
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
        readonly byte[] _bBuffer = new byte[8192];
        readonly bool _stopCapturing = false;
        string output = string.Empty;
        string proto = string.Empty;
        string port = string.Empty;
        string localIP = string.Empty;
        IP ipHeader;

        public Sniffer(string localIP) 
        {
            this.localIP = localIP;
        }

        public Sniffer(string localIP, string proto, string port)
        {
            this.localIP = localIP;
            this.proto = proto;
            this.port = port;
        }

        public void StartCapturing()
        {
            try
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);
                _socket.Bind(new IPEndPoint(IPAddress.Parse(localIP), 0));
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
                Console.WriteLine(e.Message);
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
                    Console.WriteLine("StartReceiving: " + e.Message);
                }
            }
        }

        private void ParseData(IP ipHeader)
        {
            string showData = proto.ToLower() + port.ToLower();

            switch (ipHeader.ProtocolType)
            {
                //TCP
                case Protocol.TCP:
                    TCP tcpHeader = new TCP(ipHeader.Data, ipHeader.MessageLength);

                    if (showData == "tcp")
                    {
                        Console.WriteLine("[{0}] {1}:{2} -> {3}:{4} | {5}", DateTime.Now, ipHeader.SourceAddress, tcpHeader.SourcePort, ipHeader.DestinationAddress, tcpHeader.DestinationPort, "IP > TCP");
                    }

                    //DNS
                    if ((tcpHeader.DestinationPort == "53" || tcpHeader.SourcePort == "53") && (showData == "tcp53" || showData == string.Empty))
                    {
                        DNS dnsHeader = new DNS(tcpHeader.Data, (int)tcpHeader.MessageLength);

                        Console.WriteLine("[{0}] {1}:{2} -> {3}:{4} | {5}", DateTime.Now, ipHeader.SourceAddress, tcpHeader.SourcePort, ipHeader.DestinationAddress, tcpHeader.DestinationPort, "IP > TCP > DNS");
                        Console.WriteLine("Identification: {0}\r\nFlags: {1}\r\nQuestions: {2}\r\nAnswer RRs: {3}\r\nAuthority RRs: {4}\r\nAdditional RRs: {5}", dnsHeader.Identification, dnsHeader.Flags, dnsHeader.TotalQuestions, dnsHeader.TotalAnswerRRs, dnsHeader.TotalAuthorityRRs, dnsHeader.TotalAdditionalRRs);

                        output = System.Text.Encoding.Default.GetString(tcpHeader.Data);
                        Console.WriteLine(output.Trim());
                    }

                    //HTTP
                    if ((tcpHeader.DestinationPort == "80" || tcpHeader.SourcePort == "80") && (showData == "tcp80" || showData == string.Empty))
                    {
                        Console.WriteLine("[{0}] {1}:{2} -> {3}:{4} | {5} - {6}", DateTime.Now, ipHeader.SourceAddress, tcpHeader.SourcePort, ipHeader.DestinationAddress, tcpHeader.DestinationPort, "IP > TCP > HTTP", tcpHeader.Flags);
                        output = System.Text.Encoding.Default.GetString(tcpHeader.Data);
                        Console.WriteLine(output.Trim());
                    }

                    //HTTPS
                    if ((tcpHeader.DestinationPort == "443" || tcpHeader.SourcePort == "443") && (showData == "tcp443" || showData == string.Empty))
                    {
                        Console.WriteLine("[{0}] {1}:{2} -> {3}:{4} | {5} - {6}", DateTime.Now, ipHeader.SourceAddress, tcpHeader.SourcePort, ipHeader.DestinationAddress, tcpHeader.DestinationPort, "IP > TCP > HTTPS", tcpHeader.Flags);
                        output = System.Text.Encoding.Default.GetString(tcpHeader.Data);
                        Console.WriteLine(output.Trim());
                    }

                    break;

                //UDP
                case Protocol.UDP:
                    UDP udpHeader = new UDP(ipHeader.Data, (int)ipHeader.MessageLength);

                    if (showData == "udp")
                    {
                        Console.WriteLine("[{0}] {1}:{2} -> {3}:{4} | {5}", DateTime.Now, ipHeader.SourceAddress, udpHeader.SourcePort, ipHeader.DestinationAddress, udpHeader.DestinationPort, "IP > UDP");
                    }

                    //DNS
                    if ((udpHeader.DestinationPort == "53" || udpHeader.SourcePort == "53") && (showData == "udp53" || showData == string.Empty))
                    {
                        Console.WriteLine("[{0}] {1}:{2} -> {3}:{4} | {5}", DateTime.Now, ipHeader.SourceAddress, udpHeader.SourcePort, ipHeader.DestinationAddress, udpHeader.DestinationPort, "IP > UDP > DNS");
                        DNS dnsHeader = new DNS(udpHeader.Data, Convert.ToInt32(udpHeader.Length) - 8);
                        Console.WriteLine("Identification: {0}\r\nFlags: {1}\r\nQuestions: {2}\r\nAnswer RRs: {3}\r\nAuthority RRs: {4}\r\nAdditional RRs: {5}", dnsHeader.Identification, dnsHeader.Flags, dnsHeader.TotalQuestions, dnsHeader.TotalAnswerRRs, dnsHeader.TotalAuthorityRRs, dnsHeader.TotalAdditionalRRs);

                        output = System.Text.Encoding.Default.GetString(udpHeader.Data);
                        Console.WriteLine(output.Trim());
                    }

                    break;

                //ICMP
                case Protocol.ICMP:
                    if (showData == "icmp")
                    {
                        Console.WriteLine("[{0}] {1} -> {2} | {3}", DateTime.Now, ipHeader.SourceAddress, ipHeader.DestinationAddress, "IP > ICMP");
                    }

                    if (showData == string.Empty)
                    {
                        Icmp icmp = new Icmp(ipHeader.Data, (int)ipHeader.MessageLength);

                        Console.WriteLine("[{0}] {1} -> {2} | {3}", DateTime.Now, ipHeader.SourceAddress, ipHeader.DestinationAddress, "IP > ICMP");
                        Console.WriteLine("AddressMask: {0}\r\nChecksum: {1}\r\nCode: {2}\r\nIdentifier: {3}\r\nType: {4}", icmp.AddressMask, icmp.Checksum, icmp.Code, icmp.Identifier, icmp.GetType(icmp.Type));
                    }

                    break;

                //IGMP
                case Protocol.IGMP:
                    if (showData == "igmp")
                    {
                        Console.WriteLine("[{0}] {1} -> {2} | {3}", DateTime.Now, ipHeader.SourceAddress, ipHeader.DestinationAddress, "IP > IGMP");
                    }

                    if (showData == string.Empty)
                    {
                        Igmp igmp = new Igmp(ipHeader.Data, (int)ipHeader.MessageLength);

                        Console.WriteLine("[{0}] {1} -> {2} | {3}", DateTime.Now, ipHeader.SourceAddress, ipHeader.DestinationAddress, "IP > IGMP");
                        Console.WriteLine("Checksum: {0}\r\nGropeAddress: {1}\r\nMaxResponseTime: {2}\r\nType: {3}", igmp.Checksum, igmp.GropeAddress, igmp.MaxResponseTime, igmp.Type);
                    }

                    break;
                case Protocol.Unknown:
                    break;
            }
        }
    }
}