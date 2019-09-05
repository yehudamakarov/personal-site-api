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
            services.AddScoped<IGithubRepoFetcherJob, GithubRepoFetcherJob>();
            services.AddScoped<IRepoInfrastructure, RepoInfrastructure>();
            services.AddScoped<IRepoRepository, RepoRepository>();

            services.AddScoped<IGithubRepoFetcherNotifier, GithubRepoFetcherNotifier>();

            services.AddHostedService<RepoFetcherService>();

            return services;
        }
    }
}