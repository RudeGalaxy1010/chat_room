using App.Domains.Network.Impl.Messages;
using R3;

namespace App.Domains.Network.Api
{
    public interface INetworkService
    {
        Observable<MessageWrapper> MessageQueue { get; }
        ReadOnlyReactiveProperty<bool> IsConnected { get; }

        void Connect(string address, int port);
        void Disconnect();
        void SendMessage(MessageWrapper message);
    }
}