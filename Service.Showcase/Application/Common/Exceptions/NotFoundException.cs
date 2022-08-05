namespace Service.Showcase.Application.Common.Exceptions;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

[Serializable]
[ExcludeFromCodeCoverage]
public class NotFoundException : Exception
{
    public NotFoundException(string message)
        : base(message)
    {
    }

    protected NotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    /// <summary>Throws an <see cref="NotFoundException"/> if <paramref name="argument"/> is null.</summary>
    /// <param name="argument">The reference type argument to validate as non-null.</param>
    public static void ThrowIfNull(object argument)
    {
        if (argument is null)
        {
            throw new NotFoundException("The showcase with the supplied id was not found.");
        }
    }
}