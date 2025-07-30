using Microsoft.AspNetCore.SignalR;
using Final_project.Models;
using Final_project.Services.CustomerService;

namespace Final_project.Hubs
{
    public class CustomerServiceHub : Hub
    {
        private readonly ICustomerServiceService _customerService;

        public CustomerServiceHub(ICustomerServiceService customerService)
        {
            _customerService = customerService;
        }

        public async Task JoinTicketGroup(string ticketId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"ticket_{ticketId}");
        }

        public async Task LeaveTicketGroup(string ticketId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"ticket_{ticketId}");
        }

        public async Task JoinChatSession(string sessionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"chat_{sessionId}");
        }

        public async Task LeaveChatSession(string sessionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"chat_{sessionId}");
        }

        public async Task SendTicketMessage(string ticketId, string message, string senderId)
        {
            var ticketMessage = _customerService.SendTicketMessage(ticketId, senderId, message);

            await Clients.Group($"ticket_{ticketId}").SendAsync("ReceiveTicketMessage", new
            {
                Id = ticketMessage.id,
                Message = ticketMessage.message,
                SentAt = ticketMessage.sent_at,
                SenderId = ticketMessage.sender_id,
                IsRead = ticketMessage.is_read
            });
        }

        public async Task SendChatMessage(string sessionId, string message, string senderId)
        {
            var chatMessage = _customerService.SendChatMessage(sessionId, senderId, message);

            await Clients.Group($"chat_{sessionId}").SendAsync("ReceiveChatMessage", new
            {
                Id = chatMessage.id,
                Message = chatMessage.message,
                SentAt = chatMessage.sent_at,
                SenderId = chatMessage.sender_id,
                IsRead = chatMessage.is_read
            });
        }

        public async Task MarkMessagesAsRead(string ticketId, string userId)
        {
            _customerService.MarkTicketMessagesAsRead(ticketId, userId);
            await Clients.Group($"ticket_{ticketId}").SendAsync("MessagesMarkedAsRead", userId);
        }

        public async Task MarkChatMessagesAsRead(string sessionId, string userId)
        {
            _customerService.MarkChatMessagesAsRead(sessionId, userId);
            await Clients.Group($"chat_{sessionId}").SendAsync("ChatMessagesMarkedAsRead", userId);
        }

        public async Task UpdateTicketStatus(string ticketId, string status, string updatedBy)
        {
            var ticket = _customerService.GetTicketById(ticketId);
            if (ticket != null)
            {
                ticket.status = status;
                _customerService.UpdateTicket(ticket);

                await Clients.Group($"ticket_{ticketId}").SendAsync("TicketStatusUpdated", new
                {
                    TicketId = ticketId,
                    Status = status,
                    UpdatedBy = updatedBy,
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }

        public async Task NotifyNewTicket(string ticketId, string subject, string userName)
        {
            await Clients.All.SendAsync("NewTicketCreated", new
            {
                TicketId = ticketId,
                Subject = subject,
                UserName = userName,
                CreatedAt = DateTime.UtcNow
            });
        }

        public async Task NotifyNewChatSession(string sessionId, string customerName, string sellerName)
        {
            await Clients.All.SendAsync("NewChatSessionCreated", new
            {
                SessionId = sessionId,
                CustomerName = customerName,
                SellerName = sellerName,
                CreatedAt = DateTime.UtcNow
            });
        }
    }
}