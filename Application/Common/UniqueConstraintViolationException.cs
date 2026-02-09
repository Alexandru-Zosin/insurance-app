using System.Net;

namespace Application.Common;

public sealed class UniqueConstraintViolationException : Exception
{
    public UniqueConstraintViolationException(Exception inner)
        : base("Unique constraint violated.", inner)
    {
    }
}