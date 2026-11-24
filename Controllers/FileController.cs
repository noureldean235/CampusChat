
using Microsoft.AspNetCore.Mvc;

namespace CampusChat.Controllers
{
    [ApiController]
    [Route("api/conversations/{conversationId:int}/files")]
    public class FileController : ControllerBase
    {
        private const long MaxFileSize = 5 * 1024 * 1024;

        private const string Flag =
            "AWAREX{UNAUTHORIZED_CONVERSATION_UPLOAD}";

        private static readonly List<UploadedFile> UploadedFiles = new();

        // POST: api/conversations/5001/files
        [HttpPost]
        public async Task<IActionResult> UploadFile(
            int conversationId,
            IFormFile file)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return Unauthorized(new { message = "Please login first" });

            var conversation = LabData.Conversations
                .FirstOrDefault(c => c.Id == conversationId);

            if (conversation == null)
                return NotFound(new { message = "Conversation not found" });

            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file uploaded" });

            if (file.Length > MaxFileSize)
                return StatusCode(413, new { message = "File is too large" });

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg" };
            var allowedContentTypes = new[] { "image/png", "image/jpeg" };

            if (!allowedExtensions.Contains(extension) ||
                !allowedContentTypes.Contains(file.ContentType.ToLowerInvariant()))
            {
                return StatusCode(415, new
                {
                    message = "Only PNG and JPG files are allowed"
                });
            }

            // Lab simulation: file bytes are kept in memory.
            // The intentionally weak checks above trust extension and Content-Type.
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var uploadedFile = new UploadedFile
            {
                Id = UploadedFiles.Count + 1,
                ConversationId = conversationId,
                UploadedBy = userId.Value,
                FileName = Path.GetFileName(file.FileName),
                ContentType = file.ContentType,
                Content = memoryStream.ToArray()
            };

            UploadedFiles.Add(uploadedFile);

            // Intentional IDOR behavior for this local CTF lab:
            // membership is not checked before attaching the file.
            var isMember = conversation.MemberIds.Contains(userId.Value);

            if (!isMember)
            {
                return Created("", new
                {
                    message = "File uploaded successfully",
                    fileId = uploadedFile.Id,
                    flag = Flag
                });
            }

            return Created("", new
            {
                message = "File uploaded successfully",
                fileId = uploadedFile.Id
            });
        }

        // GET: api/conversations/5001/files
        [HttpGet]
        public IActionResult GetFiles(int conversationId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return Unauthorized(new { message = "Please login first" });

            var conversation = LabData.Conversations
                .FirstOrDefault(c => c.Id == conversationId);

            if (conversation == null)
                return NotFound(new { message = "Conversation not found" });

            if (!conversation.MemberIds.Contains(userId.Value))
                return StatusCode(403, new { message = "Access denied" });

            var files = UploadedFiles
                .Where(f => f.ConversationId == conversationId)
                .Select(f => new
                {
                    f.Id,
                    f.FileName,
                    f.ContentType,
                    f.UploadedBy
                })
                .ToList();

            return Ok(files);
        }
    }

    public class UploadedFile
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public int UploadedBy { get; set; }
        public string FileName { get; set; } = "";
        public string ContentType { get; set; } = "";
        public byte[] Content { get; set; } = Array.Empty<byte>();
    }
}
