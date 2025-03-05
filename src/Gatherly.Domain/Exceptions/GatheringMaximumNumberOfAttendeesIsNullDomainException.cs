using Gatherly.Domain.Primitives;

namespace Gatherly.Domain.Exceptions;

public sealed class GatheringMaximumNumberOfAttendeesIsNullDomainException : DomainException
{
    public GatheringMaximumNumberOfAttendeesIsNullDomainException(string message) : base(message)
    {
    }
}
