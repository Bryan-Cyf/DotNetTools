using Microsoft.AspNetCore.Mvc;

namespace Bryan.Demo.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public abstract class BaseController : ControllerBase
    {

    }
}