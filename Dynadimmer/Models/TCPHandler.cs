using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Dynadimmer.Models
{
    class TCPHandler : MyUIHandler
    {
        public TCPHandler()
        {
            TcpClient tcp = new TcpClient();
            tcp.Client.Connect("192.168.11.71", 3002);
        }

        public void initTCP()
        {
            TcpClient tcp = new TcpClient();
            tcp.Client.Connect("192.168.11.71", 3002);
            tcp.Client.Close();
        }
    }
}
