
namespace ProductStore.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    using ProductStore.Models;

    public class ProductsController : ApiController
    {
        private OrdersContext db = new OrdersContext();

        //Project products to product DTOs.
        private IQueryable<ProductDTO> MapProducts()
        {
            return db.Products.Select(p => new ProductDTO() { Id = p.id, Name = p.Name, Price = p.Price });
        }

        public IEnumerable<ProductDTO> GetProducts()
        {
            return this.MapProducts().AsEnumerable();
        }

        public ProductDTO GetProduct(int id)
        {
            var product = this.MapProducts().FirstOrDefault(p => p.Id == 1);
            if (product == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }
            return product;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
