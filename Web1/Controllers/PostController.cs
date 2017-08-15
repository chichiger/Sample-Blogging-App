using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Client;
using AppCommon;

namespace Web1.Controllers
{
    public class PostController : Controller
    {
        [HttpGet] 
        [Route("api/post")]
        public async Task<String> Get([FromQuery]string text, [FromQuery]string hashtags, [FromQuery] string cookie)
        {
            IMyService post1 = ServiceProxy.Create<IMyService>(new Uri("fabric:/Application2/Stateful1"), new ServicePartitionKey(0));

            string message = await post1.NewPost(text, hashtags, cookie);
            return message;
        }

       
    }
    
}
