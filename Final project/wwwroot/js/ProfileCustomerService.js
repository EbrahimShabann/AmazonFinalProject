// Profile Customer Service Integration
class ProfileCustomerService {
    constructor() {
        this.notificationCount = 0;
        this.init();
    }

    init() {
        this.loadNotificationCount();
        this.setupRealTimeUpdates();
        this.setupEventListeners();
    }

    loadNotificationCount() {
        fetch('/Customer/Profile/GetCustomerServiceNotificationCount')
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    this.notificationCount = data.count;
                    this.updateNotificationBadge();
                }
            })
            .catch(error => {
                console.error('Error loading notification count:', error);
            });
    }

    updateNotificationBadge() {
        let badge = document.getElementById('customer-service-notification-badge');
        if (!badge) {
            badge = document.createElement('span');
            badge.id = 'customer-service-notification-badge';
            badge.className = 'badge bg-danger position-absolute';
            badge.style.cssText = 'top: -5px; right: -5px; font-size: 0.7rem;';
            
            const customerServiceLinks = document.querySelectorAll('a[href*="CustomerService"]');
            customerServiceLinks.forEach(link => {
                link.style.position = 'relative';
                link.appendChild(badge.cloneNode(true));
            });
        }

        const badges = document.querySelectorAll('#customer-service-notification-badge');
        badges.forEach(badge => {
            if (this.notificationCount > 0) {
                badge.textContent = this.notificationCount > 99 ? '99+' : this.notificationCount;
                badge.style.display = 'block';
            } else {
                badge.style.display = 'none';
            }
        });
    }

    setupRealTimeUpdates() {
        // Connect to SignalR hub for real-time updates
        if (typeof signalR !== 'undefined') {
            const connection = new signalR.HubConnectionBuilder()
                .withUrl("/customerServiceHub")
                .build();

            connection.start().then(() => {
                console.log('Connected to CustomerServiceHub from profile');
                
                // Listen for new tickets
                connection.on("NewTicketCreated", (ticket) => {
                    this.notificationCount++;
                    this.updateNotificationBadge();
                    this.showNotification('New support ticket created', 'info');
                });

                // Listen for new messages
                connection.on("ReceiveTicketMessage", (message) => {
                    this.notificationCount++;
                    this.updateNotificationBadge();
                    this.showNotification('New message in support ticket', 'info');
                });

                connection.on("ReceiveChatMessage", (message) => {
                    this.notificationCount++;
                    this.updateNotificationBadge();
                    this.showNotification('New chat message received', 'info');
                });

                // Listen for status updates
                connection.on("TicketStatusUpdated", (update) => {
                    this.showNotification(`Ticket status updated to ${update.Status}`, 'success');
                    this.loadNotificationCount(); // Refresh count
                });

            }).catch(err => {
                console.error('Error connecting to CustomerServiceHub:', err);
            });
        }
    }

    setupEventListeners() {
        // Handle modal form submission
        const createTicketForm = document.getElementById('createTicketForm');
        if (createTicketForm) {
            createTicketForm.addEventListener('submit', this.handleCreateTicket.bind(this));
        }

        // Handle quick action buttons
        const quickActionButtons = document.querySelectorAll('[data-customer-service-action]');
        quickActionButtons.forEach(button => {
            button.addEventListener('click', this.handleQuickAction.bind(this));
        });
    }

    handleCreateTicket(event) {
        event.preventDefault();
        
        const formData = new FormData(event.target);
        
        fetch('/CustomerService/CreateTicket', {
            method: 'POST',
            body: formData
        })
        .then(response => {
            if (response.ok) {
                this.showNotification('Support ticket created successfully!', 'success');
                this.loadNotificationCount();
                
                // Close modal
                const modal = bootstrap.Modal.getInstance(document.getElementById('createSupportTicketModal'));
                if (modal) {
                    modal.hide();
                }
                
                // Reset form
                event.target.reset();
            } else {
                throw new Error('Failed to create ticket');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            this.showNotification('Failed to create support ticket. Please try again.', 'danger');
        });
    }

    handleQuickAction(event) {
        const action = event.target.dataset.customerServiceAction;
        
        switch (action) {
            case 'create-ticket':
                const modal = new bootstrap.Modal(document.getElementById('createSupportTicketModal'));
                modal.show();
                break;
            case 'view-tickets':
                window.location.href = '/CustomerService/Index';
                break;
            case 'open-chat':
                window.location.href = '/CustomerService/Chats';
                break;
            default:
                console.log('Unknown action:', action);
        }
    }

    showNotification(message, type = 'info') {
        if (window.customerServiceNotifications) {
            window.customerServiceNotifications.show(message, type);
        } else {
            // Fallback notification
            const alertDiv = document.createElement('div');
            alertDiv.className = `alert alert-${type} alert-dismissible fade show position-fixed`;
            alertDiv.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
            alertDiv.innerHTML = `
                ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            `;
            document.body.appendChild(alertDiv);
            
            setTimeout(() => {
                if (alertDiv.parentNode) {
                    alertDiv.remove();
                }
            }, 5000);
        }
    }

    refreshStats() {
        // Reload customer service statistics
        const statsElements = document.querySelectorAll('[id$="Tickets"], [id$="Chats"], [id$="Messages"]');
        if (statsElements.length > 0) {
            this.loadNotificationCount();
        }
    }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    window.profileCustomerService = new ProfileCustomerService();
});

// Export for use in other scripts
window.ProfileCustomerService = ProfileCustomerService; 