using System;
using System.Text;
using App.Domains.Core.Api;
using App.Domains.Network.Api;
using App.Domains.Network.Impl.Messages;
using Cysharp.Threading.Tasks;
using NativeWebSocket;
using R3;

namespace App.Domains.Network.Impl
{
    public class NetworkService : INetworkService, IDisposable
    {
        private const char SplitSymbol = '-';

        private readonly Subject<MessageWrapper> _messageQueue;
        private readonly ReactiveProperty<bool> _isConnected;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly ILogger _logger;

        private WebSocket? _webSocket;

        public Observable<MessageWrapper> MessageQueue => _messageQueue;
        public ReadOnlyReactiveProperty<bool> IsConnected => _isConnected;

        public NetworkService(ILogger logger)
        {
            _logger = logger;
            _messageQueue = new Subject<MessageWrapper>().AddTo(_disposables);
            _isConnected = new ReactiveProperty<bool>().AddTo(_disposables);
        }

        public void Dispose()
        {
            Disconnect();
            _disposables.Dispose();
        }

        public void Connect(string address, int port)
        {
            Disconnect();

            try
            {
                _webSocket = new WebSocket($"ws://{address}:{port}");
                AttachSocket(_webSocket);
                _webSocket
                    .Connect()
                    .AsUniTask()
                    .Forget(exception => _logger.LogError(exception.ToString()));
            }
            catch (Exception e)
            {
                Disconnect();
                _logger.LogError(e.ToString());
            }
        }

        public void Disconnect()
        {
            WebSocket? webSocket = _webSocket;

            if (webSocket == null)
            {
                return;
            }

            DetachSocket(webSocket);
            _webSocket = null;
            _isConnected.Value = false;

            try
            {
                switch (webSocket.State)
                {
                    case WebSocketState.Connecting:
                        webSocket.CancelConnection();
                        break;
                    case WebSocketState.Open:
                        webSocket.Close().AsUniTask().Forget(exception => _logger.LogError(exception.ToString()));
                        break;
                    case WebSocketState.Closing:
                    case WebSocketState.Closed:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
        }

        public void SendMessage(MessageWrapper message)
        {
            if (_webSocket?.State != WebSocketState.Open)
            {
                _logger.LogError("Web socket is not connected");
                return;
            }

            string text = $"{message.Type}{SplitSymbol}{message.Data}";
            _logger.Log($"Sending: {text}");

            _webSocket
                .SendText(text)
                .AsUniTask()
                .Forget(exception => _logger.LogError(exception.ToString()));
        }

        private void OnConnected() =>
            _isConnected.Value = true;

        private void OnDisconnected(WebSocketCloseCode closeCode)
        {
            if (_webSocket == null)
            {
                return;
            }

            DetachSocket(_webSocket);
            _webSocket = null;
            _isConnected.Value = false;
        }

        private void AttachSocket(WebSocket webSocket)
        {
            webSocket.OnOpen += OnConnected;
            webSocket.OnClose += OnDisconnected;
            webSocket.OnMessage += OnMessageReceived;
            webSocket.OnError += OnErrorReceived;
        }

        private void DetachSocket(WebSocket webSocket)
        {
            webSocket.OnOpen -= OnConnected;
            webSocket.OnClose -= OnDisconnected;
            webSocket.OnMessage -= OnMessageReceived;
            webSocket.OnError -= OnErrorReceived;
        }

        private void OnMessageReceived(byte[] bytes)
        {
            string message = Encoding.UTF8.GetString(bytes);
            _logger.Log($"Received: {message}");

            int separatorIndex = message.IndexOf(SplitSymbol);

            if (separatorIndex <= 0 || separatorIndex == message.Length - 1)
            {
                _logger.LogError("Invalid message format: " + message);
                return;
            }

            if (!int.TryParse(message.Substring(0, separatorIndex), out int type))
            {
                _logger.LogError("Invalid message type: " + message);
                return;
            }

            _messageQueue.OnNext(new MessageWrapper
            {
                Type = type,
                Data = message[(separatorIndex + 1)..],
            });
        }

        private void OnErrorReceived(string error) =>
            _logger.LogError(error);
    }
}
