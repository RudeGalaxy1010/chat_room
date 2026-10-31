using System;

namespace App.Domains.Network.Impl.Messages
{
    [Serializable]
    public abstract class Response
    {
        public int Status;

        public bool IsOk => Status == (int)MessageStatus.Ok;
    }
}
