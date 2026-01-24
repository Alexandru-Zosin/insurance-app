using Domain.Common;

namespace Domain.ValueObjects;

public sealed record ContactInfo
{
    public string Email { get; }
    public string Phone { get; }

    private ContactInfo(string email, string phone)
    {
        Email = email;
        Phone = phone;
    }

    public static Result<ContactInfo> Create(string email, string phone)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result<ContactInfo>.Fail(ErrorType.Validation, "Email required");

        if (string.IsNullOrWhiteSpace(phone))
            return Result<ContactInfo>.Fail(ErrorType.Validation, "Phone required");

        return Result<ContactInfo>.Ok(new ContactInfo(email, phone));
    }
}
