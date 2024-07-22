using IdentityAPI.Data;
using IdentityAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();
builder.Services.AddControllers();

// Add CORS services and configure the policy.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        builder => builder.WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod());
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnectionString"));
    
    // Add the OpenIddict entity sets.
    options.UseOpenIddict();
});

builder.Services.Configure<IdentityOptions>(options =>
{
    options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
    options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
    options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;

    options.User.RequireUniqueEmail = true;

    // Configure more if necessary.
});

// OpenID Connect server configuration.
builder.Services.AddOpenIddict()
    .AddCore(options => options.UseEntityFrameworkCore().UseDbContext<ApplicationDbContext>())
    .AddServer(options =>
    {
        // Enable the required endpoints.
        options.SetTokenEndpointUris("/connect/token");
        options.SetUserinfoEndpointUris("/connect/userinfo");

        options.AllowPasswordFlow();
        options.AllowRefreshTokenFlow();

        // Add all auth flows you want to support.
        // Supported flows are:
        // - Authorization code flow
        // - Client credentials flow
        // - Device code flow
        // - Implicit flow
        // - Password flow
        // - Refresh token flow

        // Custom auth flows are also supported.
        options.AllowCustomFlow("custom_flow_name");

        // Using reference tokens means the actual access and refresh tokens
        // are stored in the database and different tokens, referencing the actual
        // tokens (in the db), are used in request headers. The actual tokens are not
        // made public.
        options.UseReferenceAccessTokens();
        options.UseReferenceRefreshTokens();

        // Register your scopes - Scopes are a list of identifiers used to specify
        // what access privileges are requested.
        options.RegisterScopes(OpenIddictConstants.Permissions.Scopes.Email,
            OpenIddictConstants.Permissions.Scopes.Profile,
            OpenIddictConstants.Permissions.Scopes.Roles);

        // Set the lifetime of your tokens.
        options.SetAccessTokenLifetime(TimeSpan.FromMinutes(30));
        options.SetRefreshTokenLifetime(TimeSpan.FromDays(7));

        // Register signing and encryption details.
        options.AddDevelopmentEncryptionCertificate()
            .AddDevelopmentSigningCertificate();
        
        // Register ASP.NET Core host and configuration options.
        options.UseAspNetCore().EnableTokenEndpointPassthrough();
    })
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = OpenIddictConstants.Schemes.Bearer;
    options.DefaultChallengeScheme = OpenIddictConstants.Schemes.Basic;
});

builder.Services.AddIdentity<User, Role>()
    .AddSignInManager()
    .AddUserStore<UserStore>()
    .AddRoleStore<RoleStore>()
    .AddUserManager<UserManager<User>>();

builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5001");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

// Use the configured CORS policy.
app.UseCors("AllowAngularApp");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
context.Database.EnsureCreated();

var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
var existingClientApp = manager.FindByClientIdAsync("default-client").GetAwaiter().GetResult();
if (existingClientApp == null)
{
    manager.CreateAsync(new OpenIddictApplicationDescriptor
    {
        ClientId = "default-client",
        ClientSecret = "this-is-a-long-secret-but-should-be-longer",
        DisplayName = "Default client application",
        Permissions =
        {
            OpenIddictConstants.Permissions.Endpoints.Token,
            OpenIddictConstants.Permissions.GrantTypes.Password
        }
    }).GetAwaiter().GetResult();
}

app.Run();
