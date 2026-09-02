namespace todo_momo_api.Extensions;

internal static class WebApplicationCorsExtensions
{
    private const string CorsPolicyName = "AllowAngularApp";

    /// <summary>
    /// Configures Cross-Origin Resource Sharing (CORS) policies for the WebApplicationBuilder instance.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddCorsPolicies(this WebApplicationBuilder builder)
    {
        // Configure CORS policies for the application. This allows cross-origin requests from different domains.
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicyName, policy =>
            {
                // Restrict the CORS policy to allow requests only from the specified origin (e.g., "http://localhost:4200").
                policy.WithOrigins("http://localhost:4200")
                      // Allow any HTTP method (GET, POST, PUT, DELETE, PATCH) for CORS requests from the specified origin.
                      .WithMethods("GET", "POST", "PUT", "DELETE", "PATCH")
                      // Allow any HTTP header for CORS requests from the specified origin.
                      .AllowAnyHeader();
                // Uncomment this line if you want to allow credentials (cookies, authorization headers, etc.) in CORS requests.
                // .AllowCredentials();
            });
        });
        return builder;
    }

    /// <summary>
    /// Configures Cross-Origin Resource Sharing (CORS) for the WebApplication instance.
    /// </summary>
    /// <param name="app">The WebApplication instance to configure CORS for.</param>
    /// <returns>The WebApplication instance with CORS configured.</returns>
    public static WebApplication ConfigureCors(this WebApplication app)
    {
        app.UseCors(CorsPolicyName);
        return app;
    }
}
