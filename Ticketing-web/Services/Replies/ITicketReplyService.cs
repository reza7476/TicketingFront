using Ticketing_web.Configurations.Interfaces.Interfaces;
using Ticketing_web.EntityModels.Commons;

namespace Ticketing_web.Services.Replies;

public interface ITicketReplyService : IService
{
    Task<ApiResultDto<object>> Approve(string replyId);
    Task<ApiResultDto<object>> EditReply(string replyId, string editedContent);
    Task<ApiResultDto<object>> ReplyTicket(string id, string content);
}
