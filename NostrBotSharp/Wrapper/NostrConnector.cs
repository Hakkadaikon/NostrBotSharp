using Microsoft.Extensions.Logging;
using Nostr.Client.Client;
using Nostr.Client.Communicator;
using System.Net.WebSockets;

namespace NostrBotSharp.Wrapper
{
    /// <summary>
    /// Class for connecting with Nostr relay.
    /// </summary>
    public class NostrConnector
    {
        private List<NostrWebsocketCommunicator> communicators;
        private readonly ILogger<NostrWebsocketClient> logger;
        private readonly Uri[] relays;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger">Nostr logger.</param>
        /// <param name="relays">Nostr relay uris.</param>
        public NostrConnector(ILogger<NostrWebsocketClient> logger, Uri[] relays)
        {
            this.logger = logger;
            this.relays = relays;
        }

        /// <summary>
        /// Return communicator list from Nostr relay URI
        /// </summary>
        /// <param name="relays">Nostr relay uris.</param>
        /// <returns>Nostr webSocket communicator list.</returns>
        private List<NostrWebsocketCommunicator> CreateCommunicators(Uri[] relays)
        {
            var communicators = new List<NostrWebsocketCommunicator>();
            relays.ToList().ForEach(relay => communicators.Add(CreateCommunicator(relay)));
            return communicators;
        }

        /// <summary>
        /// Set communicator parameters.
        /// </summary>
        /// <param name="comm">Nostr websocket communicator.</param>
        /// <param name="uri">Nostr relay uri.</param>
        private void SetCommunicatorParam(NostrWebsocketCommunicator comm, Uri uri)
        {
            comm.Name = uri.Host;
            comm.ReconnectTimeout = TimeSpan.FromSeconds(30);
            comm.ErrorReconnectTimeout = TimeSpan.FromSeconds(60);

            comm.ReconnectionHappened.Subscribe(
                (info) =>
                {
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine(
                        "[{0}] Reconnection happened, type: {1}",
                        comm.Name,
                        info.Type);
                    Console.ResetColor();
                });

            comm.DisconnectionHappened.Subscribe(
                (info) =>
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(
                        "[{0}] Disconnection happened, Type: [{1}], Ex: [{2}] innerEx [{3}]",
                        comm.Name,
                        info.Type.ToString(),
                        info.Exception != null ?
                            info.Exception.Message :
                            "",
                        info.Exception != null && info.Exception.InnerException != null ?
                            info.Exception.InnerException.Message :
                            "");
                    Console.ResetColor();
                });
        }

        /// <summary>
        /// Return communicator from Nostr relay URI
        /// </summary>
        /// <param name="uri">Nostr relay uri.</param>
        private NostrWebsocketCommunicator CreateCommunicator(Uri uri)
        {
            var comm = new NostrWebsocketCommunicator(uri, () =>
            {
                var client = new ClientWebSocket();
                client.Options.SetRequestHeader("Origin", "http://localhost");
                return client;
            });

            SetCommunicatorParam(comm, uri);

            return comm;
        }

        /// <summary>
        /// Connect with Nostr relay.
        /// </summary>
        /// <returns></returns>
        public INostrClient Connect()
        {
            this.communicators = CreateCommunicators(this.relays);
            return new NostrMultiWebsocketClient(
                this.logger,
                this.communicators.ToArray());
        }

        /// <summary>
        /// Communication with Nostr relay.
        /// </summary>
        public void Communicate()
        {
            this.communicators.ForEach(x => x.Start());
        }

        /// <summary>
        /// Disconnect Nostr relay.
        /// </summary>
        public async void Disconnect()
        {
            foreach (var communicator in this.communicators)
            {
                await communicator.Stop(WebSocketCloseStatus.NormalClosure, string.Empty);
                await Task.Delay(500);
                communicator.Dispose();
            }
        }
    }
}
