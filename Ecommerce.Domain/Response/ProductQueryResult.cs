using ProductWarehouse.Domain.Entities;

namespace ProductWarehouse.Domain.Response
{
    public class ProductQueryResult
    {
        public ProductQueryResult(List<Product> promoCodes, long totalCount)
        {
            PromoCodes = promoCodes;
            NotFound = false;
            TotalCount = totalCount;
        }

        public ProductQueryResult(Product promoCode)
        {
            PromoCode = promoCode;
            NotFound = false;
        }

        public ProductQueryResult(Exception exception)
        {
            NotFound = false;
            Exception = exception;
        }

        public ProductQueryResult(ErrorResponse errorResponse)
        {
            ErrorResponse = errorResponse;
        }

        public ProductQueryResult(bool notFound, Exception exception)
        {
            NotFound = notFound;
            Exception = exception;
        }

        public Exception Exception { get; }

        public ErrorResponse ErrorResponse { get; }

        public bool HasException => Exception != null;

        public bool IsSuccess => Exception == null;

        public bool NotFound { get; }

        public Product PromoCode { get; }

        public List<Product> PromoCodes { get; }

        public long TotalCount { get; }

        public int Limit { get; }

        public static ProductQueryResult NotFoundResult(Exception exception)
        {
            return new ProductQueryResult(true, exception);
        }
    }
}

