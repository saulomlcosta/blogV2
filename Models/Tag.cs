
namespace BlogV2.Models;

public class Tag : BaseModel
{
    public List<Post> Posts { get; set; }
}