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
    public class PostController : Controller
    {
       
        [HttpGet] //api/{id}    ?id1=djaksdjkas&id2=djkasdjas     //post/{textbox}/{ds}
        [Route("api/post")]
        public async Task<String> Get([FromQuery]string id2, [FromQuery]string id3)
        {
            IMyService post1 = ServiceProxy.Create<IMyService>(new Uri("fabric:/Application2/Stateful1"), new ServicePartitionKey(0));

            string message = await post1.NewPost(id2, id3);
            return message;
        }

       
    }
    
}
