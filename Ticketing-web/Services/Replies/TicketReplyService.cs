using Ticketing_web.EntityModels.Commons;
using Ticketing_web.Services.Http;

namespace Ticketing_web.Services.Replies;

public class TicketReplyService : ITicketReplyService
{
    private readonly ApiClient _apiClient;

    public TicketReplyService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public  async Task<ApiResultDto<object>> EditReply(
        string replyId,
        string editedContent)
    {
        var request = new
        {
            Content = editedContent
        };
        return await _apiClient.PutAsync<object, object>($"ticket-replies/{replyId}", request);
    }

    public async Task<ApiResultDto<object>> ReplyByAgent(string id, string content)
    {
        var request = new
        {
            TicketId = id,
            Content = content
        };
        return await _apiClient.PostAsync<object>(
             "ticket-replies/reply-by-agent",
             request);
    }
}
