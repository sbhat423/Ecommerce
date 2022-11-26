using Microsoft.AspNetCore.Mvc;
using ProductWarehouse.Business.Stores;

namespace ProductWarehouse.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductStore _productStore;

        public ProductController(IProductStore productStore)
        {
            _productStore = productStore ?? throw new ArgumentNullException(nameof(productStore));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> FindByCode([FromRoute] string id)
        {
            var result = await _productStore.FindById(id);

            return Ok(result);
        }
    }
}
