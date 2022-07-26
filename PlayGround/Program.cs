using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var useNewtonsoftJson = false;

if (useNewtonsoftJson)
{
    builder.Services.AddControllers().AddNewtonsoftJson();
}
else
{
    builder.Services.AddControllers();
}

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "ShiftGrid Playground",
        Description = "",
    });
});

var app = builder.Build();

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.MapControllers();

app.Run();
