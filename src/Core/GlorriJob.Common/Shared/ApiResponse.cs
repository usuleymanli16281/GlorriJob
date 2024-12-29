namespace GlorriJob.Common.Shared;

public class ApiResponse<T>
{
    public T? Data { get; set; }
    public string? CustomStatusCode { get; set; }
    public string? Message { get; set; }

    public ApiResponse()
    {

    }
    public ApiResponse(string message)
    {
        Message = message;
    }
    public ApiResponse(string message, string customStatusCode)
    {
        Message = message;
        CustomStatusCode = customStatusCode;
    }
    public ApiResponse(string message, string customStatusCode, T data)
    {
        Message = message;
        CustomStatusCode = customStatusCode;
        Data = data;
    }
}
