using forum_service.DbContext;
using forum_service.DTO;
using forum_service.Interface;
using forum_service.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace forum_service.Service
{
    public class ForumPostService : IForumPostService
    {
        private readonly ForumDbContext _context;
        private static IHttpContextAccessor _httpContextAccessor;
        private static readonly HttpClient client = new HttpClient();

        public ForumPostService(ForumDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private static async Task<CustomUser> GetCustomUserAsync(string uid)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return null;
            }

            var token = httpContext.Request.Headers.Authorization.ToString();

            if (!string.IsNullOrEmpty(token) && token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = token.Substring("Bearer ".Length).Trim();
            }

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await client.GetAsync($"http://user-services:5000/api/Users/GetUserById?userId={uid}");

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(responseBody);
                if (responseData.ContainsKey("success") && responseData["success"] == true)
                {
                    //Console.WriteLine("User Data: " + responseData["data"]);

                    var user = new CustomUser()
                    {
                        id = responseData["data"]["id"],
                        avtUrl = responseData["data"]["userRecord"]["photoUrl"],
                        email = responseData["data"]["userRecord"]["email"],
                        NameFromEmail = responseData["data"]["nameFromEmail"],
                        Name = responseData["data"]["userRecord"]["displayName"]
                    };

                    return user;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public async Task CreatePost(ForumPostDTO forumPostDTO)
        {
            await _context.Forums.AddAsync(forumPostDTO.ToEntity());
            await _context.SaveChangesAsync();
        }

        public async Task<List<dynamic>> getPostAsync(string uid)
        {
            List<dynamic> result = new List<dynamic>();
            var list = await _context.Forums
                             .OrderByDescending(post => post.timepost)
                             .ToListAsync();
            if (list == null)
            {
                return null;
            }
            foreach (var i in list)
            {
                var likes = await _context.PostLikes.CountAsync(pl => pl.postId == i.id);
                var islike = await _context.PostLikes.FirstOrDefaultAsync(pl => pl.postId == i.id && pl.uid == uid);
                var comments_count = await _context.Comments.CountAsync(p => p.post_id == i.id);
                var user = await GetCustomUserAsync(i.uid);
                result.Add(new
                {
                    i.id,
                    i.title,
                    i.description,
                    i.category,
                    i.uid,
                    i.timepost,
                    likes,
                    comments_count,
                    i.image,
                    isLike = islike != null ? true : false,
                    user
                });
                i.comments_count = comments_count;
                await _context.SaveChangesAsync();
            }
            return result;
        }

        public async Task UpdateLike(int id, bool isLike)
        {
            var post = await _context.Forums.FirstOrDefaultAsync(e => e.id == id);
            if (post == null)
            {
                return; // or handle the case when the post is not found
            }

            if (isLike)
            {
                post.likes += 1;
                var like = new PostLikes
                {
                    postId = post.id,
                    uid = post.uid
                };
                await _context.PostLikes.AddAsync(like);
            }
            else
            {
                post.likes -= 1;
                var like = await _context.PostLikes.FirstOrDefaultAsync(e => e.postId == post.id && e.uid == post.uid);
                if (like != null)
                {
                    _context.PostLikes.Remove(like);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<dynamic> detailPost(int idPost, string uid)
        {
            var post = await _context.Forums.FirstOrDefaultAsync(e => e.id == idPost);
            if (post == null)
            {
                return null;
            }
            var user = await GetCustomUserAsync(post.uid);
            var islike = await _context.PostLikes.FirstOrDefaultAsync(pl => pl.postId == post.id && pl.uid == uid);
            var Listcomments = await _context.Comments.Where(e => e.post_id == idPost).ToListAsync();
            List<dynamic> comments = new List<dynamic>();
            List<dynamic> replies = new List<dynamic>();
            foreach (var i in Listcomments)
            {
                var reply = await GetRepliesComment(i.id);
                var oneComment = new
                {
                    comments = i,
                    user = await GetCustomUserAsync(i.uid),
                    replies = reply
                };
                comments.Add(oneComment);
            }
            return new
            {
                post,
                user,
                comments,
                isLike = islike != null ? true : false,
            };
        }

        private async Task<dynamic> GetRepliesComment(int comment_id)
        {
            var replies = await _context.CommentReplies.Where(e => e.comment_id == comment_id).ToListAsync();
            List<dynamic> result = new List<dynamic>();
            foreach (var i in replies)
            {
                var user = await GetCustomUserAsync(i.uid);
                result.Add(new
                {
                    reply = i,
                    user
                });
            }
            return result;
        }

        public async Task CreateComment(CommentDTO commentDTO)
        {
            await _context.Comments.AddAsync(commentDTO.ToEntity());
            await _context.SaveChangesAsync();
        }

        public async Task CreateReplyComment(CommentReplies commentReplies)
        {
            await _context.CommentReplies.AddAsync(commentReplies);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePost(int idPost)
        {
            var post = await _context.Forums.FirstOrDefaultAsync(e => e.id == idPost);
            if (post == null)
            {
                return;
            }

            var comments = await _context.Comments.Where(c => c.post_id == post.id).ToListAsync();
            foreach (var comment in comments)
            {
                await DeleteComment(comment.id);
            }

            _context.Forums.Remove(post);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteComment(int commentId)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(e => e.id == commentId);
            if (comment == null)
            {
                return;
            }

            var replies = await _context.CommentReplies.Where(e => e.comment_id == comment.id).ToListAsync();
            foreach (var reply in replies)
            {
                await DeleteReplyComment(reply.id);
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteReplyComment(int commentReplyId)
        {
            var reply = await _context.CommentReplies.FirstOrDefaultAsync(e => e.id == commentReplyId);
            if (reply == null)
            {
                return;
            }

            _context.CommentReplies.Remove(reply);
            await _context.SaveChangesAsync();
        }

        public async Task EditPost(int postId, string title, string description, string category, string? image)
        {
            var post = await _context.Forums.FirstOrDefaultAsync(e => e.id == postId);
            if (post == null)
            {
                return;
            }

            post.title = title;
            post.description = description;
            post.category = category;

            if (!string.IsNullOrEmpty(image))
            {
                post.image = image;
            }

            await _context.SaveChangesAsync();
        }

    }
}
