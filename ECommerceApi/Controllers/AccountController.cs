using Azure;
using ECommerceApi.API.DataAccess;
using ECommerceApi.API.Entites;
using ECommerceApi.Core.Controllers;
using ECommerceApi.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using MyServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ECommerceApi.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private DatabaseContext _db;
        private IConfiguration _configuration;
        public AccountController(DatabaseContext databaseContext, IConfiguration configuration)
        {
            _db = databaseContext;
        }


        [HttpPost("merchant/applyment")]
        [ProducesResponseType(200, Type = typeof(ApplymentAccountResponseModel))]
        public IActionResult Applyment([FromBody] ApplymentAccountRequestModel model)
        {
            Resp<ApplymentAccountResponseModel> response = new Resp<ApplymentAccountResponseModel>();
            if (ModelState.IsValid)
            {
                model.Username = model.Username?.Trim().ToLower();

                if (_db.Accounts.Any(x => x.Username.ToLower() == model.Username))
                {
                    response.AddError(nameof(model.Username), "This username is already in use.");
                    return BadRequest(response);
                }
                else
                {
                    Account account = new Account
                    {
                        Username = model.Username,
                        Password = model.Password,
                        CompanyName = model.CompanyName,
                        ContactName = model.ContactName,
                        ContactEmail = model.ContactEmail,
                        Type = AccountType.Merchant,
                        IsApplyment = true
                    };
                    _db.Accounts.Add(account);
                    _db.SaveChanges();

                    ApplymentAccountResponseModel applymentAccountResponseModel = new ApplymentAccountResponseModel
                    {
                        Id = account.Id,
                        Username = account.Username,
                        ContactName = account.ContactName,
                        CompanyName = account.CompanyName,
                        ContactEmail = account.ContactEmail
                    };

                    response.Data = applymentAccountResponseModel;
                    return Ok(response);
                }
            }

            List<string> errors = ModelState.Values.SelectMany(x => x.Errors.Select(y => y.ErrorMessage)).ToList();
            return BadRequest(errors);
        }



        [HttpPost("register")]
        [ProducesResponseType(200, Type = typeof(Resp<RegisterResponseModel>))]
        [ProducesResponseType(400, Type = typeof(Resp<RegisterResponseModel>))]
        public IActionResult Register([FromBody] RegisterRequestModel model)
        {
            Resp<RegisterResponseModel> response = new Resp<RegisterResponseModel>();
            if (ModelState.IsValid)
            {
                model.Username = model.Username?.Trim().ToLower();

                if (_db.Accounts.Any(x => x.Username.ToLower() == model.Username))
                {
                    response.AddError(nameof(model.Username), "This username is already in use.");
                    return BadRequest(response);
                }
                else
                {
                    Account account = new Account
                    {
                        Username = model.Username,
                        Password = model.Password,
                        Type = AccountType.Member
                    };
                    _db.Accounts.Add(account);
                    _db.SaveChanges();

                    RegisterResponseModel data = new RegisterResponseModel
                    {
                        Id = account.Id,
                        Username = account.Username
                    };

                    response.Data = data;
                    return Ok(response);
                }
            }

            List<string> errors = ModelState.Values.SelectMany(x => x.Errors.Select(y => y.ErrorMessage)).ToList();
            return BadRequest(errors);
        }



        [HttpPost("authenticate")]
        [ProducesResponseType(200, Type = typeof(Resp<AuthenticateResponseModel>))]
        [ProducesResponseType(400, Type = typeof(Resp<AuthenticateResponseModel>))]
        public IActionResult Authenticate([FromBody] AuthenticateRequestModel model)
        {
            Resp<AuthenticateResponseModel> response = new Resp<AuthenticateResponseModel>();
            model.Username = model.Username?.Trim().ToLower();

            Account account = _db.Accounts.SingleOrDefault(
                x => x.Username.ToLower() == model.Username && x.Password == model.Password);

            if (account != null)
            {
                if (account.IsApplyment)
                {
                    response.AddError("*", "The payment has not been completed yet.");
                    return BadRequest(response);
                }
                else
                {
                    string key = _configuration["JwtOptions:Key"];

                    List<Claim> claims = new List<Claim>
                    {
                        new Claim("id", account.Id.ToString()),
                        new Claim("type", ((int)account.Type).ToString()),
                        new Claim(ClaimTypes.Name, account.Username),
                        new Claim(ClaimTypes.Role, account.Type.ToString()),
                    };

                    string token = TokenService.GenerateToken(key, DateTime.Now.AddDays(30), claims);

                    AuthenticateResponseModel data = new AuthenticateResponseModel { Token = token };
                    response.Data = data;

                    return Ok(response);
                }
            }
            else
            {
                response.AddError("*", "Username or password does not match.");
                return BadRequest(response);
            }
        }
    }
}

