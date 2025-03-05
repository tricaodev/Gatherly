﻿namespace Gatherly.Domain.Primitives;

public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }
}
