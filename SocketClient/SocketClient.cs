using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SocketClient
{
    internal class SocketClient
    {
        public SocketClient()
        {
            while (true) StartClient();
        }

        internal void StartClient()
        {
            Console.Write("Message: ");
            string msg = Console.ReadLine() + " <EOM>";
            IPAddress iPServerPAddress = IPAddress.Parse("192.168.2.2");
            IPEndPoint serverEndPoint = new(iPServerPAddress, 22222);

            Socket sender = new(
                iPServerPAddress.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            sender.Connect(serverEndPoint);
            Console.WriteLine("Connected to: " + serverEndPoint.ToString());

            byte[] byteArr = Encoding.ASCII.GetBytes(msg);
            sender.Send(byteArr);

            string returnMsg = GetMessage(sender);
            Console.WriteLine(returnMsg);

            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }
        private static string GetMessage(Socket socket)
        {
            string data = null;
            byte[] bytes = null;

            while (true)
            {
                bytes = new byte[4096];
                int bytesRec = socket.Receive(bytes);
                data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                if (data.Contains("<EOM>")) break;
            }

            return data;
        }
    }
}
