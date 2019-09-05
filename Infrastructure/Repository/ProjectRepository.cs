using Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repository
{
    public class ProjectRepository : RepositoryBase, IProjectRepository
    {
        public ProjectRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}