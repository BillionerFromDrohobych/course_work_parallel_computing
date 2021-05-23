using System;
using System.Net;
using System.Net.Sockets;


namespace AppServer
{
	
	public static class ServerConnection{
	
	private static readonly IPAddress LocalAddr = IPAddress.Parse("127.0.0.1");
	private const int Port = 8080;
		public static void CreateServer(){
			try{
				
				var server = new TcpListener(LocalAddr, Port);

				server.Start();
				Console.WriteLine("Server Started");
				while (true)
				{
					var clientSocket = server.AcceptSocket();
					var client = new Client();
					client.StartClient(clientSocket);
				}
			}catch (Exception e){
				Console.WriteLine(e.ToString());
			}
		}
	}
}
