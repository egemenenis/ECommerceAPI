using ECommerceApi.API.DataAccess;
using ECommerceApi.API.Entites;
using ECommerceApi.Core.Controllers;
using ECommerceApi.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "Admin")]
    public class CartController : ControllerBase
    {
        private DatabaseContext _db;
        private IConfiguration _configuration;

        public CartController(DatabaseContext databaseContext, IConfiguration configuration)
        {
            _db = databaseContext;
            _configuration = configuration;
        }


        //GetOrCreate => Bring or create a cart
        //AddToCart => Adding product to cart

        [HttpGet("GetOrCreate/{accountId}")]
        [ProducesResponseType(200, Type = typeof(Resp<CartModel>))]
        public IActionResult GetOrCreate([FromRoute] int accountId)
        {
            Resp<CartModel> response = new Resp<CartModel>();
            Cart cart = _db.Carts.SingleOrDefault(x => x.AccountId == accountId && x.IsClosed == false);

            if (cart == null)
            {
                cart = new Cart
                {
                    AccountId = accountId,
                    Date = System.DateTime.Now,
                    IsClosed = false
                };

                _db.Carts.Add(cart);
                _db.SaveChanges();
            }

            CartModel data = new CartModel
            {
                Id = cart.Id,
                AccountId = cart.AccountId,
                Date = cart.Date,
                IsClosed = cart.IsClosed
            };

            response.Data = data;
            return Ok(response);
        }
    }
}
