using Ticketing_web.EntityModels.Commons;
using Ticketing_web.EntityModels.Documents;
using Ticketing_web.Services.Http;

namespace Ticketing_web.Services.FileStorage;

public class FileService : IFileService
{
    private readonly ApiClient _apiClient;

    public FileService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<ApiResultDto<object>> Delete(string documentId)
    {
        return await _apiClient.DeleteAsync<object>($"storages/{documentId}");
    }

    public async Task<ApiResultDto<DocumentDto>> Upload(IFormFile document)
    {
        return await _apiClient
           .PostFileAsync<DocumentDto>(
              "storages/upload", document);
    }
}
