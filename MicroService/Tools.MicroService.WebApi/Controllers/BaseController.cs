using Microsoft.AspNetCore.Mvc;

namespace Tools.MicroService.WebApi
{
    [ApiController]
    [Route("[controller]/[action]")]
    public abstract class BaseController : ControllerBase
    {

    }
}