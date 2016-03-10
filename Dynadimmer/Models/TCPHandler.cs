using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Dynadimmer.Models
{
    class TCPHandler : MyUIHandler
    {
        private IPAddress ip;
        private TcpClient tcpclient;

        public TCPHandler()
        {
            try
            {
                this.ip = IPAddress.Parse("192.168.4.1");
                tcpclient = new TcpClient();
                tcpclient.Client.Connect(ip, 23);
            }

            catch
            {
                
            }
          
        }

        public void initTCP()
        {
         
            //TcpListener tcplistener = new TcpListener(ip, 25);
            //tcplistener.Start();
            Stream stm = tcpclient.GetStream();
            string str = "Time: " + DateTime.Now + " ";
            
            ASCIIEncoding asen = new ASCIIEncoding();
            byte[] ba = asen.GetBytes(str);
            stm.Write(ba, 0, ba.Length);
            stm.Close();
            tcpclient.Client.Close();
        }
    }
}
