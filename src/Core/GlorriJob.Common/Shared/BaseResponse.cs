using System.Net;

namespace GlorriJob.Common.Shared;

public class BaseResponse<T>
{
    public T? Data { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public string? Message { get; set; }
}
