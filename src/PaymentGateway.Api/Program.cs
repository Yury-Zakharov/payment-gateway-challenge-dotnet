using System.Net;

using PaymentGateway.Api.Clients;
using PaymentGateway.Api.Filters;
using PaymentGateway.Api.HealthChecks;
using PaymentGateway.Api.Models.Configuration;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Observability;
using PaymentGateway.Api.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<BankClientOptions>(
    builder.Configuration.GetSection("BankClient"));

builder.Services.AddHttpClient<IBankClient, BankClient>();

builder.Services.AddSingleton<IPaymentsRepository, InMemoryPaymentRepository>();
builder.Services.AddScoped<IPaymentsResource, PaymentsResource>();

builder.Services.AddHealthChecks()
    .AddCheck<RepositoryHealthCheck>("repository")
    .AddCheck<BankClientHealthCheck>("bank");

builder.Services.AddObservability();

var app = builder.Build();
app.UseExceptionHandler("/error");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapHealthChecks("/health");

app.MapGet("/payments/{id:guid}", async (Guid id, IPaymentsResource resource, CancellationToken ct) =>
    {
        var result = await resource.GetAsync(id, ct);
        return result is null
            ? Results.NotFound()
            : Results.Ok(result);
    })
    .Produces<GetPaymentResponse>()
    .Produces((int)HttpStatusCode.NotFound);
   

app.MapPost("/payments", async (PostPaymentRequest request, IPaymentsResource resource, CancellationToken ct) =>
    {
        var result = await resource.ProcessPaymentAsync(request, ct);
        
        return result switch
        {
            // I would prefer returning 201 Created, but spec explicitly says 200 Ok. Ok.
            PaymentProcessingResult.Authorized(var payment) => Results.Ok(payment),
            PaymentProcessingResult.Declined(var payment)   => Results.Ok(payment),
            PaymentProcessingResult.BankError(var msg)      => Results.Problem(msg, statusCode: 503),
            _ => Results.Problem("Unknown result", statusCode: 500)
        };
    })
    .Produces<PostPaymentResponse>()
    .ProducesProblem((int)HttpStatusCode.InternalServerError)
    .ProducesProblem((int)HttpStatusCode.ServiceUnavailable)
    .AddEndpointFilter<ValidationFilter>();   

app.MapPrometheusScrapingEndpoint("/metrics");
app.Run();

// For testcontainers compatibility.
// ReSharper disable once ClassNeverInstantiated.Global
public partial class Program { }

