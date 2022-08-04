namespace api.core.Entities;

public class Messages
{
    public int Id { get; set; }

    public int SenderId { get; set; }

    public string SenderName { get; set; }

    public Users Sender { get; set; }

    public int RecipientId { get; set; }

    public string RecipientName { get; set; }

    public Users Recipient { get; set; }

    public string Content { get; set; }

    public DateTime MessageSent { get; set; } = DateTime.UtcNow;

    public bool SenderDeleted { get; set; }
}