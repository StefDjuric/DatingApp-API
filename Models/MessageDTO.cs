namespace DatingApp_API.Models
{
  
        public class MessageDTO
        {
            public int Id { get; set; }
            public required string SenderUsername { get; set; }
            public int SenderId { get; set; }
            public required string SenderPhotoUrl { get; set; }
            public required string RecipientUsername { get; set; }
            public int RecipientId { get; set; }
            public required string RecipientPhotoUrl { get; set; }
            public required string Content { get; set; }
            public DateTime? DateRead { get; set; }
            public DateTime DateSent { get; set; } = DateTime.UtcNow;
        
        

    }
}
