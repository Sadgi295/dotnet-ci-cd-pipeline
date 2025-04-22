// Create a builder object for configuring the web application and services
var builder = WebApplication.CreateBuilder(args);

// Add services to the dependency injection container
builder.Services.AddControllers();     // Add MVC controllers support
builder.Services.AddEndpointsApiExplorer();  // Add API explorer services for API documentation
builder.Services.AddSwaggerGen();      // Add Swagger generator for API documentation UI
// Register the CounterService as a singleton to maintain a single counter across all requests
builder.Services.AddSingleton<ICounterService, CounterService>();

// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline based on environment
if (app.Environment.IsDevelopment())
{
    // Enable Swagger and SwaggerUI in development environment only
    app.UseSwagger();      // Enable Swagger endpoint for API specification
    app.UseSwaggerUI();    // Enable Swagger UI for interactive documentation
}

// Configure middleware components in the HTTP pipeline
app.UseHttpsRedirection();    // Redirect HTTP requests to HTTPS
app.UseAuthorization();       // Enable authorization capabilities
app.MapControllers();         // Map controller routes to handle requests


// Configure Swagger UI only in development environment
if (builder.Environment.IsDevelopment())
{
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

// Start the application and begin listening for requests
app.Run();
