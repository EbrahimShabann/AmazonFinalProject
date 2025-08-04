// EnhancedChatbot.js - Amazon Chatbot Functionality
// This enhanced version includes better UI, product recommendations, and improved functionality

class EnhancedChatbot {
    constructor() {
        // Core variables
        this.chatHistory = [];
        this.previousChats = [];
        this.selectedModel = "meta-llama/llama-4-scout-17b-16e-instruct";
        this.isAuthenticated = false;
        this.isMinimized = false;
        this.isConnected = true;
        this.messageQueue = [];
        this.isProcessing = false;
        this.suggestions = [
            "Show me today's deals",
            "Track my orders",
            "Find electronics under $100",
            "Help me find a gift",
            "What's new in fashion?",
            "Show me bestsellers",
            "Find books by genre",
            "Search for home decor"
        ];

        // DOM Elements
        this.elements = {
            chatbotToggle: document.getElementById('chatbot-toggle'),
            chatbotContainer: document.getElementById('chatbot-container'),
            chatbotClose: document.getElementById('chatbot-close'),
            chatbotMinimize: document.getElementById('chatbot-minimize'),
            sendBtn: document.getElementById('send-btn'),
            messageInput: document.getElementById('message-input'),
            newChatBtn: document.getElementById('new-chat-btn'),
            clearChatBtn: document.getElementById('clear-chat-btn'),
            chatWindow: document.getElementById('chat-window'),
            previousChatsSelect: document.getElementById('previous-chats'),
            voiceBtn: document.getElementById('voice-btn'),
            inputSuggestions: document.getElementById('input-suggestions'),
            quickActions: document.getElementById('quick-actions'),
            connectionStatus: document.getElementById('connection-status'),
            statusDot: document.getElementById('status-dot'),
            statusText: document.getElementById('status-text'),
            chatNotification: document.getElementById('chat-notification')
        };

        this.init();
    }

    init() {
        this.setupEventListeners();
        this.setupInputBehavior();
        this.setupVoiceInput();
        this.setupQuickActions();
        this.checkConnection();
        this.loadChatHistory();
        this.addWelcomeMessage();

        // Set authentication status (if available globally)
        if (typeof window.isUserAuthenticated !== 'undefined') {
            this.isAuthenticated = window.isUserAuthenticated;
        }

        // Auto-pulse button for first-time users
        this.showInitialPulse();
    }

    setupEventListeners() {
        // Toggle chatbot
        this.elements.chatbotToggle?.addEventListener('click', (e) => {
            e.preventDefault();
            e.stopPropagation();
            this.toggleChatbot();
        });

        // Close chatbot
        this.elements.chatbotClose?.addEventListener('click', (e) => {
            e.preventDefault();
            e.stopPropagation();
            this.closeChatbot();
        });

        // Minimize chatbot
        this.elements.chatbotMinimize?.addEventListener('click', (e) => {
            e.preventDefault();
            e.stopPropagation();
            this.minimizeChatbot();
        });

        // Send message
        this.elements.sendBtn?.addEventListener('click', () => this.handleSendMessage());

        // Input events
        this.elements.messageInput?.addEventListener('keydown', (e) => this.handleKeyPress(e));
        this.elements.messageInput?.addEventListener('input', () => this.handleInputChange());
        this.elements.messageInput?.addEventListener('focus', () => this.showSuggestions());
        this.elements.messageInput?.addEventListener('blur', () => this.hideSuggestions());

        // Chat controls
        this.elements.newChatBtn?.addEventListener('click', () => this.startNewChat());
        this.elements.clearChatBtn?.addEventListener('click', () => this.clearCurrentChat());
        this.elements.previousChatsSelect?.addEventListener('change', (e) => this.loadPreviousChat(e));

        // Outside click handler
        document.addEventListener('click', (e) => this.handleOutsideClick(e));

        // Keyboard shortcuts
        document.addEventListener('keydown', (e) => this.handleGlobalKeyboard(e));

        // Window events
        window.addEventListener('beforeunload', () => this.saveChatHistory());
        window.addEventListener('online', () => this.updateConnectionStatus(true));
        window.addEventListener('offline', () => this.updateConnectionStatus(false));
    }

