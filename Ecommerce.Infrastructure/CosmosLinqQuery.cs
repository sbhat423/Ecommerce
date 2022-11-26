using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using ProductWarehouse.Data.Interfaces;

namespace ProductWarehouse.Data
{
    public class CosmosLinqQuery<T> : ICosmosLinqQuery<T>
    {
        public QueryDefinition ToQueryDefinition(IQueryable<T> query) => query.ToQueryDefinition();
    }
}
