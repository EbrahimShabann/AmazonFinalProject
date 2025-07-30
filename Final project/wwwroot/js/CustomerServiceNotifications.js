// Customer Service Notification System
class CustomerServiceNotifications {
    constructor() {
        this.notifications = [];
        this.soundEnabled = true;
        this.init();
    }

    init() {
        // Create notification container
        this.createNotificationContainer();
        
        // Set up notification permissions
        this.requestNotificationPermission();
        
        // Initialize sound
        this.initSound();
    }

    createNotificationContainer() {
        const container = document.createElement('div');
        container.id = 'customer-service-notifications';
        container.className = 'position-fixed';
        container.style.cssText = 'top: 20px; right: 20px; z-index: 9999; max-width: 350px;';
        document.body.appendChild(container);
    }

    async requestNotificationPermission() {
        if ('Notification' in window) {
            const permission = await Notification.requestPermission();
            console.log('Notification permission:', permission);
        }
    }

    initSound() {
        // Create audio context for notification sounds
        this.audioContext = new (window.AudioContext || window.webkitAudioContext)();
    }

    // Show notification
    show(message, type = 'info', duration = 5000) {
        const notification = this.createNotificationElement(message, type);
        const container = document.getElementById('customer-service-notifications');
        
        if (container) {
            container.appendChild(notification);
            
            // Play sound
            this.playNotificationSound(type);
            
            // Auto-remove after duration
            setTimeout(() => {
                if (notification.parentNode) {
                    notification.remove();
                }
            }, duration);
            
            // Store notification
            this.notifications.push(notification);
        }
    }

    createNotificationElement(message, type) {
        const notification = document.createElement('div');
        notification.className = `alert alert-${type} alert-dismissible fade show mb-2`;
        notification.style.cssText = 'min-width: 300px; box-shadow: 0 4px 12px rgba(0,0,0,0.15);';
        
        const icon = this.getNotificationIcon(type);
        notification.innerHTML = `
            <i class="${icon} me-2"></i>
            ${this.escapeHtml(message)}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;
        
        return notification;
    }

    getNotificationIcon(type) {
        const icons = {
            'success': 'fas fa-check-circle',
            'info': 'fas fa-info-circle',
            'warning': 'fas fa-exclamation-triangle',
            'danger': 'fas fa-exclamation-circle',
            'ticket': 'fas fa-ticket-alt',
            'chat': 'fas fa-comments',
            'message': 'fas fa-envelope'
        };
        
        return icons[type] || icons['info'];
    }

    playNotificationSound(type) {
        if (!this.soundEnabled) return;
        
        try {
            // Create a simple notification sound
            const oscillator = this.audioContext.createOscillator();
            const gainNode = this.audioContext.createGain();
            
            oscillator.connect(gainNode);
            gainNode.connect(this.audioContext.destination);
            
            // Different frequencies for different notification types
            const frequencies = {
                'success': 800,
                'info': 600,
                'warning': 400,
                'danger': 300,
                'ticket': 700,
                'chat': 500,
                'message': 650
            };
            
            oscillator.frequency.setValueAtTime(frequencies[type] || 600, this.audioContext.currentTime);
            gainNode.gain.setValueAtTime(0.1, this.audioContext.currentTime);
            gainNode.gain.exponentialRampToValueAtTime(0.01, this.audioContext.currentTime + 0.5);
            
            oscillator.start(this.audioContext.currentTime);
            oscillator.stop(this.audioContext.currentTime + 0.5);
        } catch (error) {
            console.log('Could not play notification sound:', error);
        }
    }

    // Show browser notification
    showBrowserNotification(title, message, icon = null) {
        if ('Notification' in window && Notification.permission === 'granted') {
            const notification = new Notification(title, {
                body: message,
                icon: icon || '/favicon.ico',
                badge: '/favicon.ico',
                tag: 'customer-service'
            });
            
            notification.onclick = function() {
                window.focus();
                notification.close();
            };
            
            setTimeout(() => notification.close(), 5000);
        }
    }

    // Notify about new ticket
    notifyNewTicket(ticket) {
        const message = `New ticket: ${ticket.Subject} by ${ticket.UserName}`;
        this.show(message, 'ticket');
        this.showBrowserNotification('New Support Ticket', message);
    }

    // Notify about new chat message
    notifyNewChatMessage(session, senderName) {
        const message = `New message from ${senderName}`;
        this.show(message, 'chat');
        this.showBrowserNotification('New Chat Message', message);
    }

    // Notify about ticket status change
    notifyTicketStatusChange(ticketId, oldStatus, newStatus) {
        const message = `Ticket status changed from ${oldStatus} to ${newStatus}`;
        this.show(message, 'info');
    }

    // Notify about new message in ticket
    notifyNewTicketMessage(ticketId, senderName) {
        const message = `New message in ticket from ${senderName}`;
        this.show(message, 'message');
        this.showBrowserNotification('New Ticket Message', message);
    }

    // Update unread count badge
    updateUnreadBadge(count) {
        let badge = document.getElementById('unread-badge');
        if (!badge) {
            badge = document.createElement('span');
            badge.id = 'unread-badge';
            badge.className = 'badge bg-danger position-absolute';
            badge.style.cssText = 'top: -5px; right: -5px; font-size: 0.7rem;';
            
            const customerServiceLink = document.querySelector('a[href*="CustomerService"]');
            if (customerServiceLink) {
                customerServiceLink.style.position = 'relative';
                customerServiceLink.appendChild(badge);
            }
        }
        
        if (count > 0) {
            badge.textContent = count > 99 ? '99+' : count;
            badge.style.display = 'block';
        } else {
            badge.style.display = 'none';
        }
    }

    // Toggle sound
    toggleSound() {
        this.soundEnabled = !this.soundEnabled;
        localStorage.setItem('customerServiceSoundEnabled', this.soundEnabled);
        
        const message = this.soundEnabled ? 'Sound notifications enabled' : 'Sound notifications disabled';
        this.show(message, 'info', 2000);
    }

    // Clear all notifications
    clearAll() {
        const container = document.getElementById('customer-service-notifications');
        if (container) {
            container.innerHTML = '';
        }
        this.notifications = [];
    }

    // Escape HTML to prevent XSS
    escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    // Initialize from localStorage
    loadSettings() {
        const soundEnabled = localStorage.getItem('customerServiceSoundEnabled');
        if (soundEnabled !== null) {
            this.soundEnabled = soundEnabled === 'true';
        }
    }
}

// Initialize notification system
document.addEventListener('DOMContentLoaded', function() {
    window.customerServiceNotifications = new CustomerServiceNotifications();
    window.customerServiceNotifications.loadSettings();
});

// Export for use in other scripts
window.CustomerServiceNotifications = CustomerServiceNotifications; 