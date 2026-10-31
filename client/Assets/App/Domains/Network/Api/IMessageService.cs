using System;
using App.Domains.Network.Impl.Messages;

namespace App.Domains.Network.Api
{
    public interface IMessageService
    {
        void Register(MessageType type, Action<Response> handler);
        void Unregister(MessageType type, Action<Response> handler);
        void Send(Request message);
    }
}