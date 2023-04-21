using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Models;

public class Role : BaseModel
{
    public IList<User> Users { get; set; }
}