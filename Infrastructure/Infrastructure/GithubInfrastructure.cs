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
using Newtonsoft.Json.Linq;

namespace Infrastructure.Infrastructure
{
    public class GithubInfrastructure : RepositoryBase, IGithubInfrastructure
    {
        private readonly HttpClient _githubHttpClient;
        private readonly ILogger<GithubInfrastructure> _logger;

        public GithubInfrastructure(IConfiguration configuration, ILogger<GithubInfrastructure> logger) : base(
            configuration
        )
        {
            _logger = logger;
            var githubPrivateAccessToken = configuration["GITHUB_ACCESS_TOKEN"];
            if (githubPrivateAccessToken == null)
                throw new InvalidCredentialException("You need to set an environment variable of GITHUB_ACCESS_TOKEN");
            _githubHttpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.github.com"),
                DefaultRequestHeaders =
                {
                    Authorization = new AuthenticationHeaderValue("Bearer", githubPrivateAccessToken),
                    UserAgent =
                    {
                        new ProductInfoHeaderValue(
                            new ProductHeaderValue("yehudamakarov|personal-site-api")
                        )
                    }
                }
            };
        }

        public async Task<IList<PinnedRepo>> FetchPinnedReposAsync()
        {
            _logger.LogInformation("Fetching pinned repositories from Github.");
            var respContent = await FetchPinnedReposRespContent();
            _logger.LogInformation(
                "Completed fetching pinned repositories from Github with response {respContent}",
                respContent
            );
            var repos = ConvertRespToObjects(respContent);
            _logger.LogInformation("Completed parsing respContent to objects.");
            return repos;
        }

        private IList<PinnedRepo> ConvertRespToObjects(string respContent)
        {
            _logger.LogInformation("Parsing respContent to objects.");
            var jObject = JObject.Parse(respContent);
            var jsonReposFromGithub = jObject?["data"]?["user"]?["pinnedItems"]?["nodes"]?.Children().ToList();
            if (jsonReposFromGithub == null)
            {
                _logger.LogError("There was a problem fetching repos.");
                return new List<PinnedRepo>();
            }

            var repos = new List<PinnedRepo>();
            foreach (var repo in jsonReposFromGithub.Select(jsonRepoFromGithub =>
                jsonRepoFromGithub.ToObject<PinnedRepo>()))
            {
                _logger.LogInformation("Parsed repo from Github with Id: {DatabaseId}. Details: {@repo}",
                    repo.DatabaseId, repo);
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
                throw new HttpRequestException("The request to fetch repositories from GitHub failed.");
            var respContent = await resp.Content.ReadAsStringAsync();
            return respContent;
        }
    }
}