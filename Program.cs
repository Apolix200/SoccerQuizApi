using Microsoft.AspNetCore.HttpOverrides;
using SoccerQuizApi.Models;
using SoccerQuizApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<SoccerQuizDBSettings>(
    builder.Configuration.GetSection("SoccerQuizStoreDatabase"));

builder.Services.AddSingleton<UserService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//React Cors
builder.Services.AddCors();

//Nginx Proxy
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

var app = builder.Build();

//Nginx Proxy
app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

//React Cors
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3000"));

app.UseAuthorization();

app.MapControllers();

app.Run();

