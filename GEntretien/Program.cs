using Microsoft.EntityFrameworkCore;
using GEntretien.Components;
using GEntretien.Domain.Interfaces;
using GEntretien.Infrastructure.Persistence;
using GEntretien.Infrastructure.Repositories;
using FluentValidation;
using GEntretien.Application.Validators;
using Blazored.FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using GEntretien.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add Authentication services
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    var googleConfig = builder.Configuration.GetSection("Authentication:Google");
    options.ClientId = googleConfig["ClientId"] ?? "";
    options.ClientSecret = googleConfig["ClientSecret"] ?? "";
    options.SaveTokens = true;
    options.Scope.Add("email");
    options.Scope.Add("profile");
});

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorization();

builder.Services.AddScoped<IVersionService, VersionService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();
builder.Services.AddScoped<IInterventionRepository, InterventionRepository>();

builder.Services.AddScoped<FluentValidation.IValidator<GEntretien.Domain.Entities.Equipment>, GEntretien.Application.Validators.EquipmentValidator>();
builder.Services.AddScoped<FluentValidation.IValidator<GEntretien.Domain.Entities.Intervention>, GEntretien.Application.Validators.InterventionValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Map authentication endpoints
app.MapGet("/login", async (HttpContext context) =>
{
    var properties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties
    {
        RedirectUri = "/"
    };
    await context.ChallengeAsync(GoogleDefaults.AuthenticationScheme, properties);
}).WithName("Login");

app.MapPost("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    context.Response.Redirect("/");
}).WithName("Logout");

app.Run();

