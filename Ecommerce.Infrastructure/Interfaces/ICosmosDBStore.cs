using Microsoft.Azure.Cosmos;

namespace Product.Data.Interfaces
{
    public interface ICosmosDBStore<T> where T : class
    {
        Task<T> Get(string id, PartitionKey partitionKey, ItemRequestOptions? itemRequestOptions, CancellationToken cancellationToken = default);
    }
}
