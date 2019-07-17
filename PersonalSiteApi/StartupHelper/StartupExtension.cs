using Core.BL;
using Core.Interfaces;
using Infrastructure.Infrastructure;
using Infrastructure.Notification.RepoSync;
using Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;
using PersonalSiteApi.BackgroundServices;

namespace PersonalSiteApi.StartupHelper
{
    public static class StartupExtension
    {
        public static IServiceCollection AddSrc(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticationBL, AuthenticationBL>();
            services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();

            services.AddScoped<IGithubRepoBL, GithubRepoBL>();
            services.AddScoped<IGithubRepoFetcherBL, GithubRepoFetcherBL>();
            services.AddScoped<IRepoInfrastructure, RepoInfrastructure>();
            services.AddScoped<IRepoRepository, RepoRepository>();

            services.AddScoped<IGithubRepoFetcherNotifier, GithubGithubRepoFetcherNotifier>();

            services.AddHostedService<RepoFetcherService>();

            return services;
        }
    }
}