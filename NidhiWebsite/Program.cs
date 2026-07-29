using Microsoft.EntityFrameworkCore;
using NidhiWebsite.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMemoryCache();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;

    if (!string.IsNullOrEmpty(path) &&
        !path.EndsWith(".html") &&
        !path.Contains("."))
    {
        var htmlFile = Path.Combine(
            app.Environment.WebRootPath,
            path.TrimStart('/') + ".html"
        );

        if (File.Exists(htmlFile))
        {
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync(htmlFile);
            return;
        }
    }

    await next();
});
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;

    if (!string.IsNullOrEmpty(path) &&
        !path.EndsWith(".html") &&
        !path.Contains("."))
    {
        var htmlFile = Path.Combine(
            app.Environment.WebRootPath,
            path.TrimStart('/') + ".html"
        );

        if (File.Exists(htmlFile))
        {
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync(htmlFile);
            return;
        }
    }

    await next();
});

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
