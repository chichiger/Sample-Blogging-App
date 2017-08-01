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
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        [Route("login")]
        public async Task<String> Get([FromQuery]string id4, [FromQuery]string id5)
        //public async Task Get([FromQuery]string id2, [FromQuery]string id3)

        {
            IMyService post4 = ServiceProxy.Create<IMyService>(new Uri("fabric:/Application2/Stateful1"), new ServicePartitionKey(0));

            string message = await post4.logIn(id4, id5);
            return message;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        //public string Get(string id)
        //{
        //    return "value" + id;

        //}


        //public async Task<String> Get(string id)
        public async Task<string> Get(string id)
        {
            IMyService helloWorld = ServiceProxy.Create<IMyService>(new Uri("fabric:/Application2/Stateful1"), new ServicePartitionKey(0));

            string message = await helloWorld.HelloWorldAsync(id);
            return id;
        }
        

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
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
