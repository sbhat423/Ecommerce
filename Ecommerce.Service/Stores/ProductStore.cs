using Microsoft.Azure.Cosmos;
using ProductWarehouse.Data.DBClient;
using ProductWarehouse.Domain.Entities;
using ProductWarehouse.Domain.Response;
using System.Net;

namespace ProductWarehouse.Business.Stores
{
    internal class ProductStore : IProductStore
    {
        private readonly IProductCosmosDBClient _productCosmosDBClient;

        public ProductStore(IProductCosmosDBClient productCosmosDBClient)
        {
            _productCosmosDBClient = productCosmosDBClient;
        }

        public async Task<ProductQueryResult> Add(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            try
            {
                var feedResponse = await _productCosmosDBClient.GetItemsAsync(null, x => x.Id == product.Id);
                var result = feedResponse?.Resource?.FirstOrDefault();
                if (result != null)
                {
                    return new ProductQueryResult(new ErrorResponse() { Messages = new string[] { "Product exists" } });
                }

                var resultModel = product;
                var createdModel = await _productCosmosDBClient.InsertAsync(resultModel);
                return new ProductQueryResult(createdModel);
            }
            catch (CosmosException exception)
            {
                if (exception.StatusCode == HttpStatusCode.BadRequest)
                {
                    return ProductQueryResult.NotFoundResult(exception);
                }

                return new ProductQueryResult(exception);
            }
            catch (Exception exception)
            {
                return new ProductQueryResult(exception);
            }
        }

        public Task<ProductQueryResult> Delete(string code)
        {
            throw new NotImplementedException();
        }

        public async Task<ProductQueryResult> FindById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            try
            {
                var feedResponse = await _productCosmosDBClient.GetItemsAsync(null, x => x.Id == id);
                var result = feedResponse?.Resource?.FirstOrDefault();
                if (result == null)
                {
                    return ProductQueryResult.NotFoundResult(null);
                }

                return new ProductQueryResult(result);
            }
            catch (CosmosException exception)
            {
                if (exception.StatusCode == HttpStatusCode.NotFound)
                {
                    return ProductQueryResult.NotFoundResult(exception);
                }

                return new ProductQueryResult(exception);
            }
            catch (Exception exception)
            {
                return new ProductQueryResult(exception);
            }
        }

        public async Task<ProductQueryResult> Get(int page, int pageSize)
        {
            if (page <= 0)
            {
                throw new ArgumentException(nameof(page));
            }

            if (pageSize <= 0)
            {
                throw new ArgumentException(nameof(pageSize));
            }

            try
            {
                var totalDocuments = await _productCosmosDBClient.GetTotalCount();
                if (totalDocuments < (page - 1) * pageSize)
                {
                    return new ProductQueryResult(new ErrorResponse() { Messages = new string[] { "Product does not exist" } });
                }

                var result = await _productCosmosDBClient.GetItemsAsync(page, pageSize);
                return new ProductQueryResult(result.ToList(), totalDocuments);
            }
            catch (CosmosException exception)
            {
                return new ProductQueryResult(exception);
            }
            catch (Exception exception)
            {
                return new ProductQueryResult(exception);
            }
        }
    }
}
