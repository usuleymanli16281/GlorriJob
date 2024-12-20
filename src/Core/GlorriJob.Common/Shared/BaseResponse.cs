namespace GlorriJob.Common.Shared;

public class BaseResponse<T>
{
    public T? Data { get; set; }
    public string? StatusCode { get; set; }
    public string? Message { get; set; }

    public BaseResponse()
    {

    }
    public BaseResponse(string message)
    {
        Message = message;
    }
    public BaseResponse(string message, string statusCode)
    {
        Message = message;
        StatusCode = statusCode;
    }
    public BaseResponse(string message, string statusCode, T data)
    {
        Message = message;
        StatusCode = statusCode;
        Data = data;
    }
}
