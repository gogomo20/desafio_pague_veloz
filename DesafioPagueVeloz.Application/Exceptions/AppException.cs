namespace DesafioPagueVeloz.Application;

public enum ApplicationTypeErrors
{
    BadRequest,
    NotFound,
    InternalServerError,
    Invalid
}
public class AppException : Exception
{
    public ApplicationTypeErrors Type { get; private set; }

    public AppException(string message, ApplicationTypeErrors type) : base(message)
    {
        Type = type;
    }

    public static AppException Invalid(string message) =>
        throw new AppException(message, ApplicationTypeErrors.Invalid);
    public static AppException NotFound(string message) =>
        throw new AppException(message, ApplicationTypeErrors.NotFound);
    public static AppException BadRequest(string message) =>
        throw new AppException(message, ApplicationTypeErrors.BadRequest);
    public static AppException InternalServerError(string message) =>
        throw new AppException(message, ApplicationTypeErrors.InternalServerError);
}