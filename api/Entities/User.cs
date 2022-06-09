namespace api.Entities;

public class User
{
    public int Id { get; set; }

    public string UserName { get; set; }

    public string PhotoUrl { get; set; }

    public ICollection<GameInfo> Games { get; set; }
}