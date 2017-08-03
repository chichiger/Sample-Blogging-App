using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using AppCommon;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Drawing;

namespace Web1.Controllers
{
    //[Route("api/[controller]")]
    public class ImageController : Controller
    {
        //[HttpGet("api/abc")]
        [HttpGet] //api/{id}    ?id1=djaksdjkas&id2=djkasdjas     //post/{textbox}/{ds}
        [Route("api/image")]
        public async Task<String> Get([FromQuery]string id8, [FromQuery]string id9)

        {
            IMyService post3 = ServiceProxy.Create<IMyService>(new Uri("fabric:/Application2/Stateful1"), new ServicePartitionKey(0));

            string message = await post3.NewImage(id8, id9);
            return message;
        }

        [Route("api/upload")]
        public async Task<String> GetFile([FromQuery]string id10, [FromQuery]string id9)
        //public async Task Get([FromQuery]string id2, [FromQuery]string id3)

        {
            IMyService post3 = ServiceProxy.Create<IMyService>(new Uri("fabric:/Application2/Stateful1"), new ServicePartitionKey(0));

            string message = await post3.uploadImage(id10, id9);
            return message;
        }


        // POST api/values
        [HttpPost]
        [Route("api/file")] 
        public async Task<IActionResult> Post(IFormFile file, string id12)
        {
            string s = "";
            
            if (file.Length > 0)
            {
                using (var fileStream = file.OpenReadStream())
                using (var ms = new MemoryStream())
                {
                    fileStream.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    s = Convert.ToBase64String(fileBytes);
                }
            }
            IMyService post5 = ServiceProxy.Create<IMyService>(new Uri("fabric:/Application2/Stateful1"), new ServicePartitionKey(0));
            
            string message = await post5.uploadImage(s, id12);
            return Ok("passed");
        }
        
       


        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

}
