namespace BlogV2.Models;

public class Category : BaseModel
{
    public IList<Post> Posts { get; set; }
}