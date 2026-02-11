namespace Application.Exceptions;
public sealed class DuplicateKeyException : Exception
{
    public DuplicateKeyException(Exception inner)
        : base("Unique constraint violated.", inner)
    {
    }
}