using forum_service.DTO;
using forum_service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace forum_service.Controllers
{
    public class CommentController : ControllerBase
    {
        private readonly IForumPostService _forumPostService;

        public CommentController(IForumPostService forumPostService)
        {
            _forumPostService = forumPostService;
        }

        [HttpPost("forum/create-comment")]
        [Authorize]
        public async Task<IActionResult> createComment([FromBody] CommentDTO commentDTO)
        {
            await _forumPostService.CreateComment(commentDTO);
            return Ok(new
            {
                success = true,
                message = "Comment created successfully",
            });
        }

        [HttpPost("forum/create-reply-comment")]
        [Authorize]
        public async Task<IActionResult> createReplyComment([FromBody] CommentRepliesDTO commentReplies)
        {
            await _forumPostService.CreateReplyComment(commentReplies.ToEntity());
            return Ok(new
            {
                success = true,
                message = "Reply comment created successfully",
            });
        }

        [HttpDelete("forum/delete-comment/{commentId}")]
        [Authorize]
        public async Task<IActionResult> deleteComment(int commentId)
        {
            await _forumPostService.DeleteComment(commentId);
            return Ok(new
            {
                success = true,
                message = "Comment deleted successfully",
            });
        }

        [HttpDelete("forum/delete-reply-comment/{commentReplyId}")]
        [Authorize]
        public async Task<IActionResult> deleteReplyComment(int commentReplyId)
        {
            await _forumPostService.DeleteReplyComment(commentReplyId);
            return Ok(new
            {
                success = true,
                message = "Reply comment deleted successfully",
            });
        }
    }
}
