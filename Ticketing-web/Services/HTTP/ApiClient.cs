
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
        AddAuthorizationHeader();
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
        AddAuthorizationHeader();
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
        AddAuthorizationHeader();
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
        var result = new ApiResultDto<TResponse>
        {
            StatusCode = (int)response.StatusCode,
            IsSuccess = false
        };

        // 1. خواندن متن پاسخ بک‌اند
        var raw = await response.Content.ReadAsStringAsync(
            cancellationToken);

        // 2. اگر پاسخ HTTP ناموفق بود
        if (!response.IsSuccessStatusCode)
        {
            result.Error = string.IsNullOrWhiteSpace(raw)
                ? $"خطای HTTP: {result.StatusCode}"
                : raw;

            return result;
        }

        // 3. اگر پاسخ خالی بود
        if (string.IsNullOrWhiteSpace(raw))
        {
            result.IsSuccess = true;
            return result;
        }

        // 4. اگر انتظار داریم پاسخ از نوع string باشد
        if (typeof(TResponse) == typeof(string))
        {
            result.Data = (TResponse)(object)raw;
            result.IsSuccess = true;
            return result;
        }

        // 5. تبدیل JSON دریافتی به نوع مورد انتظار
        try
        {
            result.Data = JsonSerializer.Deserialize<TResponse>(
                raw, _jsonOptions);

            result.IsSuccess = true;
        }
        catch (JsonException)
        {
            result.Error = "ساختار پاسخ بک‌اند با مدل مورد انتظار مطابقت ندارد.";
        }

        return result;
    }

}
