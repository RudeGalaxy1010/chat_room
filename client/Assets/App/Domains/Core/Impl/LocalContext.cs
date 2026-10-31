using System;
using App.Domains.Core.Api;
using App.Domains.Network.Api;
using App.Domains.Network.Impl.Messages;

namespace App.Domains.Core.Impl
{
    public class LocalContext : ILocalContext, IDisposable
    {
        private readonly IMessageService _messageService;

        public int Id { get; private set; } = -1;

        public string Name { get; private set; } = string.Empty;

        public LocalContext(IMessageService messageService)
        {
            _messageService = messageService;
            _messageService.Register(MessageType.Connect, OnConnect);
        }

        public void Dispose()
        {
            _messageService.Unregister(MessageType.Connect, OnConnect);
            Id = -1;
            Name = string.Empty;
        }

        private void OnConnect(Response response)
        {
            ConnectResponse connectResponse = (ConnectResponse)response;

            if (connectResponse.IsOk)
            {
                Id = connectResponse.Id;
                Name = connectResponse.Name;
            }
        }
    }
}