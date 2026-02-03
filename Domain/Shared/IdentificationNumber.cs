namespace Domain.Shared;

public sealed record IdentificationNumber
{
    public string Value { get; }

    private IdentificationNumber(string value)
    {
        Value = value;
    }

    public static IdentificationNumber Create(string value)
    {
        return new IdentificationNumber(value);
    }
}
