using Microsoft.AspNetCore.Mvc;

namespace Rentify.Backend.Presentation.WebApi.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class BaseApiController : ControllerBase
    {
      
    }
}