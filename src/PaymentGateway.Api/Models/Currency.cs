// ReSharper disable InconsistentNaming for Currency codes

using System.ComponentModel;
using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Models;

// In real system I would think of single source of currency codes across all services.

/// <summary>
/// Supported ISO 4217 currency codes (limited to 3 per requirements).
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<Currency>))]
public enum Currency
{
    [Description("British Pound")]
    GBP,
    [Description("US Dollar")]
    USD,
    [Description("Euro")]
    EUR,
    // Handle with care
    [Description("Japanese Yen")]
    JPY
}