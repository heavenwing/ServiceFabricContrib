using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;

namespace Mocks
{
    public class MockReliableConcurrentQueue<T> : IReliableConcurrentQueue<T>
    {
        private readonly ConcurrentQueue<T> queue = new ConcurrentQueue<T>();

        public Uri Name { get; set; }

        public Task EnqueueAsync(ITransaction tx, T value, CancellationToken cancellationToken = new CancellationToken(),
            TimeSpan? timeout = null)
        {
            this.queue.Enqueue(value);
            return Task.FromResult(true);
        }

        public Task<ConditionalValue<T>> TryDequeueAsync(ITransaction tx, CancellationToken cancellationToken = new CancellationToken(),
            TimeSpan? timeout = null)
        {
            T item = default(T);
            bool result = this.queue.TryDequeue(out item);
            return Task.FromResult((ConditionalValue<T>)Activator.CreateInstance(typeof(ConditionalValue<T>), result, item));
        }

        public long Count => queue.Count;
    }
}
