using Nostr.Client.Client;
using Nostr.Client.Responses;
using System.Reactive.Linq;

namespace NostrBotSharp.Wrapper
{
    /// <summary>
    /// Class for subscribing Nostr relay data.
    /// </summary>
    public class NostrSubscriber
    {
        public delegate void SubscribeCallback(NostrEventResponse response);

        private readonly INostrClient client;
        private readonly SubscribeCallback callback;

        /// <summary>
        /// Validate before subscribe
        /// </summary>
        /// <param name="response">Nostr event response</param>
        /// <exception cref="ArgumentNullException">Exception when event data is null</exception>
        private void Validate(NostrEventResponse response)
        {
            NostrEventAnalyzer.Validate(response.Event);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="client">Nostr client interface</param>
        /// <param name="callback">Subscribe callback</param>
        public NostrSubscriber(INostrClient client, SubscribeCallback callback)
        {
            this.client = client;
            this.callback = callback;
        }

        /// <summary>
        /// Callback when subscribed.
        /// </summary>
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
