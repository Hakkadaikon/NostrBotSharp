using Nostr.Client.Messages;
using Nostr.Client.Messages.Contacts;
using Nostr.Client.Messages.Metadata;

namespace NostrBotSharp.Wrapper
{
    public class NostrEventAnalyzer
    {
        public static class Contact
        {
            public static List<string> GetRelays(NostrContactEvent ev)
            {
                var relays = new List<string>();
                if (ev.Relays == null || ev.Relays.Count == 0)
                {
                    return relays;
                }

                foreach (var relay in ev.Relays)
                {
                    relays.Add(relay.Key);
                }

                return relays;
            }
        }

        public static class Metadata
        {
            public static string GetDisplayName(NostrMetadataEvent ev)
            {
                string defaultDisplayName = "Unknown";
                if (ev.Metadata == null)
                {
                    return defaultDisplayName;
                }

                try
                {
                    return (string)ev.Metadata.AdditionalData["display_name"];
                }
                catch (Exception)
                {
                    try
                    {
                        return (string)ev.Metadata.AdditionalData["displayName"];
                    }
                    catch (Exception)
                    {
                        return defaultDisplayName;
                    }
                }                
            }
        }

        public static List<string> GetAuthors(NostrEvent ev)
        {
            var authors = new List<string>();
            if (ev.Tags == null || ev.Tags.Count == 0)
            {
                return authors;
            }

            foreach (var tag in ev.Tags)
            {
                if (tag.AdditionalData.Length != 1)
                {
                    continue;
                }

                if (string.IsNullOrEmpty((string)tag.AdditionalData[0]))
                {
                    continue;
                }

                authors.Add((string)tag.AdditionalData[0]);
            }

            return authors;
        }
    }
}
