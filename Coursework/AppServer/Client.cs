using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AppServer
{
    public class Client
    {
        private Socket _clientSocket;
        private string _serverResponse;

        public void StartClient(Socket inClientSocket)
        {
            _clientSocket = inClientSocket;
            var ctThread = new Thread(StartCommunication);
            ctThread.Start();
        }
        private void StartCommunication()
        {
            var messageReceived = new byte[4096];

            while ((true))
            {
                try
                {
                    StringBuilder decodedUserMessage = new StringBuilder();

                    do
                    {
                        var byteRecv = _clientSocket.Receive(messageReceived);
                        decodedUserMessage.Append(Encoding.ASCII.GetString(messageReceived, 
                            0, byteRecv));
                    }
                    while (_clientSocket.Available>0);
					

                    _serverResponse = FormAnswer( decodedUserMessage.ToString());

                    var message = Encoding.ASCII.GetBytes(_serverResponse);
                    if (message.Equals("StopClientConnection"))
                    {
                        _clientSocket.Shutdown(SocketShutdown.Both);
                        _clientSocket.Close();
                        break;
                    }
                    _clientSocket.Send(message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        private static string FormAnswer(string dataFromClient) {
            var response = dataFromClient + ": ";
            try
            {
                var documentsIdList = AppServer.InvertedIndex[dataFromClient];
                for(var i = 0;i<documentsIdList.Count;i++)
                {
                    var documentsIdSplitted = documentsIdList[i].Split('\\');
                    response+=documentsIdSplitted[documentsIdSplitted.Length-1]+'\n';
                }
            }
            catch (Exception e)
            {
                response += "No Matches";
                Console.WriteLine(e);
            }

            return response;
        }
    }
}