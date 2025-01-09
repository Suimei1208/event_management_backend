using forum_service.DTO;
using forum_service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace forum_service.Controllers
{
    public class ForumController : ControllerBase
    {
        private readonly IForumPostService _forumPostService;

        public ForumController(IForumPostService forumPostService)
        {
            _forumPostService = forumPostService;
        }

        [HttpPost("forum/create-post")]
        [Authorize]
        public async Task<IActionResult> createPost([FromBody] ForumPostDTO forumPostDTO)
        {
            await _forumPostService.CreatePost(forumPostDTO);
            return Ok(new
            {
                success = true,
                message = "Post created successfully",
            });
        }

        [HttpGet("forum/get-post/{uid}")]
        [Authorize]
        public async Task<IActionResult> getPost(string uid)
        {
            var result = await _forumPostService.getPostAsync(uid);
            return Ok(new
            {
                success = true,
                message = "Get post successfully",
                data = result
            });
        }

        [HttpPut("forum/update-like/{id}/{isLike}")]
        [Authorize]
        public async Task<IActionResult> updateLike(int id, bool isLike)
        {
            await _forumPostService.UpdateLike(id, isLike);
            return Ok(new
            {
                success = true,
                messge = "like +1"
            });
        }
        [HttpGet("forum/detail-post/{idPost}/{uid}")]
        [Authorize]
        public async Task<IActionResult> detailPost(int idPost, string uid)
        {
            var result = await _forumPostService.detailPost(idPost,uid);
            return Ok(new
            {
                success = true,
                message = "Get detail post successfully",
                data = result
            });
        }

    }
}
