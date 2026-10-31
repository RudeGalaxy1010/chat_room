using System;

namespace App.Domains.Network.Impl.Messages
{
    [Serializable]
    public class ConnectResponse : Response
    {
        public int Id;
        public string Name;
    }
}