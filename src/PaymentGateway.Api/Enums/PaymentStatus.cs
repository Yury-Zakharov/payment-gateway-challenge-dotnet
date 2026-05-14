using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<PaymentStatus>))]
public enum PaymentStatus
{
    Authorized,
    Declined,
    Rejected
}