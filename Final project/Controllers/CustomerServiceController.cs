using System.Security.Claims;
using Final_project.Hubs;
using Final_project.Models;
using Final_project.Models.DTOs.CustomerService;
using Final_project.Services.CustomerService;
using Final_project.ViewModel.CustomerService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Final_project.Controllers.CustomerService
{
    //[Authorize]
    public class CustomerServiceController : Controller
    {
        private readonly ICustomerServiceService _customerService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<CustomerServiceHub> _hubContext;

        public CustomerServiceController(ICustomerServiceService customerService, UserManager<ApplicationUser> userManager, IHubContext<CustomerServiceHub> hubContext)
        {
            _customerService = customerService;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private async Task<bool> IsSupportRoleAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return false;

            return await _userManager.IsInRoleAsync(user, "customerService") ||
                   await _userManager.IsInRoleAsync(user, "Admin");
        }

        private async Task<bool> IsAdminRoleAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return false;

            return await _userManager.IsInRoleAsync(user, "Admin");
        }

        private async Task<bool> IsCustomerRoleAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return false;

            return await _userManager.IsInRoleAsync(user, "Customer");
        }

        private async Task<bool> IsSellerRoleAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return false;

            return await _userManager.IsInRoleAsync(user, "Seller");
        }

        // Support Tickets - Customers can view their own tickets, Support/Admin can view all
        public async Task<IActionResult> Index()
        {
            var isSupportOrAdmin = await IsSupportRoleAsync();
            var isCustomer = await IsCustomerRoleAsync();

            IEnumerable<support_ticket> tickets;

            if (isSupportOrAdmin)
            {
                // Support and Admin can see all tickets
                tickets = _customerService.GetAllTickets();
            }
            else if (isCustomer)
            {
                // Customers can only see their own tickets
                tickets = _customerService.GetTicketsByUser(GetCurrentUserId());
            }
            else
            {
                // Sellers and other roles see their own tickets
                tickets = _customerService.GetTicketsByUser(GetCurrentUserId());
            }

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
                LastMessageAt = _customerService.GetLatestTicketMessage(t.id)?.sent_at,
                CanManage = isSupportOrAdmin || (isCustomer && t.user_id == GetCurrentUserId())
            }).ToList();

            ViewBag.IsSupportOrAdmin = isSupportOrAdmin;
            ViewBag.IsCustomer = isCustomer;
            ViewBag.IsSeller = await IsSellerRoleAsync();

            return View(ticketViewModels);
        }

        [HttpGet]
        public IActionResult CreateTicket()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTicket(CreateSupportTicketDto dto)
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

                // Notify about new ticket via SignalR
                var user = await _userManager.GetUserAsync(User);
                await _hubContext.Clients.All.SendAsync("NewTicketCreated", new
                {
                    TicketId = ticket.id,
                    Subject = ticket.subject,
                    UserName = user?.UserName ?? "Unknown User",
                    CreatedAt = DateTime.UtcNow
                });

                TempData["Success"] = "Support ticket created successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(dto);
        }

        public async Task<IActionResult> TicketDetails(string id)
        {
            var ticket = _customerService.GetTicketById(id);
            if (ticket == null)
                return NotFound();

            // Check permissions
            var currentUserId = GetCurrentUserId();
            var isSupportOrAdmin = await IsSupportRoleAsync();
            var isCustomer = await IsCustomerRoleAsync();

            // Only allow access if user is Support/Admin, or if it's their own ticket
            if (!isSupportOrAdmin && ticket.user_id != currentUserId)
            {
                return Forbid();
            }

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
                    UserEmail = ticket.User?.Email,
                    CanManage = isSupportOrAdmin || (isCustomer && ticket.user_id == currentUserId)
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

            // Mark messages as read for the current user
            _customerService.MarkTicketMessagesAsRead(id, currentUserId);

            ViewBag.IsSupportOrAdmin = isSupportOrAdmin;
            ViewBag.IsCustomer = isCustomer;
            ViewBag.IsSeller = await IsSellerRoleAsync();
            ViewBag.CanManageTicket = isSupportOrAdmin || (isCustomer && ticket.user_id == currentUserId);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendTicketMessage(SendTicketMessageDTO dto)
        {
            ModelState.Remove(nameof(dto.SenderId));
            if (ModelState.IsValid)
            {
                dto.SenderId = GetCurrentUserId();
                var ticketMessage = _customerService.SendTicketMessage(dto.TicketId, dto.SenderId, dto.Message);

                // Notify via SignalR
                await _hubContext.Clients.Group($"ticket_{dto.TicketId}").SendAsync("ReceiveTicketMessage", new
                {
                    Id = ticketMessage.id,
                    Message = ticketMessage.message,
                    SentAt = ticketMessage.sent_at,
                    SenderId = ticketMessage.sender_id,
                    IsRead = ticketMessage.is_read
                });

                TempData["Success"] = "Message sent successfully!";
            }

            return RedirectToAction(nameof(TicketDetails), new { id = dto.TicketId });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,customerService")]
        public async Task<IActionResult> ResolveTicket(string ticketId)
        {
            var ticket = _customerService.GetTicketById(ticketId);
            if (ticket == null)
                return NotFound();

            // Only Support and Admin can resolve tickets
            var isSupportOrAdmin = await IsSupportRoleAsync();
            if (!isSupportOrAdmin)
            {
                return Forbid();
            }

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
                var isSupportOrAdmin = await IsSupportRoleAsync();
                var isCustomer = await IsCustomerRoleAsync();

                // Only allow updates if user is Support/Admin, or if it's their own ticket
                if (!isSupportOrAdmin && ticket.user_id != currentUserId)
                {
                    return Forbid();
                }

                // Users can only update subject and description, Support/Admin can update status and priority
                ticket.subject = dto.Subject ?? ticket.subject;
                ticket.description = dto.Description ?? ticket.description;

                if (isSupportOrAdmin)
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
        public async Task<IActionResult> Chats()
        {
            var currentUserId = GetCurrentUserId();
            var isCustomerService = await IsSupportRoleAsync();

            List<chat_session> sessions;

            if (isCustomerService)
            {
                // Customer service can see all chat sessions
                sessions = _customerService.GetActiveChatSessions();
            }
            else
            {
                // Regular users can only see their own chat sessions
                sessions = _customerService.GetUserChatSessions(currentUserId);
            }

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

            var isParticipant = session.CustomerId == currentUserId || session.SellerId == currentUserId;
            var isCustomerService = User.IsInRole("customerservice");

            if (!isParticipant && !isCustomerService)
                return Forbid();

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
        public IActionResult SendChatMessage(string SessionId, string Message)
        {
            try
            {
                // Log the incoming request for debugging
                System.Diagnostics.Debug.WriteLine($"SendChatMessage called - SessionId: {SessionId}, Message: {Message}");

                if (string.IsNullOrWhiteSpace(Message))
                {
                    return Json(new { success = false, error = "Message cannot be empty." });
                }

                if (SessionId.Length <= 0)
                {
                    return Json(new { success = false, error = "Invalid session ID." });
                }

                var currentUserId = GetCurrentUserId();

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Json(new { success = false, error = "User not authenticated." });
                }

                // Send the message using your service
                var chatMessage = _customerService.SendChatMessage(SessionId, currentUserId, Message);

                if (chatMessage == null)
                {
                    return Json(new { success = false, error = "Failed to create message." });
                }

                // Get sender name - you might need to adjust this based on your model
                var senderName = "You"; // Default fallback

                if (chatMessage?.Sender != null)
                {
                    senderName = chatMessage.Sender.UserName ?? User?.Identity?.Name ?? "You";
                }
                else if (User?.Identity?.IsAuthenticated == true)
                {
                    senderName = User.Identity.Name ?? "You";
                }

                var response = new
                {
                    success = true,
                    message = new
                    {
                        id = chatMessage?.id ?? "0", // Use string fallback
                        message = chatMessage?.message ?? Message,
                        senderName = senderName,
                        sentAt = chatMessage?.sent_at?.ToString("yyyy-MM-ddTHH:mm:ss") ?? DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                        isFromCurrentUser = true
                    }
                };
                System.Diagnostics.Debug.WriteLine($"Returning JSON response: {System.Text.Json.JsonSerializer.Serialize(response)}");

                return Json(response);
            }
            catch (Exception ex)
            {
                // Log the full exception for debugging
                System.Diagnostics.Debug.WriteLine($"SendChatMessage error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");

                return Json(new
                {
                    success = false,
                    error = "Failed to send message. Please try again.",
                    details = ex.Message // Remove this in production
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetChatMessages(string sessionId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                // Get messages from your service
                var messages = _customerService.GetChatMessages(sessionId);

                var messageList = messages.Select(m => new {
                    id = m.id,
                    message = m.message,
                    senderName = m.Sender.UserName,
                    sentAt = m.sent_at,
                    isFromCurrentUser = m.sender_id
                    == currentUserId
                }).OrderBy(m => m.sentAt).ToList();

                return Json(new { success = true, messages = messageList });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = "Failed to load messages." });
            }
        }


        [HttpGet]
        [Authorize(Roles = "customerService")]
        public async Task<IActionResult> CreateChatSession()
        {
            // Only customerService role can create chat sessions
            var isCustomerService = await IsSupportRoleAsync();
            if (!isCustomerService)
            {
                return Forbid();
            }

            var customers = await _customerService.GetAllCustomers();
            var sellers = await _customerService.GetAllSellers();

            ViewBag.Customers = customers;
            ViewBag.Sellers = sellers;

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "customerService")]
        public async Task<IActionResult> CreateChatSession(CreateChatSessionDTO dto)
        {
            // Only customerService role can create chat sessions
            var isCustomerService = await IsSupportRoleAsync();
            if (!isCustomerService)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                var session = _customerService.CreateOrGetChatSession(dto.CustomerId, dto.SellerId);
                return RedirectToAction(nameof(Chats));
            }

            // If model is invalid, reload the form with data
            var customers = await _customerService.GetAllCustomers();
            var sellers = await _customerService.GetAllSellers();

            ViewBag.Customers = customers;
            ViewBag.Sellers = sellers;

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> CloseChatSession(string sessionId)
        {
            var currentUserId = GetCurrentUserId();
            var isCustomerService = await IsSupportRoleAsync();

            var session = _customerService.GetChatSessionById(sessionId);
            if (session == null)
            {
                TempData["Error"] = "Chat session not found.";
                return RedirectToAction(nameof(Chats));
            }

            // Only customer service or participants can close the session
            if (!isCustomerService && session.CustomerId != currentUserId && session.SellerId != currentUserId)
            {
                TempData["Error"] = "You are not authorized to close this chat session.";
                return RedirectToAction(nameof(Chats));
            }

            _customerService.CloseChatSession(sessionId);
            TempData["Success"] = "Chat session closed successfully!";
            return RedirectToAction(nameof(Chats));
        }

        // Dashboard for Support and Admin only
        [Authorize(Roles = "Admin,customerService")]
        public async Task<IActionResult> Dashboard()
        {
            // Check if user has proper permissions
            var isSupportOrAdmin = await IsSupportRoleAsync();
            if (!isSupportOrAdmin)
            {
                return Forbid();
            }

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