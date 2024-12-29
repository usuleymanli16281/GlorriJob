namespace GlorriJob.Common.Shared;

public class BaseResponse<T>
{
    public T? Data { get; set; }
    public string? CustomStatusCode { get; set; }
    public string? Message { get; set; }

    public BaseResponse()
    {

    }
    public BaseResponse(string message)
    {
        Message = message;
    }
    public BaseResponse(string message, string customStatusCode)
    {
        Message = message;
        CustomStatusCode = customStatusCode;
    }
    public BaseResponse(string message, string customStatusCode, T data)
    {
        Message = message;
        CustomStatusCode = customStatusCode;
        Data = data;
    }
}
