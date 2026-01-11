namespace Bhd.Application.Exceptions;

public class AppException : Exception
{
    public AppException() : base() {}
    public AppException(string message) : base(message) {}
    public AppException(string message, Exception innerException) : base(message, innerException) {}
}

public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message) : base(message) {}
}

public class BadRequestException : AppException
{
    public BadRequestException(string message) : base(message) {}
}

public class ForbiddenException : AppException
{
    public ForbiddenException(string message) : base(message) {}
}