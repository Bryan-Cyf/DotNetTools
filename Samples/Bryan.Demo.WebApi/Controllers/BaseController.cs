using Microsoft.AspNetCore.Mvc;

namespace Helper.WebApi.Test.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public abstract class BaseController : ControllerBase
    {

    }
}