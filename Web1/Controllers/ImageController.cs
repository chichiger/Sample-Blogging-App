// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Web1.Controllers
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using AppCommon;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Client;

    public class ImageController : Controller
    {
        [HttpGet]
        [Route("api/image")]
        public async Task<String> Get([FromQuery] string id8, [FromQuery] string id9, [FromQuery] string cookie)
        {
            IMyService post3 = ServiceProxy.Create<IMyService>(new Uri("fabric:/Application2/Stateful1"), new ServicePartitionKey(0));

            string message = await post3.UrlImage(id8, id9, cookie);
            return message;
        }


        [HttpPost]
        [Route("api/file")]
        public async Task<IActionResult> Post(IFormFile file, string id12, string cookie)
        {
            string s = "";

            if (file.Length > 0)
            {
                using (Stream fileStream = file.OpenReadStream())
                using (MemoryStream ms = new MemoryStream())
                {
                    fileStream.CopyTo(ms);
                    byte[] fileBytes = ms.ToArray();
                    s = Convert.ToBase64String(fileBytes);
                }
            }
            IMyService post5 = ServiceProxy.Create<IMyService>(new Uri("fabric:/Application2/Stateful1"), new ServicePartitionKey(0));

            string message = await post5.FileImage(s, id12, cookie);
            return this.Ok("passed");
        }
    }
}