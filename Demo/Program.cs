using Nito.AsyncEx;
using RestClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            AsyncContext.Run(() => MainAsync());
        }

        static async Task MainAsync()
        {
            var typicode = RestClient.Create<ITypicode>();
            var blogPost = await typicode.GetAsync(1);
            Console.WriteLine(blogPost.Body);
        }
    }

    [Site("https://jsonplaceholder.typicode.com")]
    public interface ITypicode
    {
        [Get("posts")]
        Task<BlogPost[]> GetAsync();

        [Get("posts/{0}")]
        Task<BlogPost> GetAsync(int id);

        [Post("posts")]
        Task<BlogPost> PostAsync(int id, [Body] BlogPost data);
    }

    public class BlogPost
    {
        public int UserId { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }
}
