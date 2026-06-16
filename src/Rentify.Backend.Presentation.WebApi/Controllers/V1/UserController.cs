using Microsoft.AspNetCore.Mvc;
using Rentify.Backend.Core.Application.Dtos.Accounts;
using Rentify.Backend.Core.Application.Interfaces.Services;
using Rentify.Backend.Infraestructure.Identity.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;


namespace Rentify.Backend.Presentation.WebApi.Controllers
{
    [ApiController]
    public class UserController : BaseApiController
    {
        private readonly IAccountService _accountService;
        private readonly IJwtServices _jwtServices;

        public UserController(IAccountService accountService, IJwtServices jwtServices)
        {
            _accountService = accountService;
            _jwtServices = jwtServices;
        }

        [HttpPost("register")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest register)
        {
            try
            {
                var response = await _accountService.RegisterAsync(register, null);
                return response.HasError ? BadRequest(response.Error) : StatusCode(StatusCodes.Status201Created, "Usuario creado con existo");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("login")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> LoginAsync([FromBody] AuthenticationRequest request)
        {
            try
            {
                var response = await _accountService.AuthenticationAsync(request);
                return StatusCode(response.HasError ? StatusCodes.Status400BadRequest : StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}