using System;

namespace App.Domains.Network.Impl.Messages
{
    [Serializable]
    public class ChatMessageResponse : Response
    {
        public int Id;
        public string NickName;
        public string Message;
        public string Color;
    }
}