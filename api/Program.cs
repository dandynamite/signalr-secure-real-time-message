using SignalRTest.Hubs;
using SignalRTest.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddSignalR()
    .AddAzureSignalR(builder.Configuration.GetValue<string>("AppConfig:SignalRConnection"));

builder.Services.AddScoped(x => new CosmosService(builder.Configuration.GetValue<string>("AppConfig:CosmosConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", b => b
        .WithOrigins(builder.Configuration.GetValue<string>("AppConfig:SPAUrl"))
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddAuthentication(options => options.DefaultScheme = MyCustomAuthSchemeOptions.Schema)
    .AddScheme<MyCustomAuthSchemeOptions, TicketValidationAuthHandler>(MyCustomAuthSchemeOptions.Schema, options => { });

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("CorsPolicy");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chatHub");

app.Run();
