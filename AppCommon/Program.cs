using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCommon
{
    public interface IMyService : IService //defines contract
    {
        Task<string> NewPost(string t, string tag, string username);
        Task<string> GetPost(string tag); // function to return posts to user
        Task<string> signUp(string username, string password);
        Task<string> logIn(string username, string password);
        Task<string> UrlImage(string url, string tag, string username);
        Task<string> FileImage(string URL, string tag, string username);
        Task<string> searchUp(string name);
        Task<string> getImage(string tag);
    }
}
