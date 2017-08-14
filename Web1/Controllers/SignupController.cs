using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using AppCommon;

namespace Web1.Controllers
{
    //[Route("api/[controller]")]
    public class SignupController : Controller
    {
        [HttpGet] 
        [Route("api/signup")]
        public async Task<String> Get([FromQuery]string id6, [FromQuery]string id7)

        {
            IMyService post3 = ServiceProxy.Create<IMyService>(new Uri("fabric:/Application2/Stateful1"), new ServicePartitionKey(0));

            string message = await post3.signUp(id6, id7);
            return message;
        }

        
    }

}
