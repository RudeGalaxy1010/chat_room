using System;

namespace App.Domains.Network.Impl.Messages
{
    [Serializable]
    public class MessageWrapper
    {
        public int Type;
        public string Data;
    }
}
