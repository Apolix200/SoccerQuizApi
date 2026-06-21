using Microsoft.AspNetCore.HttpOverrides;
using SoccerQuizApi.Helper;
using SoccerQuizApi.Models;
using SoccerQuizApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<SoccerQuizDBSettings>(
builder.Configuration.GetSection("SoccerQuizStoreDatabase"));

builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<QuizService>();
builder.Services.AddSingleton<ResultService>();
builder.Services.AddSingleton<AdminHelper>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS (ONLY ONCE)
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "https://vacdeakvarquiz.com",
                "https://www.vacdeakvarquiz.com"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Nginx / reverse proxy
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;
});

var app = builder.Build();

// Proxy headers FIRST
app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();

// IMPORTANT: only ONE CORS middleware
app.UseCors("CorsPolicy");

app.UseAuthorization();
app.MapControllers();

app.Run();