using Icon.Api.Common.ModelBinding;
using Icon.Api.Extensions.DependencyInjection;
using Icon.Api.Extensions.Web.Versioning;
using Icon.Api.Middlewares;
using Icon.Api.Modules;
using Icon.SharedKernel.Abstractions;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Register Modules
builder.Services.AddModule(new CoreModule());
builder.Services.AddModule(new DataModule(builder.Configuration));
builder.Services.AddModule(new AuthenticationModule(builder.Configuration));

builder.Services.AddApiExplorerVersioning();
builder.Services.AddControllers(options => options.ModelBinderProviders.Insert(0, new UlidModelBinderProvider()));

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Icon API",
        Version = "v1",
        Description = "A ticket management API using DDD with ASP.NET Identity"
    });
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Initialize Database
using (IServiceScope scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
    await initializer.InitializeAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Icon API v1");
        options.DocumentTitle = "Icon API Docs";
    });
}

app.UseExceptionHandler(_ => { });
app.UseCors("AllowFrontend");

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
