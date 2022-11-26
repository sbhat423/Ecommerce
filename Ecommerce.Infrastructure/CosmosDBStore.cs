using Product.Data.Interfaces;
using Microsoft.Azure.Cosmos;

namespace Product.Data
{
    internal class CosmosDBStore<T> : ICosmosDBStore <T> where T : class
    {
        public Database Database { get; set; }
        public Container Container { get; set; }
        private readonly CosmosClient _cosmosClient;

        public CosmosDBStore(CosmosClient cosmosClient)
        {   
            _cosmosClient = cosmosClient ?? throw new ArgumentNullException(nameof(cosmosClient));
        }

        public async Task<T> Get(string id, PartitionKey partitionKey, ItemRequestOptions? itemRequestOptions, CancellationToken cancellationToken = default)
        {
            return await Container.ReadItemAsync<T>(id, partitionKey, itemRequestOptions, cancellationToken);
        }
    }
}
