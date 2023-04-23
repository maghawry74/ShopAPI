namespace ShopAPI.BL.CustomClasses;

public class StatusCodeEx : Exception
{
    public int StatusCode { init; get; }
    public StatusCodeEx(int statusCode, string? Message = null) : base(Message)
    {
        StatusCode = statusCode;
    }
}
