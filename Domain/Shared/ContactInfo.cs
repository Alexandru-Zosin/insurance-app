using Domain.Common;
namespace Domain.Shared;

public sealed record ContactInfo(string Email, string Phone)
{
    public static ContactInfo Create(string email, string phone)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(phone))
            throw new DomainException("Invalid email or phone provided");

        return new ContactInfo(email, phone);
    }
}