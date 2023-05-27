using System.IO.Compression;
using System.Text;
using System.Text.Json.Serialization;
using BlogV2.Data;
using BlogV2.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BlogV2.Extensions;

public static class AppExtension
{
    public static void LoadConfiguration(this WebApplicationBuilder builder)
    {
        Configuration.JwtKey = builder.Configuration.GetValue<string>("JwtKey")!;
        Configuration.ApiKeyName = builder.Configuration.GetValue<string>("ApiKeyName")!;
        Configuration.ApiKeyValue = builder.Configuration.GetValue<string>("ApiKeyValue")!;

        var smtp = new Configuration.SmtpConfiguration();
        builder.Configuration.GetSection("Smtp").Bind(smtp);
        Configuration.Smtp = smtp;
    }

    public static void ConfigureAuthentication(this WebApplicationBuilder builder)
    {
        var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);
        builder.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });
    }

    public static void ConfigureMvc(this WebApplicationBuilder builder)
    {
        builder.Services.AddResponseCompression(options =>
        {
            //options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
            //options.Providers.Add<CustomCompressionProvider>();
        });
        builder.Services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
        builder.Services.AddMemoryCache();
        builder.Services
            .AddControllers()
            .ConfigureApiBehaviorOptions(
                options => options.SuppressModelStateInvalidFilter = true)
            .AddJsonOptions(x =>
            {
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                x.JsonSerializerOptions.DefaultIgnoreCondition
                    = JsonIgnoreCondition.WhenWritingDefault; // Caso o objeto venha nulo, não renderizar.
            });
    }

    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<BlogDataContext>(
            options =>
                options.UseSqlServer(connectionString));

        builder.Services.AddTransient<TokenService>();
        builder.Services.AddTransient<EmailService>();
    }
}