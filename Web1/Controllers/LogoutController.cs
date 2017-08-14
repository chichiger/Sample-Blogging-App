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
    public class LogoutController : Controller
    {
       
        [HttpGet] 
        [Route("api/logout")]
        public async Task<String> Get()
        {
            IMyService post5 = ServiceProxy.Create<IMyService>(new Uri("fabric:/Application2/Stateful1"), new ServicePartitionKey(0));

            string message = await post5.logOut();
            return message;
        }

    }

}
