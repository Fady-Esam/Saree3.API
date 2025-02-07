using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Saree3.API.Domains;
using Saree3.API;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Saree3.API.Models;
using Saree3.API.BL.Classes;
using Saree3.API.BL.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<JWTModel>(
    builder.Configuration.GetSection("JWT")
);

builder.Services.AddHttpClient();

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//Configure the DbContext with a connection string
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(connectionString).ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning)));
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//// Configure Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDBContext>()
    .AddDefaultTokenProviders();
builder.Services.AddScoped<IAuthService, AuthService>();
//builder.Services.AddScoped<IHomeService, HomeService>();

// Add other services like controllers, session, etc.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddSession();
/*
builder.Services.AddMemoryCache(); // Register IMemoryCache
builder.Services.AddDistributedSqlServerCache(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.SchemaName = "dbo";
    options.TableName = "Cache";
});
builder.Services.AddDistributedMemoryCache();
*/

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JWT").Get<JWTModel>();
    options.SaveToken = false;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSettings!.Key)),
        ClockSkew = TimeSpan.Zero, // Optional: Set to zero to avoid clock skew issues
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience
    };
});
//.AddGoogle(options =>
//{
//    options.ClientId = "rOkBTRTHAg0jDAXH2M927hFrOZwiiuweotiu221346546aqBosq8Cr-7DHBzJHk";
//    options.ClientSecret = "rOkBT88797wteuywiuetXH2M927hFrOZwiiuweotiu221346546aqBosq8Cr-7DHBzJHk";
//}).AddFacebook(options =>
//{
//    options.ClientId = "OkBTRTHAg0jDAXH2M927hFrOZw329_aqBosq8Cr-7DHBzJHk";
//    options.ClientSecret = "kiopreqBTRTHAg0jDAXH2M927hFrOZw329_aqBosq8Cr-7DHBzJHk";
//});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
//app.UseSession();
app.MapControllers();

app.Run();
