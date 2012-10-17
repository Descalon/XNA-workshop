using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters;
using Pong_Protocol.Sprites;

namespace Pong_Protocol.Networking {
    class Client {
        public TcpClient clientSocket;
        public Thread t;
        public PaddleInfo recieve;
        public ClientConnectEventHandler onClientConnect;

        protected void OnConnect(EventArgs e) {
            if (onClientConnect != null) {
                onClientConnect(this, e);
            }
        }
       
        public static Client Connect(string ipAddress, int socket = 1337) {
            Client c = new Client();
            try {
                c.clientSocket = new TcpClient(ipAddress, socket);
                c.t = new Thread(new ThreadStart(c.ListenForCommands));
                c.t.Start();
            } catch (SocketException) {
                Console.WriteLine("No connection could be made!"); //LOG
            } catch (Exception e) {
                Console.WriteLine("Why is this going wrong: {0}", e); //LOG
            }
            return c;
        }
        public static Client SetClient(TcpClient clientSocket){
            Client c = new Client();
            c.clientSocket = clientSocket;

            return c;
        }

        public void ListenForCommands() {
            if (clientSocket.Connected) {
                Console.WriteLine("We're connected :D!"); //LOG
                OnConnect(EventArgs.Empty);
            }
            BinaryFormatter bf = new BinaryFormatter();
            while (clientSocket.Connected) {
                try {
                    recieve = (PaddleInfo)bf.Deserialize(clientSocket.GetStream());
                } catch (ObjectDisposedException) {
                    Console.WriteLine("Client stream is closed."); //LOG
                    return;
                }
            }
        }

        public void CloseSocket() {
            clientSocket.Close();
            t.Join();
        }
    }
}
