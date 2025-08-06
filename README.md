🛒 AmazonFinalProject
A full-stack Amazon Clone e-commerce platform built with ASP.NET Core MVC, Entity Framework Core, and SQL Server, supporting features like product browsing, shopping cart, checkout, payments, customer service chat, two-factor authentication, and seller/admin dashboards.

📌 Features
🛍️ Product catalog with images, sizes, colors, and reviews

🔐 User authentication with email confirmation and Google OAuth

📦 Shopping cart and wishlist

💳 Stripe payment integration

💬 Real-time customer service chat (SignalR)

📧 Email + SMS notifications (via Gmail SMTP and Twilio)

📁 Admin & seller dashboards

🧠 AI assistant support (OpenAI integration)

🔒 Two-factor authentication using SMS

🧾 Order history, saved carts, and notification system

📈 Audit logs and activity tracking

🧱 Technologies Used
Layer =>	Technology
Backend =>	ASP.NET Core MVC, C#, EF Core
Frontend =>	Razor Views, HTML/CSS/JS/jQuery
Authentication =>	ASP.NET Identity + Google OAuth
Realtime =>	SignalR for chat
Payment =>	Stripe API
Database =>	SQL Server
Email =>	Gmail SMTP
SMS =>	Twilio
AI =>	OpenAI GPT integration

⚙️ Project Setup
1) Clone the repo

## git clone https://github.com/EbrahimShabann/AmazonFinalProject.git

2) Configure appsettings.json

Ensure your file includes keys for:
# ConnectionStrings:DefaultConnection
# Authentication:Google
# Stripe:SecretKey, PublishableKey
# EmailSettings (SMTP settings)
# TwilioSettings
# openAI:APIKey (optional)

🧪 Sample Credentials (Optional)
# Admin: Admin@amazon.com / Admin1234

🔐 Security Features
# Email confirmation
# 2FA with SMS
# Google login
# Token expiration handling
# Secure password policies

🤖 AI Assistant (optional)
# Supports chat-based product support or recommendations using OpenAI (ChatGPT-like interface).


