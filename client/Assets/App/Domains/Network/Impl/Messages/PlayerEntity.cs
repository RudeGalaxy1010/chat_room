using System;

namespace App.Domains.Network.Impl.Messages
{
    [Serializable]
    public class PlayerEntity : Entity
    {
        public float X;
        public float Y;
        public string Name;
        public string Color;
    }
}
