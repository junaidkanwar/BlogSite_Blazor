using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BlogSite.Data;
using Microsoft.JSInterop;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Register services
builder.Services.AddScoped<UserService>();
builder.Services.AddSingleton<CommentService>();
builder.Services.AddSingleton<ContactMessageService>();
builder.Services.AddSingleton<BlogService>();
builder.Services.AddSingleton<UserState>();

// Configure other services and middleware

var app = builder.Build(); // Call Build() only once

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

// Configure other middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
