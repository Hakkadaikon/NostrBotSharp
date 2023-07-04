using Microsoft.Extensions.Logging.Abstractions;
using Nostr.Client.Client;
using Nostr.Client.Responses;
using NostrBotSharp.Wrapper;

namespace NostrBotSharp
{
    class Program
    {
        public static void Subscribe(NostrEventResponse response)
        {
            // Subscribe callback.
        }

        public static void Main(String[] Args)
        {
            var relays = new[]
            {
                new Uri("wss://relay.damus.io"),
                new Uri("wss://nostr-relay.nokotaro.com"),
                new Uri("wss://universe.nostrich.land/?lang=ja&lang=en"),
                new Uri("wss://nostr.h3z.jp"),
                new Uri("wss://relay.nostrich.land"),
                new Uri("wss://relay-jp.nostr.wirednet.jp"),
            };

            // Connect relay.
            var connector = new NostrConnector(NostrLogger<NostrWebsocketClient>.Instance, relays);
            var client = connector.Connect();
            connector.Communicate();

            // Subscribe.
            var subscriber = new NostrSubscriber(client, Subscribe);
            subscriber.Subscribe();

            var sender = new NostrSender(client);

            // Post to relay.
            var myPrivateKeyHex = "";
            sender.Post(myPrivateKeyHex, "Test");
        }
    }
}