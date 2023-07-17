using Nostr.Client.Messages;
using Nostr.Client.Messages.Contacts;
using Nostr.Client.Messages.Metadata;
using Nostr.Client.Responses;

namespace NostrBotSharp.Wrapper
{
    /// <summary>
    /// Class for analyzing Nostr event.
    /// </summary>
    public static class NostrEventAnalyzer
    {
        public static void Validate(NostrEvent? ev)
        {
            if ((ev == null) ||
                (string.IsNullOrEmpty(ev.Id)) ||
                (string.IsNullOrEmpty(ev.Pubkey))
                )
            {
                throw new ArgumentNullException();
            }
        }
        
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

                string displayName1 = null; 
                string displayName2 = null;
                try
                {
                    displayName1  = (string)ev.Metadata.AdditionalData["display_name"];
                }
                catch (Exception)
                {
                    try
                    {
                        displayName2  = (string)ev.Metadata.AdditionalData["displayName"];
                    }
                    catch (Exception)
                    {
                        return defaultDisplayName;
                    }
                }

                return displayName1 ?? displayName2;
            }

            public static string GetName(NostrMetadataEvent ev)
            {
                string defaultDisplayName = "Unknown";
                if (ev.Metadata == null)
                {
                    return defaultDisplayName;
                }

                if (string.IsNullOrEmpty(ev.Metadata.Name))
                {
                    return defaultDisplayName;
                }

                return ev.Metadata.Name;
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
