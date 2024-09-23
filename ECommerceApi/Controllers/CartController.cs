using ECommerceApi.API.DataAccess;
using ECommerceApi.API.Entites;
using ECommerceApi.Core.Controllers;
using ECommerceApi.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            Cart cart = _db.Carts
                .Include(x => x.CartProducts)
                .SingleOrDefault(x => x.AccountId == accountId && x.IsClosed == false);

            if (cart == null)
            {
                cart = new Cart
                {
                    AccountId = accountId,
                    Date = System.DateTime.Now,
                    IsClosed = false,
                    CartProducts = new List<CartProduct>()
                };

                _db.Carts.Add(cart);
                _db.SaveChanges();
            }

            CartModel data = CartToCartModel(cart);

            response.Data = data;
            return Ok(response);
        }


        [HttpPost("AddToCart/{accountId}")]
        public IActionResult AddToCart([FromRoute] int accountId, [FromBody] AddToCartModel model)
        {
            Resp<CartModel> response = new Resp<CartModel>();
            Cart cart = _db.Carts
                .Include(x => x.CartProducts)
                .SingleOrDefault(x => x.AccountId == accountId && x.IsClosed == false);

            if (cart == null)
            {
                cart = new Cart
                {
                    AccountId = accountId,
                    Date = System.DateTime.Now,
                    IsClosed = false,
                    CartProducts = new List<CartProduct>()
                };

                _db.Carts.Add(cart);
            }

            Product product = _db.Products.Find(model.ProductId);

            cart.CartProducts.Add(new CartProduct
            {
                CartId = cart.Id,
                ProductId = product.Id,
                UnitPrice = product.UnitPrice,
                DiscountedPrice = product.DiscountedPrice,
                Quantity = model.Quantity
            });

            _db.SaveChanges();

            CartModel data = CartToCartModel(cart);
            response.Data = data;

            return Ok(response);
        }


        private static CartModel CartToCartModel(Cart cart)
        {
            CartModel data = new CartModel
            {
                Id = cart.Id,
                AccountId = cart.AccountId,
                Date = cart.Date,
                IsClosed = cart.IsClosed,
                CartProducts = new List<CartProductModel>()
            };

            foreach (CartProduct cartProduct in cart.CartProducts)
            {
                data.CartProducts.Add(new CartProductModel
                {
                    Id = cartProduct.Id,
                    CartId = cartProduct.Id,
                    UnitPrice = cartProduct.UnitPrice,
                    DiscountedPrice = cartProduct.DiscountedPrice,
                    Quantity = cartProduct.Quantity,
                    ProductId = cartProduct.ProductId.Value
                });
            }

            return data;
        }
    }
}
