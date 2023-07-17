using Nostr.Client.Client;
using Nostr.Client.Responses;
using NostrBotSharp.Wrapper;
using Nostr.Client.Messages;
using Nostr.Client.Messages.Metadata;

namespace NostrBotSharp
{
    class Program
    {
        static NostrSender sender;

        static NostrEventMap noteMap = 
            new NostrEventMap(NostrKind.ShortTextNote, 1000);

        static NostrEventMap reactionMap = 
            new NostrEventMap(NostrKind.Reaction);

        static NostrEventMap contactsMap =
            new NostrEventMap(NostrKind.Contacts);

        static NostrEventMap metadataMap =
            new NostrEventMap(NostrKind.Metadata);

        static bool textSend = false;

        /// <summary>
        /// Callback when subscribed.
        /// </summary>
        /// <param name="response"></param>
        public static void Subscribe(NostrEventResponse response)
        {
            var ev = response.Event;
 
            switch (ev.Kind)
            {
                case NostrKind.ShortTextNote:
                    noteMap.Push(ev.Id, response);
                    break;
                case NostrKind.Metadata:
                    metadataMap.Push(ev.Pubkey, response);
                    if (!textSend)
                    {
                        sender.RequestShortTextNote(-1, 1, null, 10);
                        textSend = true;
                    }
                    break;
                case NostrKind.Reaction:
                    reactionMap.Push(ev.Pubkey, response);
                    break;
                case NostrKind.Contacts:
                    var authors = NostrEventAnalyzer.GetAuthors(ev);
                    if (authors.Count > 0 && !contactsMap.ContainsKey(ev.Pubkey))
                    {
                        sender.RequestMetadata(authors.ToArray());
                    }
                    contactsMap.Push(ev.Pubkey, response);

                    break;
            }
        }

        public static void ViewTimeline()
        {
            while (true)
            {
                if (noteMap.Count() == 0)
                {
                    continue;
                }

                foreach (var item in noteMap.SortByTime())
                {
                    var ev = item.Value.Event;

                    string name = "";
                    string displayName = ev.Pubkey;
                    if (metadataMap.ContainsKey(ev.Pubkey))
                    {
                        displayName =
                            NostrEventAnalyzer.Metadata.GetDisplayName(
                                (NostrMetadataEvent)metadataMap.Get(ev.Pubkey).Event);
                        name =
                            NostrEventAnalyzer.Metadata.GetName(
                                (NostrMetadataEvent)metadataMap.Get(ev.Pubkey).Event);
                    }

                    // Subscribe callback.
                    Console.WriteLine("---------------------------------");
                    Console.WriteLine("note id      :" + ev.Id);
                    Console.WriteLine("display_name :" + displayName);
                    Console.WriteLine("name         :" + name);
                    Console.WriteLine("content :\r\n" + ev.Content);

                    noteMap.Remove(item.Key);
                    Thread.Sleep(100);
                }

                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Entry point.
        /// </summary>
        public static void Main()
        {
            var relays = new[]
            {
                //new Uri("wss://relay.damus.io"),
                new Uri("wss://nostr-relay.nokotaro.com"),
                new Uri("wss://universe.nostrich.land/?lang=ja&lang=en"),
                //new Uri("wss://nostr.h3z.jp"),
                //new Uri("wss://relay.nostrich.land"),
                new Uri("wss://relay-jp.nostr.wirednet.jp"),
            };

            // Connect relay.
            var connector = new NostrConnector(NostrLogger<NostrWebsocketClient>.Instance, relays);
            var client = connector.Connect();
            connector.Communicate();

            // Subscribe.
            var subscriber = new NostrSubscriber(client, Subscribe);
            subscriber.Subscribe();

            sender = new NostrSender(client);

            // TODO: pubkey : hakkadaikon
            sender.RequestContacts("101b30ee88c27a13de68bf7c8c06368ea3e3e837641595c18675677d18a46a45");

            ViewTimeline();

            // Post to relay.
            var myPrivateKeyHex = "";
            sender.Post(myPrivateKeyHex, "Test");
        }
    }
}