using Core.BL;
using Core.Interfaces;
using Core.Job;
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

            services.AddScoped<IGithubRepoInfrastructure, GithubRepoInfrastructure>();

            services.AddScoped<IGithubRepoBL, GithubRepoBL>();
            services.AddScoped<IRepoRepository, RepoRepository>();

            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IProjectBL, ProjectBL>();

            services.AddScoped<IGithubRepoFetcherNotifier, GithubRepoFetcherNotifier>();

            services.AddScoped<IGithubRepoFetcherJob, GithubRepoFetcherJob>();
            services.AddScoped<IAddToProjectsJob, AddToProjectsJob>();

            services.AddHostedService<RepoFetcherService>();
            services.AddHostedService<AddToProjectsService>();


            return services;
        }
    }
}