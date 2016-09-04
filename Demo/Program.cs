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
            try
            {
                var typicode = RestClient.Create<ITypicode>();
                var blogPost = await typicode.GetAsync(10000000);
                Console.WriteLine(blogPost.Body);
            }
            catch(RestException ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
    
    [Site("https://jsonplaceholder.typicode.com", Error = typeof(TypicodeError))]
    public interface ITypicode
    {
        [Get("posts")]
        Task<BlogPost[]> GetAsync();

        [Get("posts/{0}")]
        Task<BlogPost> GetAsync(int id);

        [Post("posts")]
        Task<BlogPost> PostAsync(int id, [Body] BlogPost data);
    }

    public class TypicodeError
    {
    }

    public class BlogPost
    {
        public int UserId { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }
}
