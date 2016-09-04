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
                var blogPost = await typicode.PutAsync(1, new BlogPost { Body = "Wow!" });
                Console.WriteLine(blogPost.Body);
            }
            catch (RestException<TypicodeError> ex)
            {
                Console.WriteLine(ex);
            }
            catch (RestException ex)
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
        Task<BlogPost> PostAsync([Body] BlogPost data);

        [Put("posts/{0}")]
        Task<BlogPost> PutAsync(int id, [Body] BlogPost data);

        // TODO: Add HTTP header support
        //[Get("posts")]
        //[Header("X-API-KEY: {1}")]
        //[Header("Content-Type: {2}; charset={3}")]
        //Task<BlogPost> GetAsync(int id, string apiKey, out string contentType, out string charset);
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
