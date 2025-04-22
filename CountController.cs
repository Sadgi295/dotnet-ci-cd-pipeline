using Microsoft.AspNetCore.Mvc;

/// <summary>
/// API controller that handles requests related to counting operations
/// </summary>
[ApiController] 
[Route("[controller]")]  // Routes requests to this controller using the controller name as route prefix
public class CountController : ControllerBase
{
    // Dependency injected counter service used for incrementing operations
    private readonly ICounterService _counterService;

    /// <summary>
    /// Constructor that receives the counter service via dependency injection
    /// </summary>
    /// <param name="counterService">The service handling counter operations</param>
    public CountController(ICounterService counterService)
    {
        _counterService = counterService;
    }

    /// <summary>
    /// Handles HTTP GET requests by incrementing the counter and returning the new value
    /// </summary>
    /// <returns>HTTP 200 OK response containing the incremented counter value</returns>
    [HttpGet]  // Maps this method to HTTP GET requests
    public IActionResult Get()
    {
        // Increment the counter and return its new value with HTTP 200 OK status
        return Ok(_counterService.IncrementAndGet());
    }
}
