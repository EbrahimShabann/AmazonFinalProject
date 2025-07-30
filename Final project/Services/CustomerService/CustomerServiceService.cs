using Final_project.Models;
using Final_project.Repository;

namespace Final_project.Services.CustomerService
{
    public class CustomerServiceService:ICustomerServiceService
    {

        private readonly UnitOfWork _unitOfWork;

        public CustomerServiceService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Support Ticket operations
        public List<support_ticket> GetAllTickets()
        {
            return _unitOfWork.SupportTicketRepo.getAll();
        }

        public List<support_ticket> GetTicketsByUser(string userId)
        {
            return _unitOfWork.SupportTicketRepo.GetTicketsByUserId(userId);
        }

        public support_ticket GetTicketById(string ticketId)
        {
            return _unitOfWork.SupportTicketRepo.getById(ticketId);
        }

        public support_ticket CreateTicket(support_ticket ticket)
        {
            ticket.created_at = DateTime.UtcNow;
            ticket.status = "Open";
            ticket.is_deleted = false;

            _unitOfWork.SupportTicketRepo.add(ticket);
            _unitOfWork.save();

            // Log ticket creation
            LogTicketChange(ticket.id, ticket.user_id, "Status", null, "Open");

            return ticket;
        }

        public void UpdateTicket(support_ticket ticket)
        {
            var existingTicket = _unitOfWork.SupportTicketRepo.getById(ticket.id);
            if (existingTicket != null)
            {
                // Log changes
                if (existingTicket.status != ticket.status)
                    LogTicketChange(ticket.id, ticket.user_id, "Status", existingTicket.status, ticket.status);

                if (existingTicket.priority != ticket.priority)
                    LogTicketChange(ticket.id, ticket.user_id, "Priority", existingTicket.priority, ticket.priority);

                _unitOfWork.SupportTicketRepo.Update(ticket);
                _unitOfWork.save();
            }
        }

        public void ResolveTicket(string ticketId, string resolvedBy)
        {
            var ticket = _unitOfWork.SupportTicketRepo.getById(ticketId);
            if (ticket != null)
            {
                var oldStatus = ticket.status;
                _unitOfWork.SupportTicketRepo.ResolveTicket(ticketId, resolvedBy);
                _unitOfWork.save();

                // Log resolution
                LogTicketChange(ticketId, resolvedBy, "Status", oldStatus, "Resolved");
            }
        }

        public void DeleteTicket(string ticketId)
        {
            _unitOfWork.SupportTicketRepo.SoftDelete(ticketId);
            _unitOfWork.save();
        }

        public List<support_ticket> GetTicketsByStatus(string status)
        {
            return _unitOfWork.SupportTicketRepo.GetTicketsByStatus(status);
        }

        public List<support_ticket> GetTicketsByPriority(string priority)
        {
            return _unitOfWork.SupportTicketRepo.GetTicketsByPriority(priority);
        }

        public List<support_ticket> GetUnresolvedTickets()
        {
            return _unitOfWork.SupportTicketRepo.GetUnresolvedTickets();
        }

        // Ticket Message operations
        public List<ticket_message> GetTicketMessages(string ticketId)
        {
            return _unitOfWork.TicketMessageRepo.GetMessagesByTicketId(ticketId);
        }

        public ticket_message SendTicketMessage(string ticketId, string senderId, string message)
        {
            var ticketMessage = new ticket_message
            {
                ticket_id = ticketId,
                sender_id = senderId,
                message = message,
                sent_at = DateTime.UtcNow,
                is_read = false
            };

            _unitOfWork.TicketMessageRepo.add(ticketMessage);
            _unitOfWork.save();

            return ticketMessage;
        }

        public void MarkTicketMessagesAsRead(string ticketId, string userId)
        {
            _unitOfWork.TicketMessageRepo.MarkMessagesAsRead(ticketId, userId);
            _unitOfWork.save();
        }

        public ticket_message GetLatestTicketMessage(string ticketId)
        {
            return _unitOfWork.TicketMessageRepo.GetLatestMessage(ticketId);
        }

        // Ticket History operations
        public List<ticket_history> GetTicketHistory(string ticketId)
        {
            return _unitOfWork.TicketHistoryRepo.GetHistoryByTicketId(ticketId);
        }

        public void LogTicketChange(string ticketId, string changedBy, string fieldChanged, string oldValue, string newValue)
        {
            _unitOfWork.TicketHistoryRepo.LogChange(ticketId, changedBy, fieldChanged, oldValue, newValue);
            _unitOfWork.save();
        }

        // Chat Session operations
        public List<chat_session> GetUserChatSessions(string userId)
        {
            var customerSessions = _unitOfWork.ChatSessionRepo.GetSessionsByCustomerId(userId);
            var sellerSessions = _unitOfWork.ChatSessionRepo.GetSessionsBySellerId(userId);

            return customerSessions.Concat(sellerSessions).Distinct().ToList();
        }

        public chat_session GetChatSessionById(string sessionId)
        {
            return _unitOfWork.ChatSessionRepo.getById(sessionId);
        }

        public chat_session CreateOrGetChatSession(string customerId, string sellerId)
        {
            var existingSession = _unitOfWork.ChatSessionRepo.GetSessionByParticipants(customerId, sellerId);

            if (existingSession != null && existingSession.Status != "Closed")
            {
                return existingSession;
            }

            var newSession = new chat_session
            {
                CustomerId = customerId,
                SellerId = sellerId,
                CreatedAt = DateTime.UtcNow,
                LastMessageAt = DateTime.UtcNow,
                Status = "Active",
                IsDeleted = false
            };

            _unitOfWork.ChatSessionRepo.add(newSession);
            _unitOfWork.save();

            return newSession;
        }

        public void CloseChatSession(string sessionId)
        {
            _unitOfWork.ChatSessionRepo.CloseSession(sessionId);
            _unitOfWork.save();
        }

        public List<chat_session> GetActiveChatSessions()
        {
            return _unitOfWork.ChatSessionRepo.GetActiveSessions();
        }

        // Chat Message operations
        public List<chat_message> GetChatMessages(string sessionId)
        {
            return _unitOfWork.ChatMessageRepo.GetMessagesBySessionId(sessionId);
        }

        public chat_message SendChatMessage(string sessionId, string senderId, string message)
        {
            var chatMessage = new chat_message
            {
                session_id = sessionId,
                sender_id = senderId,
                message = message,
                sent_at = DateTime.UtcNow,
                is_read = false
            };

            _unitOfWork.ChatMessageRepo.add(chatMessage);

            // Update session's last message time
            _unitOfWork.ChatSessionRepo.UpdateLastMessageTime(sessionId);

            _unitOfWork.save();

            return chatMessage;
        }

        public void MarkChatMessagesAsRead(string sessionId, string userId)
        {
            _unitOfWork.ChatMessageRepo.MarkMessagesAsRead(sessionId, userId);
            _unitOfWork.save();
        }

        public int GetUnreadChatMessageCount(string sessionId, string userId)
        {
            return _unitOfWork.ChatMessageRepo.GetUnreadMessageCount(sessionId, userId);
        }

        public chat_message GetLatestChatMessage(string sessionId)
        {
            return _unitOfWork.ChatMessageRepo.GetLatestMessage(sessionId);
        }
    }
}
