using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Core.BL;
using Core.Interfaces;
using Core.Types;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Infrastructure.Repository
{
    public class GithubRepoInfrastructure : RepositoryBase, IGithubRepoInfrastructure
    {
        private HttpClient _githubHttpClient;

        public GithubRepoInfrastructure(IConfiguration configuration) : base(configuration)
        {
            var githubPrivateAccessToken = configuration["GITHUB_ACCESS_TOKEN"];
            _githubHttpClient = new HttpClient()
            {
                BaseAddress = new Uri("https://api.github.com"),
                DefaultRequestHeaders =
                {
                    Authorization = new AuthenticationHeaderValue("Bearer", githubPrivateAccessToken),
                    UserAgent =
                    {
                        new ProductInfoHeaderValue(new ProductHeaderValue("yehudamakarov|personal-site-api"))
                    }
                },
            };
        }


        public async Task<IEnumerable<Repo>> GetPinnedReposAsync()
        {
            var respContent = await FetchPinnedReposRespContent();
            var repos = ConvertRespToObjects(respContent);
            return repos;
        }

        private static IEnumerable<Repo> ConvertRespToObjects(string respContent)
        {
            var jObject = JObject.Parse(respContent);
            var jsonRepos = jObject["data"]["user"]["pinnedItems"]["nodes"]
                .Children()
                .ToList();
            var repos = new List<Repo>();
            foreach (var jsonRepo in jsonRepos)
            {
                var repo = jsonRepo.ToObject<Repo>();
                repos.Add(repo);
            }

            return repos;
        }

        private async Task<string> FetchPinnedReposRespContent()
        {
            const string query =
                "{\"query\":\"{\\n  user(login: \\\"yehudamakarov\\\") {\\n    pinnedItems(types: REPOSITORY, first: 6) {\\n      nodes {\\n      \\t... on Repository {\\n          name\\n          description\\n          databaseId\\n          url\\n          createdAt\\n          updatedAt\\n        }\\n      }\\n    }\\n  }\\n}\\n\"}";
            var resp = await _githubHttpClient.PostAsync("/graphql", new StringContent(query));
//            if (!resp.IsSuccessStatusCode)
//                throw new HttpRequestException("The request to fetch repositories from GitHub failed.");
            var respContent = await resp.Content.ReadAsStringAsync();
            return respContent;
        }
    }
}