using Microsoft.EntityFrameworkCore;
using Reddit;
using Reddit.Repositories;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
     .AddJsonOptions(options =>
     {
         options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
     });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options => {
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("MSSQL"));
    options.LogTo(Console.WriteLine, LogLevel.Information);
    options.UseLazyLoadingProxies();
});
builder.Services.AddScoped<IPostsRepository, PostsRepository>();
// Scoped, Singleton, Transient

builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()));

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();
app.MapControllers();

// Map health checks endpoint
app.MapHealthChecks("/health");

// Map root path to return "Running" status
app.MapGet("/", () => Results.Ok("Running Reddit API"));

app.Run();
