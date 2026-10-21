namespace Ticketing_web.EntityModels.Commons;

public class ApiResultDto<TData>
{
    public TData? Data { get; set; }
    public string? Error { get; set; }
    public int StatusCode { get; set; }
    public bool IsSuccess { get; set; }
}
