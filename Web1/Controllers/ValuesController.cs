using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Client;
using AppCommon;

namespace Web1.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        [Route("login")]
        public async Task<String> Get([FromQuery]string email, [FromQuery]string password)
        {
            IMyService post4 = ServiceProxy.Create<IMyService>(new Uri("fabric:/Application2/Stateful1"), new ServicePartitionKey(0));

            string message = await post4.LogIn(email, password);
            return message;
        }

    }
}
