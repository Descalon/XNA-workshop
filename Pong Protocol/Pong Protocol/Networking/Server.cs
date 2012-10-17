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
    public delegate void ClientConnectEventHandler(object sender, EventArgs e);

    public class Server {
        public event ClientConnectEventHandler onClientConnect;

        public TcpListener listener;
        public TcpClient client;
        private PaddleInfo send = null;
        Thread t;

        private Server(){
            
        }
        protected void OnConnect(EventArgs e) {
            if (onClientConnect != null) {
                onClientConnect(this, e);
            }
        }
        public static Server CreateSession(string ipAddress, int port = 1337) {
            Server s = new Server();
            s.listener = new TcpListener(IPAddress.Parse(ipAddress), port);
            s.listener.Start();
            s.t = new Thread(new ThreadStart(s.Listen));
            s.t.Start();
            return s;
        }
        public void Listen() {
            try {
                client = listener.AcceptTcpClient();
                OnConnect(EventArgs.Empty);
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
