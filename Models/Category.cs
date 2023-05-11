namespace BlogV2.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public IList<Post> Posts { get; set; }
    /* Por ser um Ilist, sempre será inicializada, 
    ela vem um array vazio, mas não um objeto nulo.*/
}