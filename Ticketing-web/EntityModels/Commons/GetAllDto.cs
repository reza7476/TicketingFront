using System.Text.Json.Serialization;

namespace Ticketing_web.EntityModels.Commons;


public class GetAllDto<TData>
{
    [JsonPropertyName("elements")]
    public List<TData> Data { get; set; } = new();

    [JsonPropertyName("totalElements")]
    public int TotalElements { get; set; }

    public int CurrentPage { get; set; }

    public int TotalPages { get; set; }
}