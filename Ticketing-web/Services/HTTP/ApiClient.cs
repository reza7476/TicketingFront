
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Ticketing_web.EntityModels.Commons;

namespace Ticketing_web.Services.Http;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiClient(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    // GET: دریافت اطلاعات
    public async Task<ApiResultDto<TResponse>> GetAsync<TResponse>(
        string url,
        CancellationToken cancellationToken = default)
    {
        AddAuthorizationHeader();

        using var response = await _httpClient.GetAsync(
            url, cancellationToken);

        return await HandleResponse<TResponse>(
            response, cancellationToken);
    }

    // POST: ایجاد اطلاعات
    public async Task<ApiResultDto<TResponse>> PostAsync<TRequest, TResponse>(
        string url,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            url, request, _jsonOptions, cancellationToken);

        return await HandleResponse<TResponse>(
            response, cancellationToken);
    }

    public async Task<ApiResultDto<object>> PostAsync<TRequest>(
    string url,
    TRequest request,
    CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            url, request, _jsonOptions, cancellationToken);

        return await HandleResponse<object>(
            response, cancellationToken);
    }

    // PUT: ویرایش کامل اطلاعات
    public async Task<ApiResultDto<TResponse>> PutAsync<TRequest, TResponse>(
        string url,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PutAsJsonAsync(
            url, request, _jsonOptions, cancellationToken);

        return await HandleResponse<TResponse>(
            response, cancellationToken);
    }

    // PATCH: ویرایش بخشی از اطلاعات
    public async Task<ApiResultDto<TResponse>> PatchAsync<TRequest, TResponse>(
        string url,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PatchAsJsonAsync(
            url, request, _jsonOptions, cancellationToken);

        return await HandleResponse<TResponse>(
            response, cancellationToken);
    }

    // DELETE: حذف اطلاعات
    public async Task<ApiResultDto<TResponse>> DeleteAsync<TResponse>(
        string url,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.DeleteAsync(
            url, cancellationToken);

        return await HandleResponse<TResponse>(
            response, cancellationToken);
    }

    

    private void AddAuthorizationHeader()
    {
        var token = _httpContextAccessor
            .HttpContext?
            .Session.GetString("JwtToken");
        _httpClient.DefaultRequestHeaders.Authorization = null;
        if (!string.IsNullOrWhiteSpace(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", token);
        }

    }




    private async Task<ApiResultDto<TResponse>> HandleResponse<TResponse>(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var statusCode = (int)response.StatusCode;

        var raw = await response.Content.ReadAsStringAsync(
            cancellationToken);

        var result = new ApiResultDto<TResponse>
        {
            StatusCode = statusCode
        };

        // 1. پاسخ ناموفق HTTP
        if (!response.IsSuccessStatusCode)
        {
            result.IsSuccess = false;
            result.Error = ExtractError(raw)
                ?? $"درخواست ناموفق بود. کد HTTP: {statusCode}";

            return result;
        }

        // 2. پاسخ موفق بدون محتوا؛ مانند 204 یا پاسخ خالی
        if (string.IsNullOrWhiteSpace(raw))
        {
            result.IsSuccess = true;
            result.Data = default;

            return result;
        }

        // 3. بررسی اینکه آیا پاسخ در قالب ApiResultDto است؟
        try
        {
            using var document = JsonDocument.Parse(raw);

            var root = document.RootElement;

            if (root.ValueKind == JsonValueKind.Object &&
                TryGetProperty(root, "IsSuccess", out var successElement) &&
                (successElement.ValueKind == JsonValueKind.True ||
                 successElement.ValueKind == JsonValueKind.False))
            {
                var apiResult =
                    JsonSerializer.Deserialize<ApiResultDto<TResponse>>(
                        raw, _jsonOptions);

                if (apiResult is not null)
                {
                    apiResult.StatusCode = statusCode;

                    if (!apiResult.IsSuccess &&
                        string.IsNullOrWhiteSpace(apiResult.Error))
                    {
                        apiResult.Error = "عملیات در بک‌اند ناموفق بود.";
                    }

                    return apiResult;
                }
            }
        }
        catch (JsonException)
        {
            // پاسخ در قالب JSON معتبر نیست؛ ادامه بررسی
        }

        // 4. پاسخ موفق با داده مستقیم، بدون ApiResultDto
        try
        {
            result.Data = JsonSerializer.Deserialize<TResponse>(
                raw, _jsonOptions);

            result.IsSuccess = true;
            return result;
        }
        catch (JsonException)
        {
            result.IsSuccess = false;
            result.Error = "قالب داده دریافتی با نوع مورد انتظار سازگار نیست.";

            return result;
        }
    }

    private static bool TryGetProperty(
        JsonElement element,
        string propertyName,
        out JsonElement value)
    {
        foreach (var property in element.EnumerateObject())
        {
            if (string.Equals(
                property.Name,
                propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                value = property.Value;
                return true;
            }
        }

        value = default;
        return false;
    }

    private string? ExtractError(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        try
        {
            using var document = JsonDocument.Parse(raw);

            var root = document.RootElement;

            if (root.ValueKind != JsonValueKind.Object)
                return null;

            foreach (var name in new[] { "Error", "Message", "Title" })
            {
                if (TryGetProperty(root, name, out var value) &&
                    value.ValueKind == JsonValueKind.String)
                {
                    var message = value.GetString();

                    if (!string.IsNullOrWhiteSpace(message))
                        return message;
                }
            }
        }
        catch (JsonException)
        {
            // بدنه پاسخ، JSON نیست
        }

        return null;
    }

}
