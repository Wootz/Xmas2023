using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Xmas2023;
using Xmas2023.Data;

var builder = WebApplication.CreateBuilder(args);

var domainOptions = new DomainOptions();
builder.Configuration.GetSection("DomainOptions").Bind(domainOptions);
builder.Services.AddSingleton(domainOptions);

builder.Services.AddDbContext<XmasDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

// Add services to the container.
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to 
    // the default policy
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services
    .AddRazorPages(options => {
        options.Conventions.AllowAnonymousToPage("/Index");
        options.Conventions.AllowAnonymousToPage("/Error");
    })
    .AddMicrosoftIdentityUI();

var app = builder.Build();

using (var serviceScope = app.Services.CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<XmasDbContext>();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
