using CsTemplateBot.Models;
using System.Net.Http;
using RestEase;
using System.Threading.Tasks;

namespace CsTemplateBot.Services
{
    public class GitService : IGitService
    {
        private const string UserEndpoint = "https://api.github.com/users/";
        private const string ReposEndpoint = "https://api.github.com/search/repositories";

        public async Task<GitUser> GetUser(string username)
        {
            var client = RestClient.For<IGitApi>("https://api.github.com");
            GitUser user = null;
            try
            {
                await client.GetUserAsync(username);
            }
            catch (ApiException)
            {
            }
            return user;
        }
    }
}
