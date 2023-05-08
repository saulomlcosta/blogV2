using BlogV2.Services;
using BlogV2.ViewModels;
using BlogV2.ViewModels.Accounts;
using BlogV2.Data;
using BlogV2.Extensions;
using BlogV2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;
using System.Text.RegularExpressions;

namespace BlogV2.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly EmailService _emailService;
        private readonly BlogDataContext _context;


        public AccountController(TokenService tokenService, BlogDataContext context, EmailService emailService)
        {
            _tokenService = tokenService;
            _context = context;
            _emailService = emailService;
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

                _emailService.Send(
                    user.Name,
                    user.Email,
                    "Bem vindo ao blog!",
                    $"Sua senha é <strong>{password}</strong"
                );

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
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<string>("05X04 - Falha interna no servidor"));
            }
        }

        [Authorize]
        [HttpPost("v1/accounts/upload-image")]
        public async Task<IActionResult> UploadImage(
            [FromBody] UploadImageViewModel model
        )
        {
            var fileName = $"{Guid.NewGuid().ToString()}.jpg";
            var data = new Regex(@"^data:image\/[a-z]+;base64,").Replace(model.Base64Image, "");
            var bytes = Convert.FromBase64String(data);

            try
            {
                await System.IO.File.WriteAllBytesAsync($"wwwroot/images/{fileName}", bytes);
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<string>("05X04 - Falha interna no servidor"));
            }

            var user = await _context
                .Users
                .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

            if (user == null)
                return NotFound(new ResultViewModel<User>("Usuário não encontrado"));

            user.Image = $"https://localhost:0000/images/{fileName}";

            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<string>("05X04 - Falha interna no servidor"));
            }

            return Ok(new ResultViewModel<string>("Imagem alterada com sucesso!", null));
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
