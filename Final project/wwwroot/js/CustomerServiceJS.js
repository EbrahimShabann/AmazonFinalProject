// Customer Service JavaScript for real-time functionality
class CustomerServiceManager {
    constructor() {
        this.connection = null;
        this.currentTicketId = null;
        this.currentSessionId = null;
        this.currentUserId = null;
        this.init();
    }

    async init() {
        // Initialize SignalR connection
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/customerServiceHub")
            .withAutomaticReconnect()
            .build();

        // Set up event handlers
        this.setupEventHandlers();
        
        // Start connection
        try {
            await this.connection.start();
            console.log("Customer Service Hub connected");
        } catch (err) {
            console.error("Error connecting to Customer Service Hub:", err);
        }
    }

    setupEventHandlers() {
        // Ticket message received
        this.connection.on("ReceiveTicketMessage", (message) => {
            this.displayTicketMessage(message);
        });

        // Chat message received
        this.connection.on("ReceiveChatMessage", (message) => {
            this.displayChatMessage(message);
        });

        // Messages marked as read
        this.connection.on("MessagesMarkedAsRead", (userId) => {
            this.updateMessageReadStatus(userId);
        });

        // Chat messages marked as read
        this.connection.on("ChatMessagesMarkedAsRead", (userId) => {
            this.updateChatMessageReadStatus(userId);
        });

        // Ticket status updated
        this.connection.on("TicketStatusUpdated", (update) => {
            this.updateTicketStatus(update);
        });

        // New ticket notification
        this.connection.on("NewTicketCreated", (ticket) => {
            this.showNewTicketNotification(ticket);
            if (window.customerServiceNotifications) {
                window.customerServiceNotifications.notifyNewTicket(ticket);
            }
        });

        // New chat session notification
        this.connection.on("NewChatSessionCreated", (session) => {
            this.showNewChatNotification(session);
            if (window.customerServiceNotifications) {
                window.customerServiceNotifications.show(`New chat session: ${session.CustomerName} with ${session.SellerName}`, 'chat');
            }
        });
    }

    // Join ticket group for real-time updates
    async joinTicketGroup(ticketId) {
        if (this.connection && this.connection.state === signalR.HubConnectionState.Connected) {
            await this.connection.invoke("JoinTicketGroup", ticketId);
            this.currentTicketId = ticketId;
        }
    }

    // Leave ticket group
    async leaveTicketGroup(ticketId) {
        if (this.connection && this.connection.state === signalR.HubConnectionState.Connected) {
            await this.connection.invoke("LeaveTicketGroup", ticketId);
            this.currentTicketId = null;
        }
    }

    // Join chat session group
    async joinChatSession(sessionId) {
        if (this.connection && this.connection.state === signalR.HubConnectionState.Connected) {
            await this.connection.invoke("JoinChatSession", sessionId);
            this.currentSessionId = sessionId;
        }
    }

    // Leave chat session group
    async leaveChatSession(sessionId) {
        if (this.connection && this.connection.state === signalR.HubConnectionState.Connected) {
            await this.connection.invoke("LeaveChatSession", sessionId);
            this.currentSessionId = null;
        }
    }

    // Send ticket message
    async sendTicketMessage(ticketId, message, senderId) {
        if (this.connection && this.connection.state === signalR.HubConnectionState.Connected) {
            await this.connection.invoke("SendTicketMessage", ticketId, message, senderId);
        }
    }

    // Send chat message
    async sendChatMessage(sessionId, message, senderId) {
        if (this.connection && this.connection.state === signalR.HubConnectionState.Connected) {
            await this.connection.invoke("SendChatMessage", sessionId, message, senderId);
        }
    }

    // Mark messages as read
    async markMessagesAsRead(ticketId, userId) {
        if (this.connection && this.connection.state === signalR.HubConnectionState.Connected) {
            await this.connection.invoke("MarkMessagesAsRead", ticketId, userId);
        }
    }

    // Mark chat messages as read
    async markChatMessagesAsRead(sessionId, userId) {
        if (this.connection && this.connection.state === signalR.HubConnectionState.Connected) {
            await this.connection.invoke("MarkChatMessagesAsRead", sessionId, userId);
        }
    }

            // Display ticket message in UI
        displayTicketMessage(message) {
            const messagesContainer = document.getElementById('ticketMessages');
            if (messagesContainer) {
                const messageElement = this.createMessageElement(message, 'ticket');
                messagesContainer.appendChild(messageElement);
                messagesContainer.scrollTop = messagesContainer.scrollHeight;
                
                // Notify about new message if not from current user
                if (message.SenderId !== this.currentUserId && window.customerServiceNotifications) {
                    window.customerServiceNotifications.notifyNewTicketMessage(message.TicketId, message.SenderName);
                }
            }
        }

