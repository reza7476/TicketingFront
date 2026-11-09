using Ticketing_web.Configurations.Interfaces.Interfaces;
using Ticketing_web.EntityModels.Commons;
using Ticketing_web.EntityModels.Documents;

namespace Ticketing_web.Services.FileStorage;

public interface IFileService : IService
{
    Task<ApiResultDto<object>> Delete(string documentId);
    Task<ApiResultDto<DocumentDto>> Upload(IFormFile document);
}
