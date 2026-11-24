
namespace CampusChat
{
    public class Conversation
    {
        public int Id { get; set; }
        public List<int> MemberIds { get; set; } = new();
    }

    public class ChatMessage
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public int SenderId { get; set; }
        public string Text { get; set; } = "";
    }

    public static class LabData
    {
        public static List<Conversation> Conversations = new()
        {
            new Conversation
            {
                Id = 5001,
                MemberIds = new List<int> { 101, 202 }
            },
            new Conversation
            {
                Id = 5002,
                MemberIds = new List<int> { 202, 303 }
            }
        };

        public static List<ChatMessage> Messages = new()
        {
            new ChatMessage
            {
                Id = 1,
                ConversationId = 5001,
                SenderId = 101,
                Text = "Hi Omar!"
            },
            new ChatMessage
            {
                Id = 2,
                ConversationId = 5001,
                SenderId = 202,
                Text = "Hello Ahmed!"
            },
            new ChatMessage
            {
                Id = 3,
                ConversationId = 5002,
                SenderId = 202,
                Text = "Hi Sara!"
            }
        };
    }
}