    setupInputBehavior() {
        // Auto-resize textarea
        this.elements.messageInput?.addEventListener('input', () => {
            this.elements.messageInput.style.height = 'auto';
            this.elements.messageInput.style.height = Math.min(this.elements.messageInput.scrollHeight, 120) + 'px';
        });

        // Character counter (optional)
        this.elements.messageInput?.addEventListener('input', () => {
            const length = this.elements.messageInput.value.length;
            if (length > 950) {
                this.elements.messageInput.style.borderColor = '#dc3545';
            } else {
                this.elements.messageInput.style.borderColor = '';
            }
        });
    }

    setupVoiceInput() {
        if (this.elements.voiceBtn && ('webkitSpeechRecognition' in window || 'SpeechRecognition' in window)) {
            this.speechRecognition = new (window.SpeechRecognition || window.webkitSpeechRecognition)();
            this.speechRecognition.continuous = false;
            this.speechRecognition.interimResults = false;
            this.speechRecognition.lang = 'en-US';

            this.speechRecognition.onstart = () => {
                this.elements.voiceBtn.classList.add('recording');
                this.elements.voiceBtn.innerHTML = '<i class="fas fa-stop"></i>';
            };

            this.speechRecognition.onend = () => {
                this.elements.voiceBtn.classList.remove('recording');
                this.elements.voiceBtn.innerHTML = '<i class="fas fa-microphone"></i>';
            };

            this.speechRecognition.onresult = (event) => {
                const transcript = event.results[0][0].transcript;
                this.elements.messageInput.value = transcript;
                this.handleSendMessage();
            };

            this.elements.voiceBtn.addEventListener('click', () => {
                if (this.elements.voiceBtn.classList.contains('recording')) {
                    this.speechRecognition.stop();
                } else {
                    this.speechRecognition.start();
                }
            });
        } else if (this.elements.voiceBtn) {
            this.elements.voiceBtn.style.display = 'none';
        }
    }

    setupQuickActions() {
        this.elements.quickActions?.addEventListener('click', (e) => {
            if (e.target.classList.contains('quick-action')) {
                const message = e.target.dataset.message;
                if (message) {
                    this.elements.messageInput.value = message;
                    this.handleSendMessage();
                }
            }
        });
    }

    toggleChatbot() {
        const isVisible = this.elements.chatbotContainer?.classList.contains('show');
        if (isVisible) {
            this.closeChatbot();
        } else {
            this.openChatbot();
        }
    }

    openChatbot() {
        if (!this.elements.chatbotContainer || !this.elements.chatbotToggle) return;

        this.elements.chatbotContainer.classList.remove('minimized');
        this.elements.chatbotContainer.classList.add('show');
        this.elements.chatbotToggle.classList.add('active');
        this.elements.chatbotToggle.innerHTML = '<i class="fas fa-times"></i>';
        this.elements.chatbotContainer.setAttribute('aria-hidden', 'false');
        this.elements.messageInput?.focus();
        this.hideNotification();
        this.isMinimized = false;

        // Track analytics (if needed)
        this.trackEvent('chatbot_opened');
    }

    closeChatbot() {
        if (!this.elements.chatbotContainer || !this.elements.chatbotToggle) return;

        this.elements.chatbotContainer.classList.remove('show', 'minimized');
        this.elements.chatbotToggle.classList.remove('active');
        this.elements.chatbotToggle.innerHTML = '<i class="fas fa-comments"></i>';
        this.elements.chatbotContainer.setAttribute('aria-hidden', 'true');
        this.hideSuggestions();
        this.isMinimized = false;

        // Track analytics (if needed)
        this.trackEvent('chatbot_closed');
    }

    minimizeChatbot() {
        if (!this.elements.chatbotContainer) return;

        this.elements.chatbotContainer.classList.add('minimized');
        this.isMinimized = true;

        // Track analytics (if needed)
        this.trackEvent('chatbot_minimized');
    }

