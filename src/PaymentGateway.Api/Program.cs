using PaymentGateway.Api.Filters;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<PaymentsRepository>();
builder.Services.AddScoped<IPaymentsResource, PaymentsResource>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/payments/{id:guid}", async (Guid id, IPaymentsResource resource) =>
{
    var result = await resource.GetAsync(id);
    return result is null
        ? Results.NotFound()
        : Results.Ok(result);
});


app.MapPost("/payments", async (PostPaymentRequest request, IPaymentsResource resource) =>
    {
        var result = await resource.ProcessPaymentAsync(request);
        
        return result switch
        {
            // I would prefer returning 201 Created, but spec explicitly says 200 Ok. Ok.
            PaymentProcessingResult.Authorized(var payment) => Results.Ok(payment),
            PaymentProcessingResult.Declined(var payment)   => Results.Ok(payment),
            PaymentProcessingResult.BankError(var msg)      => Results.Problem(msg, statusCode: 503),
            _ => Results.Problem("Unknown result", statusCode: 500)
        };
    })
    .AddEndpointFilter<ValidationFilter>();   

app.Run();


