using PaymentGateway.Api.Validation;

namespace PaymentGateway.Api.Filters;

public sealed class ValidationFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext ctx, EndpointFilterDelegate next)
    {
        foreach (var arg in ctx.Arguments)
        {
            if (arg is null) 
                continue;

            var results = ModelValidator.Validate(arg);
            if (results.Count > 0)
            {
                var errors = results
                    .GroupBy(r => r.MemberNames.FirstOrDefault() ?? "error")
                    .ToDictionary(g => g.Key, g => g.Select(r => r.ErrorMessage!).ToArray());

                return Results.BadRequest(new { errors }); 
            }
        }

        return await next(ctx);
    }
}