using NBitcoin.RPC;
using Nostr.Client.Messages;
using Nostr.Client.Responses;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NostrBotSharp.Wrapper
{
    public class NostrEventMap
    {
        private readonly Dictionary<string, NostrEventResponse> map;
        private readonly NostrKind kind;
        private readonly uint numOfEvents;

        public NostrEventMap(NostrKind kind, uint numOfEvents = 0)
        {
            this.map = new Dictionary<string, NostrEventResponse>();
            this.kind = kind;
            this.numOfEvents = numOfEvents;
        }

        public void Organize()
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
            this.map.Remove(id);
        }

        public bool ContainsKey(string id)
        {
            return this.map.ContainsKey(id);
        }

        public NostrEventResponse Get(string id)
        {
            return this.map[id];
        }

        public void Push(string id, NostrEventResponse response)
        {
            NostrEventAnalyzer.Validate(response.Event);

            if (string.IsNullOrEmpty(id) ||
                response.Event.Kind != this.kind)
            {
                return;
            }

            if (this.map.ContainsKey(id))
            {
                if (
                 response.Event.CreatedAt.Value.Ticks >
                 this.map[id].Event.CreatedAt.Value.Ticks)
                 {
                    this.map[id] = response;
                    return;
                 }
            }

            try
            {
                this.map.Add(id, response);
            }
            catch (Exception ex)
            {
            }
            
            Organize();
        }
    }
}
