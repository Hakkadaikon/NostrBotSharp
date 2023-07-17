using Nostr.Client.Messages;
using Nostr.Client.Responses;

namespace NostrBotSharp.Wrapper
{
    public class NostrEventMap
    {
        private readonly Dictionary<string, NostrEventResponse> map;
        private readonly NostrKind kind;
        private readonly uint numOfEvents;
        private object syncObject = new Object();

        public NostrEventMap(NostrKind kind, uint numOfEvents = 0)
        {
            this.map = new Dictionary<string, NostrEventResponse>();
            this.kind = kind;
            this.numOfEvents = numOfEvents;
        }

        private void Organize()
        {
            if (this.numOfEvents == 0)
            {
                return;
            }

            foreach (var item in SortByTime())
            {
                if (this.map.Count <= this.numOfEvents)
                {
                    break;
                }

                this.map.Remove(item.Key);
            }
        }

        public IOrderedEnumerable<KeyValuePair<string, NostrEventResponse>> SortByTime()
        {
            return
                this.map.OrderBy(x => x.Value.Event.CreatedAt.Value.Ticks);
        }

        public int Count()
        {
            return this.map.Count;
        }

        public void Remove(string id)
        {
            bool lockToken = false;
            Monitor.Enter(syncObject, ref lockToken);
            this.map.Remove(id);
            if (lockToken)
            {
                Monitor.Exit(syncObject);
            }
        }

        public bool ContainsKey(string id)
        {
            bool lockToken = false;
            Monitor.Enter(syncObject, ref lockToken);
            bool containsKey = this.map.ContainsKey(id);
            if (lockToken)
            {
                Monitor.Exit(syncObject);
            }

            return containsKey;
        }

        public NostrEventResponse Get(string id)
        {
            bool lockToken = false;
            Monitor.Enter(syncObject, ref lockToken);
            NostrEventResponse response = this.map[id];
            if (lockToken)
            {
                Monitor.Exit(syncObject);
            }

            return response;
        }

        public void Push(string id, NostrEventResponse response)
        {
            bool lockToken = false;
            Monitor.Enter(syncObject, ref lockToken);

            try
            {
                NostrEventAnalyzer.Validate(response.Event);
                if (string.IsNullOrEmpty(id) ||
                    response.Event.Kind != this.kind)
                {
                    goto FINALIZE;
                }

                if (this.map.ContainsKey(id))
                {
                    if (
                     response.Event.CreatedAt.Value.Ticks >
                     this.map[id].Event.CreatedAt.Value.Ticks)
                    {
                        this.map[id] = response;
                        goto FINALIZE;
                    }
                }

                this.map.Add(id, response);
            }
            catch (Exception ex)
            {
            }

        FINALIZE:
            if (lockToken)
            {
                Monitor.Exit(syncObject);
            }
        }
    }
}
