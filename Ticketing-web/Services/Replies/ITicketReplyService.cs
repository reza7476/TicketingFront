using Ticketing_web.Configurations.Interfaces.Interfaces;
using Ticketing_web.EntityModels.Commons;

namespace Ticketing_web.Services.Replies;

public interface ITicketReplyService : IService
{
    Task<ApiResultDto<object>> EditReply(string replyId, string editedContent);
    Task<ApiResultDto<object>> ReplyByAgent(string id, string content);
}
