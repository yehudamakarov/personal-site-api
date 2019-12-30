using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Types;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repository
{
    public class BlogPostRepository : RepositoryBase, IBlogPostRepository
    {
        private readonly CollectionReference _blogPostsCollection;

        public BlogPostRepository(IConfiguration configuration) : base(configuration)
        {
            _blogPostsCollection = Db.Collection("blog-posts");
        }

        public async Task<IList<BlogPost>> GetAllBlogPosts()
        {
            var querySnapshot = await _blogPostsCollection.GetSnapshotAsync();
            var results = querySnapshot.Documents.Select(documentSnapshot => documentSnapshot.ConvertTo<BlogPost>())
                .ToList();
            return results;
        }

        public async Task<BlogPost> AddBlogPost(BlogPost blogPost)
        {
            // add document to collection to get a ref
            var blogPostRef = await _blogPostsCollection.AddAsync(blogPost);
            // get a snapshot of the ref
            var snapshot = await blogPostRef.GetSnapshotAsync();
            // convert to model
            return snapshot.ConvertTo<BlogPost>();
        }

        public async Task<BlogPost> GetBlogPostById(string blogPostId)
        {
            var reference = _blogPostsCollection.Document(blogPostId);
            var documentSnapshot = await reference.GetSnapshotAsync();
            return documentSnapshot?.ConvertTo<BlogPost>();
        }

        public async Task<BlogPost> UpdateBlogPost(BlogPost blogPost)
        {
            var blogPostRef = _blogPostsCollection.Document(blogPost.Id);
            await blogPostRef.SetAsync(blogPost);
            var snapshot = await blogPostRef.GetSnapshotAsync();
            return snapshot.ConvertTo<BlogPost>();
        }

        public async Task<IList<BlogPost>> GetBlogPostsByTagId(string tagId)
        {
            var snapshot = await _blogPostsCollection.WhereArrayContains(nameof(BlogPost.TagIds), tagId).GetSnapshotAsync();
            return snapshot.Documents.Select(documentSnapshot => documentSnapshot.ConvertTo<BlogPost>()).ToList();
        }


        public async Task<IList<BlogPost>> GetBlogPostsByProjectId(string projectId)
        {
            var querySnapshot = await _blogPostsCollection.WhereEqualTo(nameof(BlogPost.ProjectId), projectId)
                .GetSnapshotAsync();
            var results = querySnapshot.Documents.Select(documentSnapshot => documentSnapshot.ConvertTo<BlogPost>())
                .ToList();
            return results;
        }
    }
}