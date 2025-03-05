namespace Gatherly.Domain.Shared;

public class Error : IEquatable<Error>
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "The specified result value is null.");

    public Error(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public string Code { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public static bool operator ==(Error a, Error b)
    {
        if (a is null && b is null)
        {
            return true;
        }
        if (a is null || b is null)
        {
            return false;
        }
        return a.Equals(b);
    }

    public static bool operator !=(Error a, Error b)
    {
        return !(a == b);
    }

    public bool Equals(Error? other)
    {
        if (other is null)
        {
            return false;
        }
        return Code == other.Code && Message == other.Message;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Code, Message);
    }

    public override string ToString()
    {
        return Code + ": " + Message;
    }

    public override bool Equals(object? obj)
    {
        return obj is Error error && Equals(error);
    }
}
