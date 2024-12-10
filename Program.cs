using GCP.Demo;
using GCP.Demo.Helpers;
using Google.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);
var binDirectory = Directory.GetCurrentDirectory();
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", $"{binDirectory}//training-415008-12146157e296.json");

// Add services to the container.
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddSingleton<EmailPublisherService>();
//builder.Services.AddSingleton<PublisherService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


    app.UseSwagger();
    app.UseSwaggerUI();


//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
