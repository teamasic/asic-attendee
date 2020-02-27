using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Services.NetworkService
{
    public class Receiver
    {
        protected UdpClient localServer = null;

        public Receiver(UdpClient localServer)
        {
            this.localServer = localServer;
        }

        protected  object Receive(ref IPEndPoint remoteHostIP)
        {
            return localServer.Receive(ref remoteHostIP);
        }

        protected async Task<UdpReceiveResult> ReceiveAsync()
        {
            return await localServer.ReceiveAsync();
        }

        protected  object Decode(object encodedMessage) {
            return encodedMessage;
        }
        protected  bool Authenticate(object decodedMessage)
        {
            return true;
        }

        public object Start(ref IPEndPoint remoteHostIP)
        {
            var encodedMessage = Encoding.UTF8.GetString(Receive(ref remoteHostIP) as byte[]);
            var decodedMessage = Decode(encodedMessage);
            if (Authenticate(decodedMessage))
            {
                return decodedMessage;
            }
            return null;
        }

        //public async Task<object> StartAsync()
        //{
        //    var udpReceiveResult = await ReceiveAsync();

        //    var encodedMessage = Encoding.UTF8.GetString(Receive(ref remoteHostIP) as byte[]);
        //    var decodedMessage = Decode(encodedMessage);
        //    if (Authenticate(decodedMessage))
        //    {
        //        return decodedMessage;
        //    }
        //    return null;
        //}

    }
}
