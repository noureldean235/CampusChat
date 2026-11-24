
using Microsoft.AspNetCore.Mvc;

namespace CampusChat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConversationController : ControllerBase
    {
        // Get current logged-in user from Session
        private int? GetCurrentUserId()
        {
            return HttpContext.Session.GetInt32("UserId");
        }

        // GET: api/Conversation
        [HttpGet]
        public IActionResult GetConversations()
        {
            var userId = GetCurrentUserId();

            if (userId == null)
                return Unauthorized(new { message = "Please login first" });

            var conversations = LabData.Conversations
                .Where(c => c.MemberIds.Contains(userId.Value))
                .Select(c => new
                {
                    conversationId = c.Id,
                    members = c.MemberIds
                })
                .ToList();

            return Ok(conversations);
        }

        // GET: api/Conversation/5001/messages
        [HttpGet("{id:int}/messages")]
        public IActionResult GetMessages(int id)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
                return Unauthorized(new { message = "Please login first" });

            var conversation = LabData.Conversations
                .FirstOrDefault(c => c.Id == id);

            if (conversation == null)
                return NotFound(new { message = "Conversation not found" });

            if (!conversation.MemberIds.Contains(userId.Value))
                return StatusCode(403, new { message = "Access denied" });

            var messages = LabData.Messages
                .Where(m => m.ConversationId == id)
                .ToList();

            return Ok(messages);
        }

        // POST: api/Conversation/5001/messages
        [HttpPost("{id:int}/messages")]
        public IActionResult SendMessage(
            int id,
            [FromBody] MessageRequest request)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
                return Unauthorized(new { message = "Please login first" });

            var conversation = LabData.Conversations
                .FirstOrDefault(c => c.Id == id);

            if (conversation == null)
                return NotFound(new { message = "Conversation not found" });

            if (!conversation.MemberIds.Contains(userId.Value))
                return StatusCode(403, new { message = "Access denied" });

            if (string.IsNullOrWhiteSpace(request.Text))
                return BadRequest(new { message = "Message cannot be empty" });

            var message = new ChatMessage
            {
                Id = LabData.Messages.Count + 1,
                ConversationId = id,
                SenderId = userId.Value,
                Text = request.Text
            };

            LabData.Messages.Add(message);

            return Ok(message);
        }
    }

    public class MessageRequest
    {
        public string Text { get; set; } = "";
    }
}
