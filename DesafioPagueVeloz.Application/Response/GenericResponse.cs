using System.Net.Security;
using System.Runtime.CompilerServices;

namespace DesafioPagueVeloz.Application.Response;

public class GenericResponse<T>
{
    public T? Data { get; private set; }
    public List<string> Errors { get; private set; } = [];
    public bool Success => !Errors.Any();
    public string Message { get; private set; }

    public void SetSuccess(T data, string message)
    {
        if (Errors.Any())
            throw new ArgumentException("Não é possivel setar sucesso para resposta pois já possuem erros");
        Data = data;
        Message = message;
    }

    public void PushError(string error)
    {
        Errors.Add(error);
    }
}