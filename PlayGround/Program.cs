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
app.UseSwaggerUI();

app.MapControllers();

app.Run();
