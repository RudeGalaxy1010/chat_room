using System;

namespace App.Domains.Network.Impl.Messages
{
    [Serializable]
    public class ChatMessageRequest : Request
    {
        public int Id;
        public string Message;
    }
}