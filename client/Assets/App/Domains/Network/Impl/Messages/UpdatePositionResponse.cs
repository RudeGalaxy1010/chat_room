using System;

namespace App.Domains.Network.Impl.Messages
{
    [Serializable]
    public class UpdatePositionResponse : Response
    {
        public int Id;
        public float X;
        public float Y;
    }
}