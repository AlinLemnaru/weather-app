using RomanianWeather.API.Services;
using RomanianWeather.API.Interfaces;
using RomanianWeather.API.Providers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Register WeatherService and MeteoApiClient
builder.Services.AddHttpClient<IWeatherApiClient, MeteoApiClient>();
builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<WeatherService>();
builder.Services.AddScoped<WeatherAnalysisService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapControllers();

app.Run();
