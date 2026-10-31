using System;

namespace App.Domains.Network.Impl.Messages
{
    [Serializable]
    public class UpdatePositionRequest : Request
    {
        public int Id;
        public float X;
        public float Y;
    }
}