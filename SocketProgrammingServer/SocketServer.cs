using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketProgrammingServer
{
    internal class SocketServer
    {
        public SocketServer() 
        {
            new MultiThreading().StartThreading();
           //StartServer();  
        }
        internal void StartServer()
        {
            IPHostEntry iPHostEntry = Dns.GetHostEntry(Dns.GetHostName(), System.Net.Sockets.AddressFamily.InterNetwork);
            int choice = ChooseIpAddress(iPHostEntry.AddressList);

            IPAddress iPAddress = iPHostEntry.AddressList[1];
            IPEndPoint iPEndPoint = new(iPAddress, 22222);
            Socket listener = new(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            listener.Bind(iPEndPoint);
            listener.Listen(10);
            Console.WriteLine("Server listens on: " + iPEndPoint);
            
            while(true) ConnectToClient(listener);
        }
        /// <summary>
        /// Accepts connection from client and recives message from it
        /// </summary>
        /// <param name="listener"></param>
        private static void ConnectToClient(Socket listener)
        {
            Socket handler = listener.Accept();
            Console.WriteLine("Connected to: " + handler.RemoteEndPoint.ToString());
            string data = GetMessage(handler);

            byte[] returnMsg = Encoding.ASCII.GetBytes("Server received msg <EOM>");
            handler.Send(returnMsg);
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();

            Console.WriteLine(data);
        }
        /// <summary>
        /// Receives message from client
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Select IP from ip address array
        /// </summary>
        /// <param name="addressList"></param>
        /// <returns>int</returns>
        private int ChooseIpAddress(IPAddress[] addressList)
        {
            var i = 0;
            foreach( var item in addressList )
            {
                Console.WriteLine($"[{i++}] {item}");
            }
          
            int j;
            do
            {
                Console.WriteLine("Input number of ip address: ");
            }
            while (!int.TryParse(Console.ReadLine(), out j) || j<0 || j>=addressList.Length);
            return j;
        }
    }
}
