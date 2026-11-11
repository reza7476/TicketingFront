using Ticketing_web.Configurations.Interfaces.Interfaces;
using Ticketing_web.EntityModels.Commons;
using Ticketing_web.EntityModels.Documents;

namespace Ticketing_web.Services.Replies;

public interface ITicketReplyService : IService
{
    Task<ApiResultDto<object>> AddFile(string id, DocumentDto document);
    Task<ApiResultDto<object>> Approve(string replyId);
    Task<ApiResultDto<object>> DeleteDocument( string documentId);
    Task<ApiResultDto<object>> EditReply(string replyId, string editedContent);
    Task<ApiResultDto<string>> ReplyTicket(
        string id,
        string content, 
        DocumentDto? document=null);
}
