using Asp.Versioning;
using EmployeeManagement.Api.Documentation;
using EmployeeManagement.Api.Middleware;
using EmployeeManagement.Application;
using EmployeeManagement.Infrastructure;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------------------------
// Request handling & API configuration
// ---------------------------------------------------------------------------------------------
builder.Services.AddControllers();

// URL segment versioning: /api/v1/...
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddMvc()
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
})
// Declares OpenAPI documents and Bearer security scheme transformer for Swagger UI
.AddOpenApi(options => options.Document.AddDocumentTransformer<BearerSecuritySchemeTransformer>());

// ---------------------------------------------------------------------------------------------
// Core Services: Exception Handling, Diagnostics, and Health Checks
// ---------------------------------------------------------------------------------------------
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();
builder.Services.AddAuthorization();

// ---------------------------------------------------------------------------------------------
// Layer Services: Application & Infrastructure
// ---------------------------------------------------------------------------------------------
builder.Services.AddApiRateLimiting(builder.Configuration);
builder.Services.AddApiOutputCaching(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Do not advertise the server implementation in HTTP response headers
builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

var app = builder.Build();

// ---------------------------------------------------------------------------------------------
// HTTP Request Pipeline
// ---------------------------------------------------------------------------------------------
var browserRenderedPaths = Array.Empty<string>();

if (app.Environment.IsDevelopment())
{
    // One OpenAPI document per API version
    app.MapOpenApi().WithDocumentPerVersion().AllowAnonymous();

    // Swagger UI over those documents
    app.MapApiDocumentation().AllowAnonymous();

    browserRenderedPaths = [ApiDocumentationExtensions.PathPrefix];
}

// Forwarded headers for reverse proxy hosting (Docker / Nginx / Kubernetes)
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseSecurityHeaders(browserRenderedPaths);
app.UseAuthorization();
app.UseApiRateLimiting();

// Cached responses are still throttled: rate limiting runs before the cache is consulted.
app.UseApiOutputCaching();

app.MapControllers();

// Liveness probe: anonymous endpoint for orchestrators and load balancers
app.MapHealthChecks("/healthz").AllowAnonymous();

app.Run();

/// <summary>
/// Exposed so that integration tests can bootstrap the API with <c>WebApplicationFactory</c>.
/// </summary>
public partial class Program;
