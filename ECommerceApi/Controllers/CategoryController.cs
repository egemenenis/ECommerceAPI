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
    public class CategoryController : ControllerBase
    {
        private DatabaseContext _db;
        private IConfiguration _configuration;
        public CategoryController(DatabaseContext databaseContext, IConfiguration configuration)
        {
            _db = databaseContext;
            _configuration = configuration;
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
        [ProducesResponseType(200, Type = typeof(Resp<CategoryModel>))]
        [ProducesResponseType(400, Type = typeof(Resp<CategoryModel>))]
        public IActionResult Create([FromBody] CategoryCreateModel model)
        {
            Resp<CategoryModel> response = new Resp<CategoryModel>();
            string categoryName = model.Name?.Trim().ToLower();

            if (_db.Categories.Any(x => x.Name.ToLower() == categoryName))
            {
                response.AddError(nameof(model.Name), "This category name already exists.");
                return BadRequest(response);
            }
            else
            {
                Category category = new Category
                {
                    Name = model.Name,
                    Description = model.Description
                };

                _db.Categories.Add(category);
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
