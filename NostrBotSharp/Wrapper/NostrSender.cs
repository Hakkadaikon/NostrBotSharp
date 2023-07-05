using Nostr.Client.Client;
using Nostr.Client.Keys;
using Nostr.Client.Messages;
using Nostr.Client.Messages.Mutable;
using Nostr.Client.Requests;

namespace NostrBotSharp.Wrapper
{
    /// <summary>
    /// Class for sending requests to Nostr.
    /// </summary>
    public class NostrSender
    {
        private readonly INostrClient client;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="client">Nostr client interface</param>
        public NostrSender(INostrClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Request metadata to nostr relay.
        /// </summary>
        /// <param name="authors">Array of author name(pubkey).</param>
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

        /// <summary>
        /// Request contact data to nostr relay.
        /// </summary>
        /// <param name="pubkey">Public key of target poster to get followers.</param>
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

        /// <summary>
        /// Request note to nostr relay.
        /// </summary>
        /// <param name="begin">Begin hour.(relative time to current time)</param>
        /// <param name="end">End hour.(relative time to current time)</param>
        /// <param name="authors">Array of author name(pubkey).</param>
        /// <param name="limit">Limit number of notes.</param>
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

        /// <summary>
        /// Reaction to specified reaction.
        /// </summary>
        /// <param name="myPrivateKeyHex">Private key of the posting account.</param>
        /// <param name="targetUserPubkey">Public key of the Note to be reacted.</param>
        /// <param name="targetNoteId">Note id of the note to be reacted.</param>
        /// <param name="content">Content to reaction</param>
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

        /// <summary>
        /// Reply to specified post.
        /// </summary>
        /// <param name="myPrivateKeyHex">Private key of the posting account</param>
        /// <param name="targetUserPubkey">Public key of the Note to be posted.</param>
        /// <param name="targetNoteId">Note id of the note to be posted.</param>
        /// <param name="content">Content to reply</param>
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

        /// <summary>
        /// Post to nostr relay.
        /// </summary>
        /// <param name="myPrivateKeyHex">Private key of the posting account</param>
        /// <param name="content">Content to post</param>
        public void Post(string myPrivateKeyHex, string content)
        {
            var ev = new NostrEvent
            {
                Kind = NostrKind.ShortTextNote,
                CreatedAt = DateTime.UtcNow,
                Content = content
            };
            
            var key = NostrPrivateKey.FromBech32(myPrivateKeyHex);
            var signed = ev.Sign(key);
            
            client.Send(new NostrEventRequest(signed));
        }
    }
}
