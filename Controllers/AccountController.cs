using BlogV2.Services;
using BlogV2.ViewModels;
using BlogV2.Data;
using BlogV2.Extensions;
using BlogV2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

namespace BlogV2.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly BlogDataContext _context;


        public AccountController(TokenService tokenService, BlogDataContext context)
        {
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("v1/accounts")]
        public async Task<IActionResult> Post(
            [FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                Slug = model.Email.Replace("@", "-").Replace(".", "-")
            };

            var password = PasswordGenerator.Generate(25);
            user.PasswordHash = PasswordHasher.Hash(password);

            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                return Ok(new ResultViewModel<dynamic>(new
                {
                    user = user.Email,
                    password
                }));
            }
            catch (DbUpdateException)
            {
                return StatusCode(400, new ResultViewModel<string>("05X99 - Este E-mail já está cadastrado"));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<string>("05X04 - Falha interna no servidor"));
            }
        }

        [HttpPost("v1/accounts/login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginViewModel model
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            var user = await _context
                .Users
                .Include("Roles")
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == model.Email);

            if (user == null)
                return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos"));

            if (!PasswordHasher.Verify(user.PasswordHash, model.Password))
                return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos"));

            try
            {
                var token = _tokenService.GenerateToken(user);
                return Ok(new ResultViewModel<string>(token, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<string>("05X04 - Falha interna no servidor"));
            }
        }

        [Authorize(Roles = "user")]
        [HttpGet("v1/user")]
        public IActionResult GetUser() => Ok(User.Identity.Name);

        [Authorize(Roles = "author")]
        [HttpGet("v1/author")]
        public IActionResult GetAuthor() => Ok(User.Identity.Name);

        [Authorize(Roles = "admin")]
        [HttpGet("v1/admin")]
        public IActionResult GetAdmin() => Ok(User.Identity.Name);
    }
}
