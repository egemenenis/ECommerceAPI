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
    [Authorize(Roles = "Admin, Merchant")]
    public class ProductController : ControllerBase
    {
        private DatabaseContext _db;
        private IConfiguration _configuration;
        public ProductController(DatabaseContext databaseContext, IConfiguration configuration)
        {
            _db = databaseContext;
        }


        [HttpGet("list")]
        [ProducesResponseType(200, Type = typeof(Resp<List<CategoryModel>>))]
        public IActionResult List()
        {
            Resp<List<CategoryModel>> response = new Resp<List<CategoryModel>>();

            List<CategoryModel> list = _db.Categories.Select(
                x => new CategoryModel { Id = x.Id, Name = x.Name, Description = x.Description }).ToList();

            response.Data = list;

            return Ok(response);
        }


        [HttpGet("get/{id}")]
        [ProducesResponseType(200, Type = typeof(Resp<CategoryModel>))]
        [ProducesResponseType(404, Type = typeof(Resp<CategoryModel>))]
        public IActionResult GetById([FromRoute] int id)
        {
            Resp<CategoryModel> response = new Resp<CategoryModel>();

            Category category = _db.Categories.SingleOrDefault(x => x.Id == id);
            CategoryModel data = null;

            if (category == null)
                return NotFound(response);

            data = new CategoryModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
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
                    .SingleOrDefault(x => x.Id == product.Id);

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
        [ProducesResponseType(200, Type = typeof(Resp<CategoryModel>))]
        [ProducesResponseType(400, Type = typeof(Resp<CategoryModel>))]
        [ProducesResponseType(404, Type = typeof(Resp<CategoryModel>))]
        public IActionResult Update([FromRoute] int id, [FromBody] CategoryUpdateModel model)
        {
            Resp<CategoryModel> response = new Resp<CategoryModel>();
            Category category = _db.Categories.Find(id);

            if (category == null)
                return NotFound(response);

            string categoryName = model.Name?.Trim().ToLower();

            if (_db.Categories.Any(x => x.Name.ToLower() == categoryName && x.Id != id))
            {
                response.AddError(nameof(model.Name), "This category name already exists.");
                return BadRequest(response);
            }

            category.Name = model.Name;
            category.Description = model.Description;

            _db.SaveChanges();

            CategoryModel data = new CategoryModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
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
            Category category = _db.Categories.Find(id);

            if (category == null)
                return NotFound();

            _db.Categories.Remove(category);
            _db.SaveChanges();

            return Ok(response);
        }
    }
}


