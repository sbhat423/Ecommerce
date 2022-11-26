using Microsoft.Azure.Cosmos;

namespace ProductWarehouse.Data.Interfaces
{
    public interface ICosmosLinqQuery<T>
    {
        QueryDefinition ToQueryDefinition(IQueryable<T> query);
    }
}
