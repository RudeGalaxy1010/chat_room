using System;

namespace App.Domains.Network.Impl.Messages
{
    [Serializable]
    public class JoinResponse : Response
    {
        public PlayerEntity Player;
    }
}