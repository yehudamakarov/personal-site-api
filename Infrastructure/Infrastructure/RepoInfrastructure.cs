using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Types;
using Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Infrastructure.Infrastructure
{
    public class RepoInfrastructure : RepositoryBase, IRepoInfrastructure
    {
        private readonly HttpClient _githubHttpClient;
        private readonly ILogger<RepoInfrastructure> _logger;

        public RepoInfrastructure(
            IConfiguration configuration, ILogger<RepoInfrastructure> logger) : base(
            configuration
        )
        {
            _logger = logger;
            var githubPrivateAccessToken = configuration["GITHUB_ACCESS_TOKEN"];
            if (githubPrivateAccessToken == null)
                throw new InvalidCredentialException(
                    "You need to set an environment variable of GITHUB_ACCESS_TOKEN"
                );
            _githubHttpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.github.com"),
                DefaultRequestHeaders =
                {
                    Authorization = new AuthenticationHeaderValue(
                        "Bearer",
                        githubPrivateAccessToken
                    ),
                    UserAgent =
                    {
                        new ProductInfoHeaderValue(
                            new ProductHeaderValue("yehudamakarov|personal-site-api")
                        )
                    }
                }
            };
        }

        public async Task<IEnumerable<Repo>> FetchPinnedReposAsync()
        {
            _logger.LogInformation("Fetching pinned repositories from Github.");
            var respContent = await FetchPinnedReposRespContent();
            _logger.LogInformation(
                "Completed fetching pinned repositories from Github with response {respContent}",
                respContent
            );
            var repos = ConvertRespToObjects(respContent);
            _logger.LogInformation(
                "Completed parsing respContent to objects. Produced {repos}.",
                JsonConvert.SerializeObject(repos)
            );
            return repos;
        }

        private IEnumerable<Repo> ConvertRespToObjects(string respContent)
        {
            _logger.LogInformation("Parsing respContent to objects.");
            var jObject = JObject.Parse(respContent);
            var jsonRepos = jObject["data"]["user"]["pinnedItems"]["nodes"]
                .Children()
                .ToList();
            var repos = new List<Repo>();
            foreach (var jsonRepo in jsonRepos)
            {
                var repo = jsonRepo.ToObject<Repo>();
                _logger.LogInformation("Parsed {@repo}", repo);
                repos.Add(repo);
            }

            return repos;
        }

        private async Task<string> FetchPinnedReposRespContent()
        {
            const string query =
                "{\"query\":\"{\\n  user(login: \\\"yehudamakarov\\\") {\\n    pinnedItems(types: REPOSITORY, first: 6) {\\n      nodes {\\n      \\t... on Repository {\\n          name\\n          description\\n          databaseId\\n          url\\n          createdAt\\n          updatedAt\\n        }\\n      }\\n    }\\n  }\\n}\\n\"}";
            var resp = await _githubHttpClient.PostAsync("/graphql", new StringContent(query));
            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException(
                    "The request to fetch repositories from GitHub failed."
                );
            var respContent = await resp.Content.ReadAsStringAsync();
            return respContent;
        }
    }
}