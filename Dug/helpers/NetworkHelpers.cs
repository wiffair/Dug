using System.Net;
using System.Net.Sockets;

namespace DNS_Checker.helpers
{
    public class NetworkHelpers
    {
        public static int? GetFreeUdpPort()
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Bind(new IPEndPoint(IPAddress.Any, 0));
                return ((IPEndPoint?)socket.LocalEndPoint)?.Port;
            }
        }
    }
}
