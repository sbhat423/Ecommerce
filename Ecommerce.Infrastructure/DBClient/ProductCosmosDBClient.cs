using Microsoft.Azure.Cosmos;
using ProductWarehouse.Domain.Entities;

namespace ProductWarehouse.Data.DBClient
{
    public class ProductCosmosDBClient : CosmosDBClient<Product>, IProductCosmosDBClient
    {
        public ProductCosmosDBClient(CosmosClient cosmosClient)
            : base(cosmosClient, new CosmosLinqQuery<Product>())
        {
        }

        public override string ContainerId => "Product";

        public override string DatabaseId => "Product";
    }
}