            // Display chat message in UI
        displayChatMessage(message) {
            const messagesContainer = document.getElementById('chatMessages');
            if (messagesContainer) {
                const messageElement = this.createMessageElement(message, 'chat');
                messagesContainer.appendChild(messageElement);
                messagesContainer.scrollTop = messagesContainer.scrollHeight;
                
                // Notify about new message if not from current user
                if (message.SenderId !== this.currentUserId && window.customerServiceNotifications) {
                    window.customerServiceNotifications.notifyNewChatMessage(message.SessionId, message.SenderName);
                }
            }
        }

    // Create message element
    createMessageElement(message, type) {
        const messageDiv = document.createElement('div');
        messageDiv.className = `message ${message.SenderId === this.currentUserId ? 'own-message' : 'other-message'}`;
        
        const time = new Date(message.SentAt).toLocaleTimeString();
        messageDiv.innerHTML = `
            <div class="message-content">
                <div class="message-text">${this.escapeHtml(message.Message)}</div>
                <div class="message-time">${time}</div>
            </div>
        `;
        
        return messageDiv;
    }

    // Update message read status
    updateMessageReadStatus(userId) {
        const messages = document.querySelectorAll('.message');
        messages.forEach(msg => {
            if (msg.dataset.senderId !== userId) {
                msg.classList.add('read');
            }
        });
    }

    // Update chat message read status
    updateChatMessageReadStatus(userId) {
        const messages = document.querySelectorAll('.chat-message');
        messages.forEach(msg => {
            if (msg.dataset.senderId !== userId) {
                msg.classList.add('read');
            }
        });
    }

    // Update ticket status
    updateTicketStatus(update) {
        const statusElement = document.querySelector('.ticket-status');
        if (statusElement) {
            statusElement.textContent = update.Status;
            statusElement.className = `badge bg-${update.Status === 'Open' ? 'warning' : update.Status === 'Resolved' ? 'success' : 'secondary'}`;
        }
    }

    // Show new ticket notification
    showNewTicketNotification(ticket) {
        this.showNotification(`New ticket created: ${ticket.Subject}`, 'info');
    }

    // Show new chat notification
    showNewChatNotification(session) {
        this.showNotification(`New chat session: ${session.CustomerName} with ${session.SellerName}`, 'info');
    }

    // Show notification
    showNotification(message, type = 'info') {
        const notification = document.createElement('div');
        notification.className = `alert alert-${type} alert-dismissible fade show position-fixed`;
        notification.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
        notification.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;
        
        document.body.appendChild(notification);
        
        // Auto-remove after 5 seconds
        setTimeout(() => {
            if (notification.parentNode) {
                notification.remove();
            }
        }, 5000);
    }

    // Escape HTML to prevent XSS
    escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    // Set current user ID
    setCurrentUserId(userId) {
        this.currentUserId = userId;
    }
}

// Initialize customer service manager when page loads
document.addEventListener('DOMContentLoaded', function() {
    window.customerServiceManager = new CustomerServiceManager();
    
    // Set up form handlers
    setupTicketFormHandlers();
    setupChatFormHandlers();
});

// Setup ticket form handlers
function setupTicketFormHandlers() {
    const ticketForm = document.getElementById('ticketMessageForm');
    if (ticketForm) {
        ticketForm.addEventListener('submit', async function(e) {
            e.preventDefault();
            
            const messageInput = document.getElementById('ticketMessage');
            const message = messageInput.value.trim();
            const ticketId = this.dataset.ticketId;
            const senderId = this.dataset.senderId;
            
            if (message && ticketId && senderId) {
                await window.customerServiceManager.sendTicketMessage(ticketId, message, senderId);
                messageInput.value = '';
            }
        });
    }
}

// Setup chat form handlers
function setupChatFormHandlers() {
    const chatForm = document.getElementById('chatMessageForm');
    if (chatForm) {
        chatForm.addEventListener('submit', async function(e) {
            e.preventDefault();
            
            const messageInput = document.getElementById('chatMessage');
            const message = messageInput.value.trim();
            const sessionId = this.dataset.sessionId;
            const senderId = this.dataset.senderId;
            
            if (message && sessionId && senderId) {
                await window.customerServiceManager.sendChatMessage(sessionId, message, senderId);
                messageInput.value = '';
            }
        });
    }
}

// Join ticket group when entering ticket details page
function joinTicketGroup(ticketId) {
    if (window.customerServiceManager) {
        window.customerServiceManager.joinTicketGroup(ticketId);
    }
}

// Join chat session when entering chat details page
function joinChatSession(sessionId) {
    if (window.customerServiceManager) {
        window.customerServiceManager.joinChatSession(sessionId);
    }
}

// Set current user ID
function setCurrentUserId(userId) {
    if (window.customerServiceManager) {
        window.customerServiceManager.setCurrentUserId(userId);
    }
} 