using System;
using System.Text.Json;
using System.Threading.Tasks;
using SocketIO.Serializer.SystemTextJson;
using SocketIOClient;
using SocketIOClient.Windows7;

namespace ws_deadlock
{
    public class SioClient : IDisposable
    {
        private readonly SocketIOClient.SocketIO _client;

        public SioClient(string apiUrl)
        {
            _client = new SocketIOClient.SocketIO(apiUrl);
            InitializeSocketClient();
        }

        private void InitializeSocketClient()
        {
            _client.ClientWebSocketProvider = () => new ClientWebSocketManaged();

            _client.Options.ConnectionTimeout = TimeSpan.FromSeconds(10);
            _client.Options.ReconnectionAttempts = 30;
            _client.Options.ReconnectionDelay = 2500;
            _client.Options.RandomizationFactor = 0;

            _client.Options.Reconnection = true;

            // client.Options.AutoUpgrade = false;
            // client.Options.Transport = TransportProtocol.Polling;

            // Make serialization case insensitive (responses are camelCase, in .NET we prefer PascalCase)
            _client.Serializer = new SystemTextJsonSerializer(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Event handlers
            _client.OnConnected += OnConnect;
            _client.OnDisconnected += OnDisconnect;
            _client.OnError += OnError;

            _client.OnReconnectAttempt += OnReconnectAttempt;
            _client.OnReconnectFailed += OnReconnectFailed;
            _client.OnReconnected += OnSioReconnected;
        }

        public void On(string eventName, Func<SocketIOResponse, Task> callback)
        {
            _client.On(eventName, callback);
        }

        public void On(string eventName, Action<SocketIOResponse> callback)
        {
            _client.On(eventName, callback);
        }

        public void Off(string eventName)
        {
            _client.Off(eventName);
        }

        public async Task EmitAsync(string eventName, object requestData)
        {
            if (!_client.Connected)
            {
                await InitializeConnection();
            }

            try
            {
                await _client.EmitAsync(eventName, requestData);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        public async Task InitializeConnection()
        {
            Console.WriteLine("Connecting to SocketIO");

            try
            {
                await _client.ConnectAsync();
            }
            catch (ConnectionException exception)
            {
                Console.WriteLine($"Error when connecting to SocketIO! {exception}");

                throw;
            }
        }

        public async Task DisconnectAsync()
        {
            Console.WriteLine("Disconnecting from SocketIO");

            if (_client == null)
            {
                return;
            }

            try
            {
                await _client.DisconnectAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error when disconnecting from SocketIO {e}");
                throw;
            }
        }

        private void OnSioReconnected(object sender, int attempt)
        {
            Console.WriteLine("Reconnected to SocketIO");
        }

        private void OnReconnectFailed(object sender, EventArgs args)
        {
            Console.WriteLine("Reconnecting to SocketIO failed");
        }

        private void OnError(object sender, string error)
        {
            Console.WriteLine($"SocketIO SIO error: {error}");
        }

        private void OnConnect(object sender, EventArgs args)
        {
            Console.WriteLine("Connected to SocketIO");
        }

        private void OnDisconnect(object sender, string reason)
        {
            Console.WriteLine($"Disconnected from SocketIO: {reason}");
        }

        private void OnReconnectAttempt(object sender, int attempt)
        {
            Console.WriteLine($"Reconnecting to SocketIO... (attempt: {attempt})");
        }

        public void Dispose()
        {
            if (_client == null)
            {
                return;
            }

            _client.OnConnected -= OnConnect;
            _client.OnDisconnected -= OnDisconnect;
            _client.OnError -= OnError;

            _client.OnReconnectAttempt -= OnReconnectAttempt;
            _client.OnReconnectFailed -= OnReconnectFailed;
            _client.OnReconnected -= OnSioReconnected;

            _client.Dispose();
        }
    }
}