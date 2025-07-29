using System.Security.Claims;
using Final_project.Models;
using Final_project.Models.DTOs.CustomerService;
using Final_project.Services.CustomerService;
using Final_project.ViewModel.CustomerService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Final_project.Controllers.CustomerService
{
    public class CustomerServiceController : Controller
    {
        private readonly ICustomerServiceService _customerService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomerServiceController(ICustomerServiceService customerService, UserManager<ApplicationUser> userManager)
        {
            _customerService = customerService;
            _userManager = userManager;
        }
        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private async Task<bool> IsSupportRoleAsync()
        {
            //var user = await _userManager.GetUserAsync(User);
            //return await _userManager.IsInRoleAsync(user, "Support");
            return await Task.FromResult(true);
        }

        // Support Tickets
        public async Task<IActionResult> Index()
        {
            var isAdmin = await IsSupportRoleAsync();
            var tickets = isAdmin ?
                _customerService.GetAllTickets() :
                _customerService.GetTicketsByUser(GetCurrentUserId());

            var ticketViewModels = tickets.Select(t => new SupportTicketViewModel
            {
                Id = t.id,
                Subject = t.subject,
                Description = t.description,
                Status = t.status,
                Priority = t.priority,
                CreatedAt = t.created_at,
                ResolvedAt = t.resolved_at,
                UserName = t.User?.UserName,
                UserEmail = t.User?.Email,
                MessageCount = _customerService.GetTicketMessages(t.id).Count,
                LastMessageAt = _customerService.GetLatestTicketMessage(t.id)?.sent_at
            }).ToList();

            return View(ticketViewModels);
        }

        [HttpGet]
        public IActionResult CreateTicket()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateTicket(CreateSupportTicketDto dto)
        {
            if (ModelState.IsValid)
            {
                var ticket = new support_ticket
                {
                    user_id = GetCurrentUserId(),
                    subject = dto.Subject,
                    description = dto.Description,
                    priority = dto.Priority
                };

                _customerService.CreateTicket(ticket);
                TempData["Success"] = "Support ticket created successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(dto);
        }

        public IActionResult TicketDetails(string id)
        {
            var ticket = _customerService.GetTicketById(id);
            if (ticket == null)
                return NotFound();

            // Check if user can access this ticket
            var currentUserId = GetCurrentUserId();
            if (/*ticket.user_id != currentUserId && !User.IsInRole("Admin")*/false)
                return Forbid();

            var messages = _customerService.GetTicketMessages(id);
            var history = _customerService.GetTicketHistory(id);

            var viewModel = new TicketDetailsViewModel
            {
                Ticket = new SupportTicketViewModel
                {
                    Id = ticket.id,
                    Subject = ticket.subject,
                    Description = ticket.description,
                    Status = ticket.status,
                    Priority = ticket.priority,
                    CreatedAt = ticket.created_at,
                    ResolvedAt = ticket.resolved_at,
                    UserName = ticket.User?.UserName,
                    UserEmail = ticket.User?.Email
                },
                Messages = messages.Select(m => new TicketMessageViewModel
                {
                    Id = m.id,
                    Message = m.message,
                    SentAt = m.sent_at,
                    SenderName = m.Sender?.UserName,
                    SenderId = m.sender_id,
                    IsRead = m.is_read,
                    IsFromUser = m.sender_id == currentUserId
                }).ToList(),
                History = history.Select(h => new TicketHistoryViewModel
                {
                    Id = h.id,
                    ChangedAt = h.changed_at,
                    FieldChanged = h.field_changed,
                    OldValue = h.old_value,
                    NewValue = h.new_value,
                    ChangedBy = h.changed_by
                }).ToList()
            };

            // Mark messages as read
            _customerService.MarkTicketMessagesAsRead(id, currentUserId);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendTicketMessage(SendTicketMessageDTO dto)
        {
            ModelState.Remove(nameof(dto.SenderId));
            if (ModelState.IsValid)
            {
                dto.SenderId = GetCurrentUserId();
                Console.WriteLine(dto.SenderId);
                _customerService.SendTicketMessage(dto.TicketId, dto.SenderId, dto.Message);
                TempData["Success"] = "Message sent successfully!";
            }

            return RedirectToAction(nameof(TicketDetails), new { id = dto.TicketId });
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public IActionResult ResolveTicket(string ticketId)
        {
            _customerService.ResolveTicket(ticketId, GetCurrentUserId());
            TempData["Success"] = "Ticket resolved successfully!";
            return RedirectToAction(nameof(TicketDetails), new { id = ticketId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTicket(UpdateSupportTicketDTO dto)
        {
            if (ModelState.IsValid)
            {
                var ticket = _customerService.GetTicketById(dto.Id);
                if (ticket == null)
                    return NotFound();

                // Check permissions
                var currentUserId = GetCurrentUserId();
                var isAdmin = await IsSupportRoleAsync();

                if (ticket.user_id != currentUserId && !isAdmin)
                    return Forbid();

                // Users can only update subject and description, admins can update status and priority
                ticket.subject = dto.Subject ?? ticket.subject;
                ticket.description = dto.Description ?? ticket.description;

                if (isAdmin)
                {
                    ticket.status = dto.Status ?? ticket.status;
                    ticket.priority = dto.Priority ?? ticket.priority;
                }

                _customerService.UpdateTicket(ticket);
                TempData["Success"] = "Ticket updated successfully!";
            }

            return RedirectToAction(nameof(TicketDetails), new { id = dto.Id });
        }

        // Chat functionality
        public IActionResult Chats()
        {
            var currentUserId = GetCurrentUserId();
            var sessions = _customerService.GetUserChatSessions(currentUserId);

            var chatViewModels = sessions.Select(s => new ChatSessionViewModel
            {
                Id = s.Id,
                CustomerName = s.Customer?.UserName,
                SellerName = s.Seller?.UserName,
                CustomerId = s.CustomerId,
                SellerId = s.SellerId,
                CreatedAt = s.CreatedAt,
                LastMessageAt = s.LastMessageAt,
                Status = s.Status,
                UnreadCount = _customerService.GetUnreadChatMessageCount(s.Id, currentUserId),
                LastMessage = _customerService.GetLatestChatMessage(s.Id)?.message
            }).OrderByDescending(c => c.LastMessageAt).ToList();

            return View(chatViewModels);
        }

        public IActionResult ChatDetails(string id)
        {
            var session = _customerService.GetChatSessionById(id);
            if (session == null)
                return NotFound();

            var currentUserId = GetCurrentUserId();

            // Check if user is part of this chat session
            //if (session.CustomerId != currentUserId && session.SellerId != currentUserId)
            //    return Forbid();

            var messages = _customerService.GetChatMessages(id);

            var viewModel = new ChatDetailsViewModel
            {
                Session = new ChatSessionViewModel
                {
                    Id = session.Id,
                    CustomerName = session.Customer?.UserName,
                    SellerName = session.Seller?.UserName,
                    CustomerId = session.CustomerId,
                    SellerId = session.SellerId,
                    CreatedAt = session.CreatedAt,
                    LastMessageAt = session.LastMessageAt,
                    Status = session.Status
                },
                Messages = messages.Select(m => new ChatMessageViewModel
                {
                    Id = m.id,
                    Message = m.message,
                    SentAt = m.sent_at,
                    SenderName = m.Sender?.UserName,
                    SenderId = m.sender_id,
                    IsRead = m.is_read,
                    IsFromCurrentUser = m.sender_id == currentUserId
                }).ToList(),
                CurrentUserId = currentUserId
            };

            // Mark messages as read
            _customerService.MarkChatMessagesAsRead(id, currentUserId);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendChatMessage(SendChatMessageDTO dto)
        {
            if (ModelState.IsValid)
            {
                dto.SenderId = GetCurrentUserId();
                _customerService.SendChatMessage(dto.SessionId, dto.SenderId, dto.Message);
                return Json(new { success = true });
            }

            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });
        }

        [HttpPost]
        public IActionResult CreateChatSession(CreateChatSessionDTO dto)
        {
            if (ModelState.IsValid)
            {
                var session = _customerService.CreateOrGetChatSession(dto.CustomerId, dto.SellerId);
                return RedirectToAction(nameof(ChatDetails), new { id = session.Id });
            }

            return RedirectToAction(nameof(Chats));
        }

        [HttpPost]
        public IActionResult CloseChatSession(string sessionId)
        {
            _customerService.CloseChatSession(sessionId);
            TempData["Success"] = "Chat session closed successfully!";
            return RedirectToAction(nameof(Chats));
        }

        // Dashboard for admins
        //[Authorize(Roles = "Admin")]
        public IActionResult Dashboard()
        {
            var allTickets = _customerService.GetAllTickets();
            var activeSessions = _customerService.GetActiveChatSessions();

            var viewModel = new CustomerServiceDashboardViewModel
            {
                TotalTickets = allTickets.Count,
                OpenTickets = allTickets.Count(t => t.status == "Open"),
                ResolvedTickets = allTickets.Count(t => t.status == "Resolved"),
                HighPriorityTickets = allTickets.Count(t => t.priority == "High" || t.priority == "Critical"),
                ActiveChatSessions = activeSessions.Count,
                RecentTickets = allTickets.Take(10).Select(t => new SupportTicketViewModel
                {
                    Id = t.id,
                    Subject = t.subject,
                    Status = t.status,
                    Priority = t.priority,
                    CreatedAt = t.created_at,
                    UserName = t.User?.UserName
                }).ToList(),
                RecentChats = activeSessions.Take(10).Select(s => new ChatSessionViewModel
                {
                    Id = s.Id,
                    CustomerName = s.Customer?.UserName,
                    SellerName = s.Seller?.UserName,
                    LastMessageAt = s.LastMessageAt,
                    Status = s.Status
                }).ToList()
            };

            return View(viewModel);
        }

        // API endpoints for real-time updates
        [HttpGet]
        public IActionResult GetUnreadMessageCount(string sessionId)
        {
            var count = _customerService.GetUnreadChatMessageCount(sessionId, GetCurrentUserId());
            return Json(new { count });
        }

        [HttpGet]
        public IActionResult GetLatestMessages(string sessionId, DateTime? since)
        {
            var messages = _customerService.GetChatMessages(sessionId);

            if (since.HasValue)
            {
                messages = messages.Where(m => m.sent_at > since.Value).ToList();
            }

            var messageViewModels = messages.Select(m => new ChatMessageViewModel
            {
                Id = m.id,
                Message = m.message,
                SentAt = m.sent_at,
                SenderName = m.Sender?.UserName,
                SenderId = m.sender_id,
                IsFromCurrentUser = m.sender_id == GetCurrentUserId()
            }).ToList();

            return Json(messageViewModels);
        }
    }
}
