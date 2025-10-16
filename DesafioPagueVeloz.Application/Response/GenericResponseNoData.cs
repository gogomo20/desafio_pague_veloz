namespace DesafioPagueVeloz.Application.Response;

public class GenericResponseNoData
{
    public List<string> Errors { get; private set; } = [];
    public bool Success => !Errors.Any();
    public string Message { get; private set; }

    public void SetSuccess(string message)
    {
        Message = message;
    }
    public void PushError(string error)
    {
        Errors.Add(error);
    }
}