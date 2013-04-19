using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using ProductStore.Models;

namespace ProductStore.Controllers
{
    [Authorize]
    public class OrdersController : ApiController
    {
        private OrdersContext db = new OrdersContext();

        // GET api/Orders
        public IEnumerable<Order> GetOrders()
        {
            var a =  db.Orders.Where(o => o.Customer == User.Identity.Name);
            return a;
        }

        // GET api/Orders/5
        public OrderDTO GetOrder(int id)
        {
            var order =
                db.Orders.Include("OrderDetails.Product").First(o => o.Id == id && o.Customer == User.Identity.Name);

            if (order == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return new OrderDTO()
                       {
                           Id = order.Id,
                           Details = order.OrderDetails.Select(d => new OrderDTO.Detail
                                                                        {
                                                                            ProductID = d.Product.id,
                                                                            Product = d.Product.Name,
                                                                            Price = d.Product.Price,
                                                                            Quantity = d.Quantity
                                                                        })
                            

                       };
        }

      

        // POST api/Orders
        public HttpResponseMessage PostOrder(OrderDTO dto)
        {
            if (ModelState.IsValid)
            {

                var order = new Order()
                                {
                                    Customer = User.Identity.Name,
                                    OrderDetails = dto.Details.Select(d => new OrderDetail
                                                                               {
                                                                                   ProductId = d.ProductID,
                                                                                   Quantity = d.Quantity
                                                                               }).ToList()
                                };

                db.Orders.Add(order);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, order);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = order.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}