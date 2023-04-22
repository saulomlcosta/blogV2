namespace BlogV2.Models;

public class User : BaseModel
{
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Image { get; set; }
    public string Bio { get; set; }

    public IList<Post> Posts { get; set; }
    public IList<Role> Roles { get; set; }
}