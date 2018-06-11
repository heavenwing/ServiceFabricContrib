using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Data.Notifications;
using ServiceFabricContrib;

namespace Mocks
{
    public class MockReliableDictionary2<TKey, TValue> : MockReliableDictionary<TKey, TValue>, IReliableDictionary2<TKey, TValue>
        where TKey : IComparable<TKey>, IEquatable<TKey>
    {
        public Task<IAsyncEnumerable<TKey>> CreateKeyEnumerableAsync(ITransaction txn)
        {
            return this.CreateKeyEnumerableAsync(txn, default(EnumerationMode));
        }

        public Task<IAsyncEnumerable<TKey>> CreateKeyEnumerableAsync(ITransaction txn, EnumerationMode enumerationMode)
        {
            return this.CreateKeyEnumerableAsync(txn, enumerationMode, TimeSpan.Zero, default(CancellationToken));
        }

        public Task<IAsyncEnumerable<TKey>> CreateKeyEnumerableAsync(ITransaction txn, EnumerationMode enumerationMode,
            TimeSpan timeout, CancellationToken cancellationToken)
        {
            return Task.FromResult<IAsyncEnumerable<TKey>>(
                new MockAsyncEnumerable<TKey>(
                    enumerationMode == EnumerationMode.Ordered
                        ? dictionary.Select(x => x.Key).OrderBy(k => k)
                        : dictionary.Select(x => x.Key)));
        }

        public long Count => Convert.ToInt64(dictionary.Count);
    }
}