    async handleSendMessage() {
        const message = this.elements.messageInput?.value.trim();
        if (message && this.elements.sendBtn && !this.isProcessing) {
            await this.sendMessage(message);
            this.elements.messageInput.value = '';
            this.elements.messageInput.style.height = 'auto';
            this.hideSuggestions();
        }
    }

    handleKeyPress(e) {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            this.handleSendMessage();
        } else if (e.key === 'Escape') {
            this.hideSuggestions();
        } else if (e.key === 'ArrowUp' || e.key === 'ArrowDown') {
            this.navigateSuggestions(e);
        }
    }

    handleInputChange() {
        const value = this.elements.messageInput?.value.toLowerCase();
        if (value && value.length > 2) {
            this.updateSuggestions(value);
        } else {
            this.hideSuggestions();
        }
    }

    handleGlobalKeyboard(e) {
        // Ctrl/Cmd + K to open chatbot
        if ((e.ctrlKey || e.metaKey) && e.key === 'k' &&
            this.elements.chatbotContainer &&
            !this.elements.chatbotContainer.classList.contains('show')) {
            e.preventDefault();
            this.openChatbot();
        }

        // Escape to close chatbot
        if (e.key === 'Escape' && this.elements.chatbotContainer?.classList.contains('show')) {
            this.closeChatbot();
        }
    }

    async sendMessage(userMessage) {
        if (!userMessage || !this.elements.chatWindow || !this.elements.sendBtn) return;

        this.isProcessing = true;

        // Add user message to chat
        this.addMessageToChat('user', userMessage);

        // Show typing indicator
        this.showTypingIndicator();

        // Disable send button
        this.elements.sendBtn.disabled = true;

        try {
            const response = await fetch('/AIChatbot/Ask', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: JSON.stringify({
                    message: userMessage,
                    model: this.selectedModel,
                    temperature: 0.7,
                    conversationHistory: this.chatHistory.slice(-10) // Send last 10 messages for context
                })
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const data = await response.json();

            // Remove typing indicator
            this.removeTypingIndicator();

            if (data.reply) {
                // Check if response suggests products and fetch them
                if (this.shouldFetchProducts(data.reply, userMessage)) {
                    await this.handleProductRecommendation(data.reply, userMessage);
                } else {
                    this.addMessageToChat('assistant', data.reply);
                }

                // Update connection status
                this.updateConnectionStatus(true);
            } else {
                this.addMessageToChat('assistant', 'Sorry, I encountered an error. Please try again.');
            }

        } catch (error) {
            console.error('Error:', error);
            this.removeTypingIndicator();

            let errorMessage = 'Sorry, I\'m having trouble connecting. Please try again later.';
            if (error.message.includes('404')) {
                errorMessage = 'The chat service is temporarily unavailable. Please try again later.';
            } else if (error.message.includes('500')) {
                errorMessage = 'There was a server error. Please try again in a moment.';
            }

            this.addMessageToChat('assistant', errorMessage);
            this.updateConnectionStatus(false);
        } finally {
            if (this.elements.sendBtn) {
                this.elements.sendBtn.disabled = false;
            }
            this.isProcessing = false;
        }
    }

    addMessageToChat(role, content, products = null) {
        const timestamp = new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
        const message = {
            role,
            content,
            timestamp,
            products: products,
            id: Date.now() + Math.random()
        };

        this.chatHistory.push(message);
        this.displayChat();
        this.saveChatHistory();

        // Show notification if chatbot is closed
        if (this.elements.chatbotContainer &&
            !this.elements.chatbotContainer.classList.contains('show') &&
            role === 'assistant' &&
            this.elements.chatNotification) {
            this.showNotification();
        }

        // Track analytics (if needed)
        this.trackEvent('message_sent', { role, hasProducts: !!products });
    }

    shouldFetchProducts(reply, userMessage) {
        const productKeywords = [
            'product', 'recommend', 'suggest', 'show', 'find', 'search', 'buy', 'purchase',
            'item', 'price', 'deal', 'sale', 'discount', 'electronics', 'fashion', 'books',
            'home', 'garden', 'kitchen', 'tech', 'gadget', 'phone', 'laptop', 'tablet'
        ];

        const replyLower = reply.toLowerCase();
        const messageLower = userMessage.toLowerCase();

        return productKeywords.some(keyword =>
            replyLower.includes(keyword) || messageLower.includes(keyword)
        ) && !messageLower.includes('track') && !messageLower.includes('order');
    }

    async handleProductRecommendation(reply, userMessage) {
        // Add the AI response first
        this.addMessageToChat('assistant', reply);

        try {
            // Show loading for products
            this.showProductLoading();

            // Fetch products from your existing product endpoint
            const products = await this.fetchRecommendedProducts(userMessage);

            // Remove loading
            this.removeProductLoading();

            if (products && products.length > 0) {
                // Update the last message with products
                const lastMessageIndex = this.chatHistory.length - 1;
                this.chatHistory[lastMessageIndex].products = products.slice(0, 4); // Limit to 4 products
                this.displayChat();
            }
        } catch (error) {
            console.error('Error fetching products:', error);
            this.removeProductLoading();
        }
    }

    async fetchRecommendedProducts(query) {
        try {
            // Use jQuery AJAX to match your existing pattern
            return new Promise((resolve, reject) => {
                $.ajax({
                    url: "/Landing/Search",
                    type: "GET",
                    data: {
                        query: query,
                        pageSize: 8,
                        pageNumber: 1
                    },
                    success: function (result) {
                        console.log("Raw product search results:", result); // DEBUG LOG

                        // FIXED: Better data transformation with logging
                        const transformedProducts = result.map(product => {
                            console.log("Original product data:", product); // DEBUG LOG

                            const transformed = {
                                id: product.productId || product.id,
                                productId: product.productId || product.id,
                                name: product.productName || product.name,
                                productName: product.productName || product.name,
                                brand: product.brand || product.Brand || 'Amazon',
                                Brand: product.brand || product.Brand || 'Amazon',
                                price: product.price,
                                discount_price: product.discountPrice,
                                discountPrice: product.discountPrice,
                                imageUrl: product.imageUrl || product.image_url,
                                image_url: product.imageUrl || product.image_url,
                                rating: product.rating || 4,
                                reviewCount: product.reviewCount || product.ReviewsCount || 100,
                                ReviewsCount: product.reviewCount || product.ReviewsCount || 100
                            };

                            console.log("Transformed product:", transformed); // DEBUG LOG
                            return transformed;
                        });

                        console.log("All transformed products:", transformedProducts); // DEBUG LOG
                        resolve(transformedProducts);
                    },
                    error: function (xhr, status, error) {
                        console.error("Product search error:", error);
                        reject(new Error(error));
                    }
                });
            });
        } catch (error) {
            console.error('Error in fetchRecommendedProducts:', error);
            return [];
        }
    }

    showTypingIndicator() {
        if (!this.elements.chatWindow) return;

        const typingDiv = document.createElement('div');
        typingDiv.className = 'message assistant typing-message';
        typingDiv.innerHTML = `
            <div class="typing-indicator">
                <div class="typing-dot"></div>
                <div class="typing-dot"></div>
                <div class="typing-dot"></div>
            </div>
        `;
        this.elements.chatWindow.appendChild(typingDiv);
        this.scrollToBottom();
    }

    removeTypingIndicator() {
        if (!this.elements.chatWindow) return;

        const typingMessage = this.elements.chatWindow.querySelector('.typing-message');
        if (typingMessage) {
            typingMessage.remove();
        }
    }

    showProductLoading() {
        if (!this.elements.chatWindow) return;

        const loadingDiv = document.createElement('div');
        loadingDiv.className = 'message assistant product-loading-message';
        loadingDiv.innerHTML = `
            <div class="message-bubble">
                <i class="fas fa-spinner fa-spin"></i> Finding relevant products...
            </div>
        `;
        this.elements.chatWindow.appendChild(loadingDiv);
        this.scrollToBottom();
    }

    removeProductLoading() {
        if (!this.elements.chatWindow) return;

        const loadingMessage = this.elements.chatWindow.querySelector('.product-loading-message');
        if (loadingMessage) {
            loadingMessage.remove();
        }
    }

    displayChat() {
        if (!this.elements.chatWindow) return;

        this.elements.chatWindow.innerHTML = '';

        this.chatHistory.forEach((msg, index) => {
            const messageDiv = document.createElement('div');
            messageDiv.className = `message ${msg.role}`;
            messageDiv.setAttribute('data-message-id', msg.id);

            if (msg.role === 'user') {
                messageDiv.innerHTML = `
                <div class="message-bubble">
                    ${this.escapeHtml(msg.content)}
                </div>
                <div class="message-timestamp">${msg.timestamp}</div>
            `;
            } else if (msg.role === 'assistant') {
                let messageHtml = `
                <div class="message-bubble">
                    ${this.formatMessage(msg.content)}
                </div>
                <div class="message-timestamp">${msg.timestamp}</div>
            `;

                // FIXED: Add products in proper container structure
                if (msg.products && msg.products.length > 0) {
                    messageHtml += '<div class="product-recommendations-container">';
                    msg.products.forEach(product => {
                        messageHtml += this.createProductRecommendationHtml(product);
                    });
                    messageHtml += '</div>';
                }

                messageDiv.innerHTML = messageHtml;
            }

            this.elements.chatWindow.appendChild(messageDiv);

            // Animate message entrance
            setTimeout(() => {
                messageDiv.style.animationDelay = `${index * 50}ms`;
            }, 10);
        });

        this.scrollToBottom();
    }
    formatMessage(content) {
        // Convert URLs to clickable links
        const urlRegex = /(https?:\/\/[^\s]+)/g;
        content = content.replace(urlRegex, '<a href="$1" target="_blank" rel="noopener noreferrer">$1</a>');

        // Convert line breaks
        content = content.replace(/\n/g, '<br>');

        // Convert **bold** to <strong>
        content = content.replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>');

        // Convert *italic* to <em>
        content = content.replace(/\*(.*?)\*/g, '<em>$1</em>');

        return content;
    }

    createProductRecommendationHtml(product) {
        console.log('Creating product HTML for:', product); // DEBUG LOG

        // FIXED: Better property handling with multiple fallbacks
        const displayPrice = product.discountPrice || product.discount_price || product.price;
        const originalPrice = (product.discountPrice || product.discount_price) ? product.price : null;

        // FIXED: Multiple image URL fallbacks and better error handling
        let imageUrl = product.imageUrl || product.image_url || product.ImageUrl || product.Image;

        // If no image URL, use placeholder
        if (!imageUrl || imageUrl === '' || imageUrl === 'null') {
            imageUrl = 'https://via.placeholder.com/60x60/f5f5f5/999?text=No+Image';
        }

        // FIXED: Better property handling
        const rating = product.rating || product.Rating || Math.floor(Math.random() * 2) + 4;
        const reviewCount = product.reviewCount || product.ReviewsCount || product.review_count || Math.floor(Math.random() * 1000) + 100;
        const productName = product.productName || product.name || product.Name || 'Product';
        const productBrand = product.Brand || product.brand || product.brandName || 'Amazon';
        const productId = product.productId || product.id || product.Id;

        console.log('Image URL being used:', imageUrl); // DEBUG LOG

        return `
        <div class="product-recommendation" onclick="window.redirectToProduct('${productId}')"
             role="button" tabindex="0" aria-label="View product: ${productName}">
            <div class="product-info">
                <img src="${imageUrl}" 
                     alt="${productName}" 
                     class="product-image"
                     onerror="console.log('Image failed to load:', this.src); this.src='https://via.placeholder.com/60x60/f5f5f5/999?text=No+Image';" 
                     onload="console.log('Image loaded successfully:', this.src);"
                     loading="lazy">
                <div class="product-details">
                    <div class="product-brand">${productBrand}</div>
                    <div class="product-name">${productName}</div>
                    <div class="product-rating">
                        <span class="stars">${'★'.repeat(Math.floor(rating))}${'☆'.repeat(5 - Math.floor(rating))}</span>
                        <span>(${reviewCount})</span>
                    </div>
                    <div class="product-price">
                        $${displayPrice}
                        ${originalPrice ? `<span style="text-decoration: line-through; color: #999; font-size: 12px; margin-left: 8px;">$${originalPrice}</span>` : ''}
                    </div>
                </div>
                <i class="fas fa-chevron-right" style="color: #ccc;"></i>
            </div>
        </div>
    `;
    }
    startNewChat() {
        if (this.chatHistory.length > 1) { // More than just welcome message
            // Save current chat to history
            const chatSummary = this.chatHistory
                .filter(msg => msg.role === 'user')
                .map(msg => msg.content.substring(0, 30))
                .join(' | ') || 'Chat ' + (this.previousChats.length + 1);

            this.previousChats.unshift({
                messages: [...this.chatHistory],
                summary: chatSummary,
                timestamp: new Date().toLocaleString(),
                id: Date.now()
            });

            // Keep only last 10 chats
            if (this.previousChats.length > 10) {
                this.previousChats = this.previousChats.slice(0, 10);
            }

            this.updatePreviousChatsDropdown();
        }

        // Clear current chat
        this.chatHistory = [];
        this.addWelcomeMessage();

        this.trackEvent('new_chat_started');
    }

    clearCurrentChat() {
        if (confirm('Are you sure you want to clear this conversation?')) {
            this.chatHistory = [];
            this.addWelcomeMessage();
            this.trackEvent('chat_cleared');
        }
    }

    loadPreviousChat(e) {
        const selectedId = parseInt(e.target.value);
        const selectedChat = this.previousChats.find(chat => chat.id === selectedId);

        if (selectedChat) {
            this.chatHistory = [...selectedChat.messages];
            this.displayChat();
            this.trackEvent('previous_chat_loaded');
        }
    }

    updatePreviousChatsDropdown() {
        if (!this.elements.previousChatsSelect) return;

        this.elements.previousChatsSelect.innerHTML = '<option disabled selected>Previous chats...</option>';

        this.previousChats.forEach(chat => {
            const option = document.createElement('option');
            option.value = chat.id;
            option.textContent = `${chat.summary} (${chat.timestamp})`;
            this.elements.previousChatsSelect.appendChild(option);
        });
    }

    showSuggestions() {
        if (this.suggestions.length > 0 && this.elements.inputSuggestions) {
            this.elements.inputSuggestions.style.display = 'block';
            this.updateSuggestions('');
        }
    }

    hideSuggestions() {
        if (this.elements.inputSuggestions) {
            setTimeout(() => {
                this.elements.inputSuggestions.style.display = 'none';
            }, 200);
        }
    }

    updateSuggestions(query) {
        if (!this.elements.inputSuggestions) return;

        const filteredSuggestions = this.suggestions.filter(suggestion =>
            suggestion.toLowerCase().includes(query.toLowerCase())
        );

        this.elements.inputSuggestions.innerHTML = '';

        filteredSuggestions.slice(0, 5).forEach(suggestion => {
            const suggestionDiv = document.createElement('div');
            suggestionDiv.className = 'suggestion-item';
            suggestionDiv.textContent = suggestion;
            suggestionDiv.addEventListener('click', () => {
                if (this.elements.messageInput) {
                    this.elements.messageInput.value = suggestion;
                }
                this.hideSuggestions();
                this.handleSendMessage();
            });
            this.elements.inputSuggestions.appendChild(suggestionDiv);
        });

        if (filteredSuggestions.length > 0) {
            this.elements.inputSuggestions.style.display = 'block';
        } else {
            this.elements.inputSuggestions.style.display = 'none';
        }
    }

    addWelcomeMessage() {
        const welcomeMessage = {
            role: 'assistant',
            content: '👋 Hi! I\'m your Amazon shopping assistant. I can help you find products, check orders, and answer questions about your account. What would you like to know?',
            timestamp: new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }),
            id: Date.now()
        };
        this.chatHistory.push(welcomeMessage);
        this.displayChat();
    }

    showNotification() {
        if (this.elements.chatNotification) {
            this.elements.chatNotification.style.display = 'flex';
            this.elements.chatbotToggle?.classList.add('pulse');
        }
    }

    hideNotification() {
        if (this.elements.chatNotification) {
            this.elements.chatNotification.style.display = 'none';
            this.elements.chatbotToggle?.classList.remove('pulse');
        }
    }

    updateConnectionStatus(isConnected) {
        this.isConnected = isConnected;
        if (isConnected && this.elements.statusDot && this.elements.statusText) {
            this.elements.statusDot.classList.remove('disconnected');
            this.elements.statusText.textContent = 'Connected';
        } else if (this.elements.statusDot && this.elements.statusText) {
            this.elements.statusDot.classList.add('disconnected');
            this.elements.statusText.textContent = 'Offline';
        }
    }

    checkConnection() {
        // Check connection every 30 seconds
        setInterval(() => {
            this.updateConnectionStatus(navigator.onLine);
        }, 30000);
    }

    showInitialPulse() {
        // Show pulse for first-time users
        if (!localStorage.getItem('chatbot_used') && this.elements.chatbotToggle) {
            setTimeout(() => {
                this.elements.chatbotToggle.classList.add('pulse');
            }, 2000);
        }
    }

    saveChatHistory() {
        try {
            const chatData = {
                chatHistory: this.chatHistory,
                previousChats: this.previousChats,
                timestamp: Date.now()
            };
            localStorage.setItem('amazon_chatbot_data', JSON.stringify(chatData));
            localStorage.setItem('chatbot_used', 'true');
        } catch (error) {
            console.warn('Could not save chat history:', error);
        }
    }

    loadChatHistory() {
        try {
            const savedData = localStorage.getItem('amazon_chatbot_data');
            if (savedData) {
                const chatData = JSON.parse(savedData);

                // Check if data is not too old (7 days)
                if (Date.now() - chatData.timestamp < 7 * 24 * 60 * 60 * 1000) {
                    this.previousChats = chatData.previousChats || [];
                    this.updatePreviousChatsDropdown();

                    // Don't restore current chat history automatically
                    // Let user start fresh each session
                }
            }
        } catch (error) {
            console.warn('Could not load chat history:', error);
        }
    }

    scrollToBottom() {
        if (this.elements.chatWindow) {
            setTimeout(() => {
                this.elements.chatWindow.scrollTop = this.elements.chatWindow.scrollHeight;
            }, 100);
        }
    }

    escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    getAntiForgeryToken() {
        // Get anti-forgery token if available
        const token = document.querySelector('input[name="__RequestVerificationToken"]');
        return token ? token.value : '';
    }

    handleOutsideClick(e) {
        if (this.elements.chatbotContainer && this.elements.chatbotToggle &&
            !this.elements.chatbotContainer.contains(e.target) &&
            !this.elements.chatbotToggle.contains(e.target) &&
            this.elements.chatbotContainer.classList.contains('show') &&
            !this.isMinimized) {
            // Optional: uncomment to close on outside click
            // this.closeChatbot();
        }
    }

    trackEvent(eventName, data = {}) {
        // Analytics tracking - implement based on your analytics service
        if (typeof gtag !== 'undefined') {
            gtag('event', eventName, {
                custom_parameter_1: data,
                event_category: 'chatbot'
            });
        }

        // Or use your custom analytics
        console.log('Event tracked:', eventName, data);
    }
}

// Public API
window.ChatbotUtils = {
    redirectToProduct: function (productId) {
        window.location.href = `/product/Details/${productId}`;
    },

    openChatbot: function () {
        const chatbot = window.chatbotInstance;
        if (chatbot) {
            chatbot.openChatbot();
        }
    },

    closeChatbot: function () {
        const chatbot = window.chatbotInstance;
        if (chatbot) {
            chatbot.closeChatbot();
        }
    },

    // Method to send a message programmatically
    sendMessage: function (message) {
        const chatbot = window.chatbotInstance;
        if (chatbot && chatbot.elements.messageInput) {
            chatbot.elements.messageInput.value = message;
            chatbot.handleSendMessage();
        }
    }
};

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function () {
    window.chatbotInstance = new EnhancedChatbot();
    console.log('Enhanced Amazon Chatbot initialized');
});