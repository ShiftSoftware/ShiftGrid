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

var app = builder.Build();

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.MapControllers();

app.Run();
