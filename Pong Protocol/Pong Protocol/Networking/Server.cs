using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters;

using Pong_Protocol.Sprites;

namespace Pong_Protocol.Networking {
    public class Server {
        public TcpListener listener;
        public TcpClient client;
        private PaddleInfo send = null;
        Thread t;

        public Server(){
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 1337);
            listener.Start();
            t = new Thread(new ThreadStart(Listen));
            t.Start();
        }
        public void Listen() {
            try {
                client = listener.AcceptTcpClient();
            } catch (SocketException e) {
                Console.WriteLine("Client exited forcibly: {0}", e); //LOG
                return;
            }
            BinaryFormatter binf = new BinaryFormatter();
            while (client.Connected) {
                if (send != null) {
                    try {
                        binf.Serialize(client.GetStream(), send);
                    } catch (Exception) {
                        Console.WriteLine("jaaaj!"); //LOG
                    }
                }
            }
        }
        public void Send(PaddleInfo info){
            send = info;
        }
        public void Close() {
            listener.Stop();
            t.Join();
        }
    }
}
