using System.Text.Json.Serialization;
using Icon.Api.Extensions.DependencyInjection;
using Icon.Api.Modules;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddModule(new CoreModule());
builder.Services.AddModule(new DataModule(builder.Configuration));

var app = builder.Build();

app.MapControllers();
app.Run();
