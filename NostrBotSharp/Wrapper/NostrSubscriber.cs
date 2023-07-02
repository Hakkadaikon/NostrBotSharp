using Nostr.Client.Client;
using Nostr.Client.Responses;
using System.Reactive.Linq;

namespace NostrBotSharp.Wrapper
{
    public class NostrSubscriber
    {
        public delegate void SubscribeCallback(NostrEventResponse response);

        private readonly INostrClient client;
        private readonly SubscribeCallback callback;

        private void Validate(NostrEventResponse response)
        {
            if ((response.Event == null) ||
                (string.IsNullOrEmpty(response.Event.Id)) ||
                (string.IsNullOrEmpty(response.Event.Pubkey)) ||
                (string.IsNullOrEmpty(response.Event.Content))
                )
            {
                throw new ArgumentNullException();
            }
        }

        public NostrSubscriber(INostrClient client, SubscribeCallback callback)
        {
            this.client = client;
            this.callback = callback;
        }

        public void Subscribe()
        {
            IObservable<NostrEventResponse> events = 
                this.client.Streams.EventStream.Where(x => x.Event != null);

            _ = events.Subscribe(
                response =>
                {
                    try
                    {
                        this.Validate(response);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error!" + ex.ToString());
                    }

                    this.callback(response);
                }
            );
        }
    }
}
