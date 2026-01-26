using Domain.Common;
using System.Net.Mail;

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

        if (!IsValidEmail(email))
            return Result<ContactInfo>.Fail(ErrorType.Validation, "Invalid email");

        if (string.IsNullOrWhiteSpace(phone))
            return Result<ContactInfo>.Fail(ErrorType.Validation, "Phone required");

        return Result<ContactInfo>.Ok(new ContactInfo(email, phone));
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var address = new MailAddress(email);
            return address.Address == email;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
