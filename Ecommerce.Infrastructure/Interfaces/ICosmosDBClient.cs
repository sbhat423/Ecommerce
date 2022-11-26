using Microsoft.Azure.Cosmos;
using System.Linq.Expressions;

namespace ProductWarehouse.Data.Interfaces
{
    public interface ICosmosDBClient<T>
    {
        string DatabaseId { get; }

        string ContainerId { get; }

        Task<T> InsertAsync(T data, PartitionKey? partitionKey = null, ItemRequestOptions itemRequestOptions = null, CancellationToken cancellationToken = default);

        Task<FeedResponse<T>> GetItemsAsync(QueryRequestOptions feedOptions = null, Expression<Func<T, bool>> predicate = null, string continuationToken = null, CancellationToken cancellationToken = default);

        Task<T> ReplaceItemAsync(string itemId, T item, PartitionKey? partitionKey = null, ItemRequestOptions requestOptions = null, CancellationToken cancellationToken = default);

        Task<FeedResponse<ContainerProperties>> ReadContainerAsync(string query, string continuationToken = null, QueryRequestOptions queryRequestOptions = null, CancellationToken cancellationToken = default);

        Task EnsureDatabaseExists(ThroughputProperties throughputProperties, RequestOptions requestOptions = null, CancellationToken cancellationToken = default);

        Task EnsureContainerExists(ContainerProperties containerProperties, ThroughputProperties throughputProperties, RequestOptions requestOptions = null, CancellationToken cancellationToken = default);

        Task<long> GetTotalCount(QueryRequestOptions feedOptions = null, CancellationToken cancellationToken = default);

        Task<List<T>> GetItemsAsync(int page, int pageSize, QueryRequestOptions feedOptions = null, CancellationToken cancellationToken = default);

        Task<T> RemoveItemAsync(string itemId, PartitionKey partitonKey, ItemRequestOptions itemRequestOptions = null, CancellationToken cancellationToken = default);

        Task CreateUserDefinedFunctionAsync(string udfId, string udfBody, RequestOptions requestOptions = null, CancellationToken cancellationToken = default);

        Task DeleteUserDefinedFunctionAsync(string udfId, RequestOptions requestOptions = null, CancellationToken cancellationToken = default);

        Task<ContainerResponse> GetContainerResponseAsync(ContainerRequestOptions containerRequestOptions = null, CancellationToken cancellationToken = default);
    }
}
