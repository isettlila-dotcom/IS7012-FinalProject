using IS7012_FinalProject.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var clerkAuthority = builder.Configuration["Clerk:Authority"]
    ?? throw new InvalidOperationException("Missing Clerk:Authority configuration.");

var clerkClientId = builder.Configuration["Clerk:ClientId"]
    ?? throw new InvalidOperationException("Missing Clerk:ClientId configuration.");

var clerkClientSecret = builder.Configuration["Clerk:ClientSecret"]
    ?? throw new InvalidOperationException("Missing Clerk:ClientSecret configuration.");

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Employees");
    options.Conventions.AuthorizeFolder("/PayrollRecords");
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme =
            CookieAuthenticationDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddOpenIdConnect(options =>
    {
        options.Authority = clerkAuthority;

        options.ClientId = clerkClientId;
        options.ClientSecret = clerkClientSecret;

        options.ResponseType = "code";
        options.UsePkce = true;

        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;

        // Preserve standard OIDC claim names such as "sub".
        options.MapInboundClaims = false;

        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapGet("/login", async context =>
{
    await context.ChallengeAsync(
        OpenIdConnectDefaults.AuthenticationScheme,
        new AuthenticationProperties
        {
            RedirectUri = "/"
        });
});

app.MapGet("/logout", async context =>
{
    await context.SignOutAsync(
        CookieAuthenticationDefaults.AuthenticationScheme);

    context.Response.Redirect("/");
});

app.MapRazorPages()
    .WithStaticAssets();

app.Run();