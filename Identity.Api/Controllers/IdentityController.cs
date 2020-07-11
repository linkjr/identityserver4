using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        [HttpGet]
        public dynamic Get()
        {
            return from m in User.Claims
                   select new
                   {
                       m.Type,
                       m.Value
                   };
        }
    }
}
