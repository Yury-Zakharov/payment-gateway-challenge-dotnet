using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Validation;

namespace PaymentGateway.Api.Tests.Models.Requests;

public class PostPaymentRequestTests
{
    private static readonly PostPaymentRequest ValidRequest = new()
    {
        CardNumber = "123456789012345",
        ExpiryMonth = 12,
        ExpiryYear = 2027,           
        Currency = Currency.GBP,
        Amount = 1000L,
        Cvv = "123"
    };

    [Fact]
    public void ValidRequest_PassesAllValidation()
    {
        var errors = ModelValidator.Validate(ValidRequest);
        Assert.Empty(errors);
    }

    [Theory]
    [MemberData(nameof(InvalidCases))]
    public void InvalidRequest_ReturnsExpectedError(PostPaymentRequest request, string expectedErrorMessage, params string[] expectedMemberNames)
    {
        var errors = ModelValidator.Validate(request);

        var error = Assert.Single(errors);
        Assert.Equal(expectedErrorMessage, error.ErrorMessage);
        Assert.Equal(expectedMemberNames, error.MemberNames);
    }

    public static IEnumerable<object[]> InvalidCases()
    {
        yield return
        [
            ValidRequest with { CardNumber = new string('1',14-1) },
            "Card number must be at least 14 characters long.",
            new[] { nameof(PostPaymentRequest.CardNumber) }
        ];

        yield return
        [
            ValidRequest with { CardNumber = new string('1',19+1) },
            "Card number must be at most 19 characters long.",
            new[] { nameof(PostPaymentRequest.CardNumber) }
        ];

        yield return
        [
            ValidRequest with { CardNumber = "4242-4242-4242-4242" }, 
            "Card number must only contain numeric characters.",
            new[] { nameof(PostPaymentRequest.CardNumber) }
        ];

        yield return
        [
            ValidRequest with { ExpiryMonth = 0 },
            "The field ExpiryMonth must be between 1 and 12.",
            new[] { nameof(PostPaymentRequest.ExpiryMonth) }
        ];

        yield return
        [
            ValidRequest with { ExpiryMonth = 13 },
            "The field ExpiryMonth must be between 1 and 12.",
            new[] { nameof(PostPaymentRequest.ExpiryMonth) }
        ];

        yield return
        [
            ValidRequest with { Cvv = "12" },
            "CVV must be at least 3 characters long.",
            new[] { nameof(PostPaymentRequest.Cvv) }
        ];

        yield return
        [
            ValidRequest with { Cvv = "12345" },
            "CVV must be at most 4 characters long.",
            new[] { nameof(PostPaymentRequest.Cvv) }
        ];

        yield return
        [
            ValidRequest with { Cvv = "12a" },
            "CVV must only contain numeric characters.",
            new[] { nameof(PostPaymentRequest.Cvv) }
        ];

        yield return
        [
            ValidRequest with { ExpiryYear = 2020, ExpiryMonth = 1 },
            "Value must be in the future.",
            new[] { nameof(PostPaymentRequest.ExpiryMonth), nameof(PostPaymentRequest.ExpiryYear) }
        ];

        yield return
        [
            ValidRequest with { Currency = (Currency)999 }, // invalid enum
            "The value is not a valid currency code.",
            new[] { nameof(PostPaymentRequest.Currency) }
        ];
    }
}