using System;

namespace App.Domains.Network.Impl.Messages
{
    [Serializable]
    public class ConnectRequest : Request
    {
        public string NickName;
    }
}