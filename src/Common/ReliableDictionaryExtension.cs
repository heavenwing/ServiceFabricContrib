using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.ServiceFabric.Data.Collections
{
    /// <summary>
    /// Extension for IReliableDictionary
    /// </summary>
    public static class ReliableDictionaryExtension
    {
        /// <summary>
        /// Get all data for dictionary
        /// </summary>
        /// <typeparam name="TEntityKey"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dict"></param>
        /// <param name="tx"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<List<TEntity>> GetAllAsync<TEntityKey, TEntity>(
            this IReliableDictionary<TEntityKey, TEntity> dict,
            ITransaction tx,
            CancellationToken cancellationToken)
            where TEntityKey : IComparable<TEntityKey>, IEquatable<TEntityKey>
        {
            return await (await dict.CreateEnumerableAsync(tx)).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Get data for dictionary by keys
        /// </summary>
        /// <typeparam name="TEntityKey"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dict"></param>
        /// <param name="tx"></param>
        /// <param name="ids"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<List<TEntity>> GetAllByIdsAsync<TEntityKey, TEntity>(
            this IReliableDictionary<TEntityKey, TEntity> dict,
            ITransaction tx,
            List<TEntityKey> ids,
            CancellationToken cancellationToken)
            where TEntityKey : IComparable<TEntityKey>, IEquatable<TEntityKey>
        {
            return await (await dict.CreateEnumerableAsync(tx,
                    id => ids.Contains(id), EnumerationMode.Unordered)).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Get all data for dictionary
        /// </summary>
        /// <typeparam name="TEntityKey"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dict"></param>
        /// <param name="stateManager"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<List<TEntity>> GetAllAsync<TEntityKey, TEntity>(
            this IReliableDictionary<TEntityKey, TEntity> dict,
            IReliableStateManager stateManager,
            CancellationToken cancellationToken)
            where TEntityKey : IComparable<TEntityKey>, IEquatable<TEntityKey>
        {
            using (var tx = stateManager.CreateTransaction())
            {
                return await (await dict.CreateEnumerableAsync(tx)).ToListAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Get data for dictionary by keys
        /// </summary>
        /// <typeparam name="TEntityKey"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dict"></param>
        /// <param name="stateManager"></param>
        /// <param name="ids"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<List<TEntity>> GetAllByIdsAsync<TEntityKey, TEntity>(
            this IReliableDictionary<TEntityKey, TEntity> dict,
            IReliableStateManager stateManager,
            List<TEntityKey> ids,
            CancellationToken cancellationToken)
            where TEntityKey : IComparable<TEntityKey>, IEquatable<TEntityKey>
        {
            using (var tx = stateManager.CreateTransaction())
            {
                return await (await dict.CreateEnumerableAsync(tx,
                    id => ids.Contains(id), EnumerationMode.Unordered)).ToListAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Search data for dictinary with match
        /// </summary>
        /// <typeparam name="TEntityKey"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dict"></param>
        /// <param name="tx"></param>
        /// <param name="match"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<List<TEntity>> FindAllAsync<TEntityKey, TEntity>(
            this IReliableDictionary<TEntityKey, TEntity> dict,
            ITransaction tx,
            Predicate<TEntity> match,
            CancellationToken cancellationToken)
            where TEntityKey : IComparable<TEntityKey>, IEquatable<TEntityKey>
        {
            return await (await dict.CreateEnumerableAsync(tx)).FindAllAsync(match, cancellationToken);
        }

        /// <summary>
        /// Search data for dictinary with match
        /// </summary>
        /// <typeparam name="TEntityKey"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dict"></param>
        /// <param name="stateManager"></param>
        /// <param name="match"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<List<TEntity>> FindAllAsync<TEntityKey, TEntity>(
            this IReliableDictionary<TEntityKey, TEntity> dict,
            IReliableStateManager stateManager,
            Predicate<TEntity> match,
            CancellationToken cancellationToken)
            where TEntityKey : IComparable<TEntityKey>, IEquatable<TEntityKey>
        {
            using (var tx = stateManager.CreateTransaction())
            {
                return await (await dict.CreateEnumerableAsync(tx)).FindAllAsync(match, cancellationToken);
            }
        }
    }
}
