
namespace Blog.Models;

public class Tag : BaseModel
{
    public List<Post> Posts { get; set; }
}