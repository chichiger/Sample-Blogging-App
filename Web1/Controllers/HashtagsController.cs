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
    public class HashtagsController : Controller
    {
        [HttpGet] 
        [Route("api/hashtags")]

        public async Task<string> Get([FromQuery] string id1)
        {
            IMyService post2 = ServiceProxy.Create<IMyService>(new Uri("fabric:/Application2/Stateful1"), new ServicePartitionKey(0));
            string results = await post2.GetPost(id1);
            return results;
        }

        [Route("api/search")]
        public async Task<String> GetOption([FromQuery]string id1)
        {
            IMyService post3 = ServiceProxy.Create<IMyService>(new Uri("fabric:/Application2/Stateful1"), new ServicePartitionKey(0));

            string message = await post3.searchUp(id1);
            return message;
        }

        [Route("api/getimg")]
        public async Task<String> GetPics([FromQuery]string id1)
        {
            IMyService post3 = ServiceProxy.Create<IMyService>(new Uri("fabric:/Application2/Stateful1"), new ServicePartitionKey(0));

            string message = await post3.getImage(id1);
            return message;
        }


    }

}
