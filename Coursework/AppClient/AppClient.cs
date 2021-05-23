using System.Net.Sockets;
using System.Net;
using System.Text;
using System;

namespace AppClient
{
    class AppClient
    {
        private  const int Port = 8080;
        private static bool _isWorking = true;
        public static void Main()
        {
            var bytes = new byte[4096];

            try
            {
                var ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Port);

                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ipPoint);
				
				
                while(_isWorking){
                    Console.WriteLine("1-Find Word\n2-Exit");
                    var n = int.Parse(Console.ReadLine());
                    switch(n){
                        case 1:
                            Console.WriteLine("input request");
                            StringBuilder decodedUserMessage = new StringBuilder();
						
                            var messages = Encoding.ASCII.GetBytes(Console.ReadLine());
                            socket.Send(messages);
                            do
                            {
                                var byteRecv = socket.Receive(bytes);
                                decodedUserMessage.Append(Encoding.ASCII.GetString(bytes, 
                                    0, byteRecv));
                            }
                            while (socket.Available>0);
                            Console.WriteLine(decodedUserMessage);
                            break;
                        case 2:
                            _isWorking = false;
                            break;
                    }
					
                }

                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
				
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.ReadKey();
			
        }
    }
}