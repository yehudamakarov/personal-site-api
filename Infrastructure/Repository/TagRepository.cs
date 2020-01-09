using System.Collections.Generic;
using System.Linq;
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

        public async Task<Tag> CreateOrFindByTagId(Tag tag)
        {
            var tagRef = _tagsCollection.Document(tag.TagId);
            var unused = await tagRef.SetAsync(tag, SetOptions.MergeAll);
            var snapshot = await tagRef.GetSnapshotAsync();
            return snapshot.ConvertTo<Tag>();
        }

        public async Task<IList<Tag>> GetAllTags()
        {
            var query = await _tagsCollection.GetSnapshotAsync();
            var tagSnapshot = query.Documents.Select(documentSnapshot => documentSnapshot.ConvertTo<Tag>()).ToList();
            return tagSnapshot;
        }

        public async Task<Tag> GetTagById(string tagId)
        {
            var reference = _tagsCollection.Document(tagId);
            var documentSnapshot = await reference.GetSnapshotAsync();
            return documentSnapshot?.ConvertTo<Tag>();
        }

        public async Task<Tag> IncrementTagCountById(string tagId, int amount)
        {
            var tag = await GetTagById(tagId);
            if (tag.ArticleCount != null)
                tag.ArticleCount += amount;
            else
                tag.ArticleCount = amount;

            return await UpdateTag(tag);
        }

        public async Task<Tag> DecrementTagCountById(string tagId, int amount)
        {
            var tag = await GetTagById(tagId);
            if (tag.ArticleCount != null)
                tag.ArticleCount -= amount;
            else
                tag.ArticleCount = 0;
            return await UpdateTag(tag);
        }

        public async Task<Tag> UpdateTag(Tag tag)
        {
            var tagRef = _tagsCollection.Document(tag.TagId);
            await tagRef.SetAsync(tag);
            var snapshot = await tagRef.GetSnapshotAsync();
            return snapshot.ConvertTo<Tag>();
        }

        public async Task<string> DeleteTag(string tagId)
        {
            var tagRef = _tagsCollection.Document(tagId);
            await tagRef.DeleteAsync();
            return tagId;
        }
    }
}