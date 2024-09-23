using ECommerceApi.API.DataAccess;
using ECommerceApi.API.Entites;
using ECommerceApi.Core.Controllers;
using ECommerceApi.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ECommerceApi.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "Admin, Merchant")]
    public class ProductController : ControllerBase
    {
        private DatabaseContext _db;
        private IConfiguration _configuration;
        public ProductController(DatabaseContext databaseContext, IConfiguration configuration)
        {
            _db = databaseContext;
            _configuration = configuration;
        }


        [HttpGet("list")]
        [ProducesResponseType(200, Type = typeof(Resp<List<ProductModel>>))]
        public IActionResult List()
        {
            Resp<List<ProductModel>> response = new Resp<List<ProductModel>>();

            //int accountId = int.Parse(HttpContext.User.FindFirst("id").Value);

            List<ProductModel> list = _db.Products
                .Include(x => x.Category)
                .Include(x => x.Account)
                //.Where(x =/*> x.AccountId == accountId)*/
                .Select(x => new ProductModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    UnitPrice = x.UnitPrice,
                    DiscountedPrice = x.DiscountedPrice,
                    Discountinued = x.Discountinued,
                    CategoryId = x.CategoryId,
                    AccountId = x.AccountId,
                    CategoryName = x.Category.Name,
                    AccountCompanyName = x.Account.CompanyName
                }).ToList();

            response.Data = list;

            return Ok(response);
        }


        [HttpGet("list/{accountId}")]
        [ProducesResponseType(200, Type = typeof(Resp<List<ProductModel>>))]
        public IActionResult ListByAccountId([FromRoute] int accountId)
        {
            Resp<List<ProductModel>> response = new Resp<List<ProductModel>>();

            //int accountId = int.Parse(HttpContext.User.FindFirst("id").Value);

            List<ProductModel> list = _db.Products
                .Include(x => x.Category)
                .Include(x => x.Account)
                //.Where(x =/*> x.AccountId == accountId)*/
                .Select(x => new ProductModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    UnitPrice = x.UnitPrice,
                    DiscountedPrice = x.DiscountedPrice,
                    Discountinued = x.Discountinued,
                    CategoryId = x.CategoryId,
                    AccountId = x.AccountId,
                    CategoryName = x.Category.Name,
                    AccountCompanyName = x.Account.CompanyName
                }).ToList();

            response.Data = list;

            return Ok(response);
        }


        [HttpGet("get/{productId}")]
        [ProducesResponseType(200, Type = typeof(Resp<ProductModel>))]
        [ProducesResponseType(404, Type = typeof(Resp<ProductModel>))]
        public IActionResult GetById([FromRoute] int productId)
        {
            Resp<ProductModel> response = new Resp<ProductModel>();

            Product product = _db.Products
                .Include(x => x.Category)
                .Include(x => x.Account)
                .SingleOrDefault(x => x.Id == productId);

            if (product == null)
                return NotFound(response);

            ProductModel data = new ProductModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                UnitPrice = product.UnitPrice,
                DiscountedPrice = product.DiscountedPrice,
                Discountinued = product.Discountinued,
                CategoryId = product.CategoryId,
                AccountId = product.AccountId,
                CategoryName = product.Category.Name,
                AccountCompanyName = product.Account.CompanyName
            };

            response.Data = data;

            return Ok(response);
        }


        [HttpPost("create")]
        [ProducesResponseType(200, Type = typeof(Resp<ProductModel>))]
        [ProducesResponseType(400, Type = typeof(Resp<ProductModel>))]
        public IActionResult Create([FromBody] ProductCreateModel model)
        {
            Resp<ProductModel> response = new Resp<ProductModel>();
            string productName = model.Name?.Trim().ToLower();

            if (_db.Products.Any(x => x.Name.ToLower() == productName))
            {
                response.AddError(nameof(model.Name), "This product name is already available.");
                return BadRequest(response);
            }
            else
            {
                int accountId = int.Parse(HttpContext.User.FindFirst("id").Value);

                Product product = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    UnitPrice = model.UnitPrice,
                    DiscountedPrice = model.DiscountedPrice,
                    Discountinued = model.Discountinued,
                    CategoryId = model.CategoryId,
                    AccountId = accountId
                };

                _db.Products.Add(product);
                _db.SaveChanges();

                product = _db.Products
                    .Include(x => x.Category)
                    .Include(x => x.Account)
                    .SingleOrDefault(x => x.Id == product
                    .Id);

                ProductModel data = new ProductModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    UnitPrice = model.UnitPrice,
                    DiscountedPrice = model.DiscountedPrice,
                    Discountinued = model.Discountinued,
                    CategoryId = model.CategoryId,
                    AccountId = product.AccountId,
                    CategoryName = product.Category.Name,
                    AccountCompanyName = product.Account.CompanyName
                };

                response.Data = data;
                return Ok(response);
            }
        }


        [HttpPut("update/{id}")]
        [ProducesResponseType(200, Type = typeof(Resp<ProductModel>))]
        [ProducesResponseType(400, Type = typeof(Resp<ProductModel>))]
        [ProducesResponseType(404, Type = typeof(Resp<ProductModel>))]
        public IActionResult Update([FromRoute] int id, [FromBody] ProductUpdateModel model)
        {
            Resp<ProductModel> response = new Resp<ProductModel>();
            int accountId = int.Parse(HttpContext.User.FindFirst("id").Value);
            string role = HttpContext.User.FindFirst(ClaimTypes.Role).Value;

            Product product = _db.Products.SingleOrDefault(x => x.Id == id && (role == "Admin" || (role != "Admin" && x.AccountId == accountId)));

            if (product == null)
                return NotFound(response);

            string productName = model.Name?.Trim().ToLower();

            if (_db.Products.Any(x => x.Name.ToLower() == productName && x.Id != id && (role == "Admin" || (role != "Admin" && x.AccountId == accountId))))
            {
                response.AddError(nameof(model.Name), "This product name already exists.");
                return BadRequest(response);
            }

            product.Name = model.Name;
            product.Description = model.Description;
            product.UnitPrice = model.UnitPrice;
            product.DiscountedPrice = model.DiscountedPrice;
            product.Discountinued = model.Discountinued;
            product.CategoryId = model.CategoryId;

            _db.SaveChanges();

            product = _db.Products
                .Include(x => x.Category)
                .Include(x => x.Account)
                .SingleOrDefault(x => x.Id == id);

            ProductModel data = new ProductModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                UnitPrice = model.UnitPrice,
                DiscountedPrice = model.DiscountedPrice,
                Discountinued = model.Discountinued,
                CategoryId = model.CategoryId,
                AccountId = product.AccountId,
                CategoryName = product.Category.Name,
                AccountCompanyName = product.Account.CompanyName
            };

            response.Data = data;

            return Ok(response);
        }


        [HttpDelete("delete/{id}")]
        [ProducesResponseType(200, Type = typeof(Resp<object>))]
        [ProducesResponseType(404, Type = typeof(Resp<object>))]
        public IActionResult Delete([FromRoute] int id)
        {
            Resp<object> response = new Resp<object>();
            int accountId = int.Parse(HttpContext.User.FindFirst("id").Value);
            string role = HttpContext.User.FindFirst(ClaimTypes.Role).Value;

            Product product = _db.Products.SingleOrDefault(x => x.Id == id && (role == "Admin" || (role != "Admin" && x.AccountId == accountId)));

            if (product == null)
                return NotFound();

            _db.Products.Remove(product);
            _db.SaveChanges();

            return Ok(response);
        }
    }
}


