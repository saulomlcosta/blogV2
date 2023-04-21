namespace Blog.Models;

public class Category : BaseModel
{
    public IList<Post> Posts { get; set; }
}