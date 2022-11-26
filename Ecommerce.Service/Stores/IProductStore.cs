using ProductWarehouse.Domain.Entities;
using ProductWarehouse.Domain.Response;

namespace ProductWarehouse.Business.Stores
{
    public interface IProductStore
    {
        Task<ProductQueryResult> FindById(string code);

        Task<ProductQueryResult> Add(Product addPromoCode);

        Task<ProductQueryResult> Get(int page, int pageSize);

        Task<ProductQueryResult> Delete(string code);
    }
}
