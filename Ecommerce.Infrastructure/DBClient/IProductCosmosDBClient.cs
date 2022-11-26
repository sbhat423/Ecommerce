
using ProductWarehouse.Data.Interfaces;
using ProductWarehouse.Domain.Entities;

namespace ProductWarehouse.Data.DBClient
{
    public interface IProductCosmosDBClient : ICosmosDBClient<Product>
    {
    }
}
