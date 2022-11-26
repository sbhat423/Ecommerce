using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;
using ProductWarehouse.Data.Interfaces;
using System.Linq.Expressions;

namespace ProductWarehouse.Data
{
    public abstract class CosmosDBClient<T> : ICosmosDBClient<T>
    {
        public abstract string ContainerId { get; }

        public abstract string DatabaseId { get; }

        public QueryRequestOptions FeedOptions => new QueryRequestOptions();

        protected Container Container { get; set; }

        protected Database Database { get; set; }

        protected int OfferThroughput { get; }

        protected CosmosClient Client => _client;

        private const string ItemCountQuery = "SELECT count(1) as Total FROM c";

        private CosmosClient _client;

        private ICosmosLinqQuery<T> _cosmosLinqQuery;

        /// <summary>
        /// Initializes a new instance of the <see cref="CosmosDBClient{T}"/> class.
        /// </summary>
        /// <param name="cosmosClient">To achieve the "single instance of CosmosClient per lifetime of the application" recommended by Microsoft,
        /// we recommend you configure it as a Singleton in your IoC framework (e.g. Use "AddSingleton()"
        /// when registering the "CosmosClient" service)</param>
        /// <param name="cosmosLinqQuery">Instance of ICosmosLinqQuery<T></param>
        /// <exception cref="ArgumentNullException">
        ///     <p><paramref name="cosmosClient"/> is null. </p>
        ///     <p>- or - </p>
        ///     <p><paramref name="cosmosLinqQuery"/> is null. </p>
        /// </exception>
        protected CosmosDBClient(CosmosClient cosmosClient, ICosmosLinqQuery<T> cosmosLinqQuery)
        {
            _client = cosmosClient ?? throw new ArgumentNullException(nameof(cosmosClient));
            _cosmosLinqQuery = cosmosLinqQuery ?? throw new ArgumentNullException(nameof(cosmosLinqQuery));
        }

        public async Task EnsureDatabaseExists(
            ThroughputProperties throughputProperties,
            RequestOptions requestOptions = null,
            CancellationToken cancellationToken = default)
        {
            var databaseResponse = await _client.CreateDatabaseIfNotExistsAsync(
                DatabaseId, throughputProperties, requestOptions, cancellationToken);
            Database = databaseResponse.Database;
        }

        public async Task EnsureContainerExists(
            ContainerProperties containerProperties,
            ThroughputProperties throughputProperties,
            RequestOptions requestOptions = null,
            CancellationToken cancellationToken = default)
        {
            containerProperties.Id = ContainerId;
            var containerResponse = await Database.CreateContainerIfNotExistsAsync(
                containerProperties, throughputProperties, requestOptions, cancellationToken);
            Container = containerResponse.Container;
        }

        public async Task<T> InsertAsync(
            T data,
            PartitionKey? partitionKey = null,
            ItemRequestOptions itemRequestOptions = null,
            CancellationToken cancellationToken = default)
        {
            var result = await Container.CreateItemAsync(data, partitionKey, itemRequestOptions, cancellationToken);
            return result.Resource;
        }

        public async Task<FeedResponse<T>> GetItemsAsync(
            QueryRequestOptions feedOptions = null,
            Expression<Func<T, bool>> predicate = null,
            string continuationToken = null,
            CancellationToken cancellationToken = default)
        {
            var query = predicate == null
                ? Container.GetItemLinqQueryable<T>(false, continuationToken, feedOptions)
                : Container.GetItemLinqQueryable<T>(false, continuationToken, feedOptions).Where(predicate);

            var feedIterator = Container.GetItemQueryIterator<T>(
                _cosmosLinqQuery.ToQueryDefinition(query), continuationToken, feedOptions);
            return await feedIterator.ReadNextAsync(cancellationToken);
        }

        public async Task<T> ReplaceItemAsync(
            string documentId,
            T document,
            PartitionKey? partitionKey = null,
            ItemRequestOptions requestOptions = null,
            CancellationToken cancellationToken = default)
        {
            var result = await Container.ReplaceItemAsync(
                document, documentId, partitionKey, requestOptions, cancellationToken);
            return result.Resource;
        }

        public async Task<FeedResponse<ContainerProperties>> ReadContainerAsync(
            string query = null,
            string continuationToken = null,
            QueryRequestOptions queryRequestOptions = null,
            CancellationToken cancellationToken = default)
        {
            FeedIterator<ContainerProperties> iterator = Database.GetContainerQueryIterator<ContainerProperties>(
                query, continuationToken, queryRequestOptions);
            return await iterator.ReadNextAsync(cancellationToken);
        }

        public async Task<ContainerResponse> GetContainerResponseAsync(
            ContainerRequestOptions containerRequestOptions = null, CancellationToken cancellationToken = default)
        {
            return await Container.ReadContainerAsync(containerRequestOptions, cancellationToken);
        }

        public async Task<long> GetTotalCount(
            QueryRequestOptions feedOptions = null,
            CancellationToken cancellationToken = default)
        {
            QueryDefinition queryDefinition = new QueryDefinition(ItemCountQuery);
            var feedIterator = Container.GetItemQueryIterator<ItemCount>(queryDefinition, null, feedOptions);
            var feedResponse = await feedIterator.ReadNextAsync(cancellationToken);

            var documentCount = feedResponse.FirstOrDefault();
            return documentCount != null ? documentCount.Total : 0;
        }

        public virtual async Task<List<T>> GetItemsAsync(
            int page,
            int pageSize,
            QueryRequestOptions feedOptions = null,
            CancellationToken cancellationToken = default)
        {
            int pageOffset = (page - 1) * pageSize;
            List<T> data = new List<T>();
            var query = $"SELECT * FROM C OFFSET {pageOffset} LIMIT {pageSize}";
            var queryDefinition = new QueryDefinition(query);

            using (FeedIterator<T> feedIterator = Container.GetItemQueryIterator<T>(queryDefinition, null, feedOptions))
            {
                while (feedIterator.HasMoreResults)
                {
                    data.AddRange(await feedIterator.ReadNextAsync(cancellationToken));
                }
            }

            return data;
        }

        public virtual async Task<T> RemoveItemAsync(
            string itemId,
            PartitionKey partitonKey,
            ItemRequestOptions itemRequestOptions = null,
            CancellationToken cancellationToken = default)
        {
            var result = await Container.DeleteItemAsync<T>(itemId, partitonKey, itemRequestOptions, cancellationToken);
            return result.Resource;
        }

        public async Task DeleteUserDefinedFunctionAsync(
            string udfId,
            RequestOptions requestOptions = null,
            CancellationToken cancellationToken = default)
        {
            Scripts scripts = Container?.Scripts;
            await scripts.DeleteUserDefinedFunctionAsync(udfId, requestOptions, cancellationToken);
        }

        public async Task CreateUserDefinedFunctionAsync(
            string udfId,
            string udfBody,
            RequestOptions requestOptions = null,
            CancellationToken cancellationToken = default)
        {
            UserDefinedFunctionProperties userDefinedFunctionProperties = new UserDefinedFunctionProperties
            {
                Id = udfId,
                Body = udfBody
            };

            Scripts scripts = Container?.Scripts;
            await scripts.CreateUserDefinedFunctionAsync(userDefinedFunctionProperties, requestOptions, cancellationToken);
        }
    }
}
