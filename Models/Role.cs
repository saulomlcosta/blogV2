namespace BlogV2.Models;

public class Role : BaseModel
{
    public IList<User> Users { get; set; }
}