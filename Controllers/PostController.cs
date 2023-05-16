using BlogV2.Data;
using BlogV2.Models;
using BlogV2.ViewModels;
using BlogV2.ViewModels.Posts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogV2.Controllers
{
    [ApiController]
    public class PostController : ControllerBase
    {
        [HttpGet("v1/posts")]
        public async Task<IActionResult> GetAsync(
            [FromServices] BlogDataContext context,
            [FromQuery] int Page = 0,
            [FromQuery] int PageSize = 25
        )
        {
            try
            {
                var count = await context.Posts.AsNoTracking().CountAsync();

                var posts = await context
                .Posts
                .AsNoTracking()
                .Include(x => x.Category)
                .Include(x => x.Author)
                .Select(x => new ListPostsViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Slug = x.Slug,
                    LastUpdateDate = x.LastUpdateDate,
                    Category = x.Category.Name,
                    Author = $"{x.Author.Name} ({x.Author.Email})"
                })
                .Skip(Page * PageSize)
                .Take(PageSize)
                .OrderByDescending(x => x.LastUpdateDate)
                .ToListAsync();

                return Ok(new ResultViewModel<dynamic>(new
                {
                    total = count,
                    page = Page,
                    pageSize = PageSize,
                    posts
                }));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<Post>("05X04 - Falha interna no servidor"));
            }
        }

        [HttpGet("v1/posts/{id:int}")]
        public async Task<IActionResult> DetailsAsync(
            [FromServices] BlogDataContext context,
            [FromRoute] int id
        )
        {
            try
            {
                var post = await context
                .Posts
                .AsNoTracking()
                .Include(x => x.Author)
                .ThenInclude(x => x.Roles)
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == id);

                if (post == null)
                    return NotFound(new ResultViewModel<Post>("Conteúdo não encontrado"));

                return Ok(new ResultViewModel<Post>(post));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<Post>("05X04 - Falha interna no servidor"));
            }
        }

        [HttpGet("v1/posts/category/{category}")]
        public async Task<IActionResult> GetByCategoryAsync(
            [FromServices] BlogDataContext context,
            [FromRoute] string category,
            [FromQuery] int Page = 0,
            [FromQuery] int PageSize = 25
        )
        {
            try
            {
                var count = await context.Posts.AsNoTracking().CountAsync();

                var posts = await context
                .Posts
                .AsNoTracking()
                .Include(x => x.Author)
                .Include(x => x.Category)
                .Where(x => x.Category.Slug == category)
                .Select(x => new ListPostsViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Slug = x.Slug,
                    LastUpdateDate = x.LastUpdateDate,
                    Category = x.Category.Name,
                    Author = $"{x.Author.Name} ({x.Author.Email})"
                })
                .Skip(Page * PageSize)
                .Take(PageSize)
                .OrderByDescending(x => x.LastUpdateDate)
                .ToListAsync();

                return Ok(new ResultViewModel<dynamic>(new
                {
                    total = count,
                    page = Page,
                    pageSize = PageSize,
                    posts
                }));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<Post>("05X04 - Falha interna no servidor"));
            }
        }
    }
}