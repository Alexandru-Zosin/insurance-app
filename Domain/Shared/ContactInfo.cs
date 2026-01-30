namespace Domain.Shared;

public sealed record ContactInfo
{
    public string Email { get; }
    public string Phone { get; }

    private ContactInfo(string email, string phone)
    {
        Email = email;
        Phone = phone;
    }

    public static ContactInfo Create(string email, string phone)
    {
        return new ContactInfo(email, phone);
    }
}
