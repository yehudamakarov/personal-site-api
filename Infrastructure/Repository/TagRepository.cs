using System.Threading.Tasks;
using Core.Interfaces;
using Core.Types;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repository
{
    public class TagRepository : RepositoryBase, ITagRepository
    {
        private readonly CollectionReference _tagsCollection;

        public TagRepository(IConfiguration configuration) : base(configuration)
        {
            _tagsCollection = Db.Collection("tags");
        }

        public async Task<Tag> AddTag(Tag tag)
        {
            var tagRef = await _tagsCollection.AddAsync(tag);
            var snapshot = await tagRef.GetSnapshotAsync();
            return snapshot.ConvertTo<Tag>();
        }
    }
}