using System;
using System.Collections.Generic;
using App.Domains.Network.Api;
using App.Domains.Network.Impl.Messages;
using R3;
using UnityEngine;

namespace App.Domains.Network.Impl
{
    public class MessageService : IMessageService, IDisposable
    {
        private readonly INetworkService _networkService;
        private readonly IMessageEncoder _messageEncoder;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private readonly Dictionary<MessageType, List<Action<Response>>> _handlers =
            new Dictionary<MessageType, List<Action<Response>>>();

        public MessageService(INetworkService networkService, IMessageEncoder messageEncoder)
        {
            _networkService = networkService;
            _messageEncoder = messageEncoder;

            _networkService.MessageQueue
                .Subscribe(OnMessageReceived)
                .AddTo(_disposables);
        }

        public void Dispose() =>
            _disposables.Dispose();

        public void Register(MessageType type, Action<Response> handler)
        {
            if (!_handlers.ContainsKey(type))
            {
                _handlers[type] = new List<Action<Response>>();
            }

            if (!_handlers[type].Contains(handler))
            {
                _handlers[type].Add(handler);
            }
        }

        public void Unregister(MessageType type, Action<Response> handler)
        {
            if (_handlers.TryGetValue(type, out List<Action<Response>> handlers))
            {
                handlers.Remove(handler);
            }
        }

        public void Send(Request message)
        {
            if (!_networkService.IsConnected.CurrentValue)
            {
                Debug.LogError("Network service is not connected");
                return;
            }

            MessageType? messageType = GetMessageType(message);

            if (!messageType.HasValue)
            {
                Debug.LogError("Unsupported request type: " + message.GetType().Name);
                return;
            }

            string data = _messageEncoder.Encode(message);

            _networkService.SendMessage(new MessageWrapper
            {
                Type = (int)messageType.Value,
                Data = data,
            });
        }

        private void OnMessageReceived(MessageWrapper message)
        {
            MessageType messageType = GetMessageType(message);
            Response response;

            try
            {
                switch (messageType)
                {
                    case MessageType.Connect:
                        response = _messageEncoder.Decode<ConnectResponse>(message.Data);
                        break;
                    case MessageType.Join:
                        response = _messageEncoder.Decode<JoinResponse>(message.Data);
                        break;
                    case MessageType.Leave:
                        response = _messageEncoder.Decode<LeaveResponse>(message.Data);
                        break;
                    case MessageType.UpdatePosition:
                        response = _messageEncoder.Decode<UpdatePositionResponse>(message.Data);
                        break;
                    case MessageType.ChatMessage:
                        response = _messageEncoder.Decode<ChatMessageResponse>(message.Data);
                        break;
                    default:
                        Debug.LogError("Unknown message type: " + messageType);
                        return;
                }
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed to decode message type {message.Type}: {exception}");
                return;
            }

            if (_handlers.TryGetValue(messageType, out List<Action<Response>> handlers))
            {
                // Taking a snapshot in case of list modified during Invoke()
                List<Action<Response>> handlersCopy = new List<Action<Response>>(handlers);

                for (int i = 0; i < handlersCopy.Count; i++)
                {
                    try
                    {
                        handlersCopy[i].Invoke(response);
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError($"Message handler failed for type {messageType}: {exception}");
                    }
                }
            }
        }

        private MessageType? GetMessageType(Request message) => message.GetType().Name switch
        {
            nameof(ConnectRequest) => MessageType.Connect,
            nameof(ReadyRequest) => MessageType.Ready,
            nameof(UpdatePositionRequest) => MessageType.UpdatePosition,
            nameof(ChatMessageRequest) => MessageType.ChatMessage,
            _ => null,
        };

        private MessageType GetMessageType(MessageWrapper message) =>
            (MessageType)message.Type;
    }
}