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

                string cacheControl;
                int maxAge;
                var blogPost = await typicode.GetAsync(1, out cacheControl, out maxAge);
                Console.WriteLine($"maxAge={maxAge}\ncacheControl={cacheControl}\n{blogPost.Body}");
            }
            catch (RestException<TypicodeError> ex)
            {
                Console.WriteLine(ex);
            }
            catch (RestException ex)
            {
                Console.WriteLine(ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    [Site("http://jsonplaceholder.typicode.com", Error = typeof(TypicodeError))]
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

        [Delete("posts/{0}")]
        Task DeleteAsync(int id);

        [Get("posts/{0}")]
        [Header("X-ID: {0}")] // in params - req header
        [Header("Cache-Control: {1}, max-age={2}")] // out params - res header
        Task<BlogPost> GetAsync(int id, out string cacheControl, out int maxAge);
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
