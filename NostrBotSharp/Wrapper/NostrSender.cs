using Nostr.Client.Client;
using Nostr.Client.Keys;
using Nostr.Client.Messages;
using Nostr.Client.Messages.Mutable;
using Nostr.Client.Requests;

namespace NostrBotSharp.Wrapper
{
    public class NostrSender
    {
        private readonly INostrClient client;

        public NostrSender(INostrClient client)
        {
            this.client = client;
        }

        public void RequestMetadata(string[] authors)
        {
            this.client.Send(
                new NostrRequest("timeline:pubkey:follows",
                new NostrFilter
                {
                    Authors = (authors.Length == 0) ? null : authors,
                    Kinds = new[]
                    {
                        NostrKind.Metadata
                    },
                }));
        }

        public void RequestContacts(string pubkey)
        {
            this.client.Send(
                new NostrRequest("timeline:pubkey:follows",
                new NostrFilter
                {
                    Authors = new[]
                    {
                        pubkey
                    },
                    Kinds = new[]
                    {
                        NostrKind.Contacts
                    },
                    Limit = 1
                }));
        }

        public void RequestShortTextNote(double begin, double end, string[]? authors, int limit)
        {
            this.client.Send(
                new NostrRequest("timeline:pubkey:follows",
                new NostrFilter
                {
                    Authors = (authors?.Length == 0) ? null : authors,
                    Kinds = new[]
                    {
                        NostrKind.ShortTextNote,
                    },
                    Since = DateTime.UtcNow.AddHours(begin),
                    Until = DateTime.UtcNow.AddHours(end),
                    Limit = limit
                }));
        }

        public void Reaction(
            string myPrivateKeyHex, string targetUserPubkey, string targetNoteId, string content)
        {
            var tags = new NostrEventTagsMutable
            {
                new NostrEventTag(NostrEventTag.ProfileIdentifier, targetUserPubkey),
                new NostrEventTag(NostrEventTag.EventIdentifier, targetNoteId)
            };

            var ev = new NostrEvent
            {
                Kind = NostrKind.Reaction,
                CreatedAt = DateTime.UtcNow,
                Content = content,
            };
            
            var key = NostrPrivateKey.FromBech32(myPrivateKeyHex);
            var signed = ev.Sign(key);
            
            client.Send(new NostrEventRequest(signed));
        }

        public void Reply(
            string myPrivateKeyHex, string targetUserPubkey, string targetNoteId, string content)
        {
            var tags = new NostrEventTagsMutable
            {
                new NostrEventTag(NostrEventTag.ProfileIdentifier, targetUserPubkey),
                new NostrEventTag(NostrEventTag.EventIdentifier, targetNoteId)
            };

            var ev = new NostrEvent
            {
                Kind = NostrKind.ShortTextNote,
                CreatedAt = DateTime.UtcNow,
                Content = content,
            };
            
            var key = NostrPrivateKey.FromBech32(myPrivateKeyHex);
            var signed = ev.Sign(key);
            
            client.Send(new NostrEventRequest(signed));
        }

        public void Post(string myPrivatekeyHex, string content)
        {
            var ev = new NostrEvent
            {
                Kind = NostrKind.ShortTextNote,
                CreatedAt = DateTime.UtcNow,
                Content = content
            };
            
            var key = NostrPrivateKey.FromBech32(myPrivatekeyHex);
            var signed = ev.Sign(key);
            
            client.Send(new NostrEventRequest(signed));
        }
    }
}
