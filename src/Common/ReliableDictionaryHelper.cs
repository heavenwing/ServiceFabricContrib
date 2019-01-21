using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceFabricContrib
{
    /// <summary>
    /// Some utiliy methods for ReliableDictionary
    /// </summary>
    public static class ReliableDictionaryHelper
    {
        /// <summary>
        /// Get all data for some dictionary
        /// </summary>
        /// <typeparam name="TEntityKey"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="stateManager"></param>
        /// <param name="dictionaryName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<List<TEntity>> GetAllAsync<TEntityKey, TEntity>(
            IReliableStateManager stateManager,
            string dictionaryName,
            CancellationToken cancellationToken)
            where TEntityKey : IComparable<TEntityKey>, IEquatable<TEntityKey>
        {
            var dict = await stateManager.GetOrAddAsync<IReliableDictionary<TEntityKey, TEntity>>(dictionaryName);
            return await dict.GetAllAsync(stateManager, cancellationToken);
        }

        /// <summary>
        /// Get all data for some dictionary by keys
        /// </summary>
        /// <typeparam name="TEntityKey"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="stateManager"></param>
        /// <param name="dictionaryName"></param>
        /// <param name="ids"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<List<TEntity>> GetAllByIdsAsync<TEntityKey, TEntity>(
            IReliableStateManager stateManager,
            string dictionaryName,
            List<TEntityKey> ids,
            CancellationToken cancellationToken)
            where TEntityKey : IComparable<TEntityKey>, IEquatable<TEntityKey>
        {
            var dict = await stateManager.GetOrAddAsync<IReliableDictionary<TEntityKey, TEntity>>(dictionaryName);
            return await dict.GetAllByIdsAsync(stateManager, ids, cancellationToken);
        }


        /// <summary>
        /// Search data for some dictionary
        /// </summary>
        /// <typeparam name="TEntityKey"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="stateManager"></param>
        /// <param name="dictionaryName"></param>
        /// <param name="match"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<List<TEntity>> FindAllAsync<TEntityKey, TEntity>(
            IReliableStateManager stateManager,
            string dictionaryName,
            Predicate<TEntity> match,
            CancellationToken cancellationToken)
            where TEntityKey : IComparable<TEntityKey>, IEquatable<TEntityKey>
        {
            var dict = await stateManager.GetOrAddAsync<IReliableDictionary<TEntityKey, TEntity>>(dictionaryName);
            return await dict.FindAllAsync(stateManager, match, cancellationToken);
        }
    }
}
