using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyServices;
using PaymentAPI.models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace PaymentAPI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class PayController : ControllerBase
    {
        private IConfiguration _configuration;
        public PayController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [AllowAnonymous]
        [HttpPost("authenticate")]
        [ProducesResponseType(200, Type = typeof(AuthResponseModel))]
        [ProducesResponseType(200, Type = typeof(string))]
        public IActionResult Authenticate([FromBody] AuthRequestModel model)
        {
            string uid = _configuration["Auth:Uid"];
            string pass = _configuration["Auth:Pass"];

            if (model.Username == uid && model.Password == pass)
            {
                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim("uid", uid));

                string token = TokenService.GenerateToken(
                    _configuration["JwtOptions:Key"],
                    DateTime.Now.AddDays(30), claims,
                    _configuration["JwtOptions:Issuer"],
                    _configuration["JwtOptions:Audience"]);

                return Ok(new AuthResponseModel { Token = token });
            }
            else
            {
                return BadRequest("Username or password does not match.");
            }
        }


        [HttpPost("payment")]
        [ProducesResponseType(200, Type = typeof(PaymentResponseModel))]
        [ProducesResponseType(200, Type = typeof(string))]
        public IActionResult Payment([FromBody] PaymentRequestModel model)
        {
            string cardno = _configuration["CardTest:No"];
            string name = _configuration["CardTest:Name"];
            string exp = _configuration["CardTest:Exp"];
            string cvv = _configuration["CardTest:CVV"];

            if (model.CardNumber == cardno && model.CardName == name && model.ExpireDate == exp && model.CVV == cvv)
            {
                return Ok(new PaymentResponseModel { Result = "Ok", TransactionId = Guid.NewGuid().ToString() });
            }
            else
            {
                return BadRequest("Card information is invalid, payment could not be received.");
            }
        }
    }
}
