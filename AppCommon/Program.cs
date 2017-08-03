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
        Task<string> NewPost(String t, String tag);
        //Task NewPost(String t, String tag); // function to get post and tag from user
        Task<string> getPost(string tag); // function to return posts to user
        Task<string> signUp(string username, string password);
        Task<string> logIn(string username, string password);
        Task<string> logOut();
        Task<string> NewImage(string URL, string tag);
        Task<string> uploadImage(string URL, string tag);
        Task<string> searchUp(string name);
        Task<string> getImage(string tag);
    }
}
