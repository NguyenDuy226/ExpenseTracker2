using System.Text;
using ExpenseTracker.API.Data;
using ExpenseTracker.API.Models;
using ExpenseTracker.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("defaultConnection");
builder.Services.AddControllers();
//Add dbContext
builder.Services.AddDbContext<AppDbContext>(async options => options
.UseSqlServer(connectionString)
.UseSeeding ((dbContext, _) =>
{
    if (!dbContext.Set<Category>().Any())
        {
            dbContext.Set<Category>().AddRange(
                new Category { Name = "Ăn uống", Type = "Chi" },
                new Category { Name = "Di chuyển", Type = "Chi" },
                new Category { Name = "Lương", Type = "Thu" },
                new Category { Name = "Giải trí", Type = "Chi" } 
            );
            dbContext.SaveChanges();
        }
})
.UseAsyncSeeding (async (dbContext, _, cancel) =>
{
    if (!await dbContext.Set<Category>().AnyAsync(cancel))
        {
            await dbContext.Set<Category>().AddRangeAsync(
                new Category { Name = "Ăn uống", Type = "Chi" },
                new Category { Name = "Di chuyển", Type = "Chi" },
                new Category { Name = "Lương", Type = "Thu" },
                new Category { Name = "Giải trí", Type = "Chi" }
            );
            await dbContext.SaveChangesAsync(cancel);
        }
})
);

//Add Identity
builder.Services.AddIdentity<AppUser, IdentityRole> (options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

//Add authentication and authoration
builder.Services.AddScoped<createJWT>();

var jwtSetting = builder.Configuration["JWT"];
var keyJson = builder.Configuration["JWT:Key"];
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyJson ?? string.Empty));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = key,
        ClockSkew = TimeSpan.Zero

    };
});
builder.Services.AddAuthorization();

//Add swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Name = "Authorization"
    });
    options.AddSecurityRequirement(require => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", require)] = []
    });
});

//Add CORS
var local = "local";
builder.Services.AddCors(options =>
{
     options.AddPolicy(name: local, 
                        policy =>
                        {
                            policy.WithOrigins("http://localhost:4200")
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                        });
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
    
app.UseHttpsRedirection();
app.UseCors(local);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();