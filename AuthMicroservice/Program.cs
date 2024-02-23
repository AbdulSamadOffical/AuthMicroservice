using AuthMicroservice;
using AuthMicroservice.Middleware;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureServices(builder.Configuration);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); // Configure AutoMapper here


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<CustomUnauthorizedMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();
//app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});


app.UseOcelot().Wait();
app.Run();
