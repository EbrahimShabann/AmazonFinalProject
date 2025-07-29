using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Final_project.Models;
using Final_project.Repository;

public class AIChatbotController : Controller
{
    private readonly UnitOfWork uof;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration configuration;

    public AIChatbotController(UnitOfWork uof, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        this.uof = uof;
        _httpClientFactory = httpClientFactory;
        this.configuration = configuration;
    }

    public IActionResult Index()
    {
        return View();
    }


    [HttpPost]
    public async Task<JsonResult> Ask([FromBody] AIChatRequest request)
    {
        string reply = "";
        string message = request.Message?.ToLower();
        string GrokKey = configuration.GetSection("OpenAI")["GrokKey"];

        if (string.IsNullOrWhiteSpace(message))
            return Json(new { reply = "Please enter a question." });

        string dbInfo = "";
        string contextDescription = "";

        // 📦 Order-related
        if (message.Contains("order") || message.Contains("tracking") || message.Contains("shipment"))
        {
            var order = uof.OrderRepo.getAll().OrderByDescending(o => o.order_date).FirstOrDefault();
            if (order != null)
            {
                dbInfo = $"Order ID: {order.id}, Date: {order.order_date}, Status: {order.status}";
                contextDescription = "User asked about an order.";
            }
            else dbInfo = "No orders found.";
        }

        // 🛍️ Product-related
        else if (message.Contains("product") || message.Contains("price") || message.Contains("brand") || message.Contains("category"))
        {
            // Try to match product name from message (simple version)
            var allProducts = uof.ProductRepository.getAll().ToList();

            var matchedProduct = allProducts.FirstOrDefault(p =>
                !string.IsNullOrEmpty(p.name) &&
                message.Contains(p.name.ToLower())); // crude name match

            if (matchedProduct != null)
            {
                // Provide detailed info about this specific product
                dbInfo = $"Product: {matchedProduct.name}\n" +
                         $"- Brand: {matchedProduct.Brand}\n" +
                         $"- Price: {matchedProduct.price} EGP\n" +
                         $"- Description: {matchedProduct.description}\n" +
                         $"- Available Sizes: {matchedProduct.SelectedSizes}\n" +
                         $"- Available Colors: {matchedProduct.SelectedColors}";

                contextDescription = $"User asked about details of a specific product: {matchedProduct.name}";
            }
            else
            {
                // Fallback to list of popular products
                var products = allProducts
                    .OrderByDescending(p => p.price)
                    .Take(5)
                    .Select(p => new { p.name, p.Brand, p.price });

                if (products.Any())
                {
                    dbInfo = string.Join("\n", products.Select(p => $"- {p.name} ({p.Brand}): {p.price} EGP"));
                    contextDescription = "User asked about products or pricing.";
                }
                else
                {
                    dbInfo = "No products found.";
                }
            }
        }


        // 🛒 Cart-related
        else if (message.Contains("cart") || message.Contains("shopping cart"))
        {
            var cartItems = uof.CartItemRepository.getAll().Take(5).ToList();
            if (cartItems.Any())
            {
                dbInfo = string.Join("\n", cartItems.Select(c => $"- Product ID {c.product_id}, Quantity: {c.quantity}"));
                contextDescription = "User asked about their shopping cart.";
            }
            else dbInfo = "Cart is empty.";
        }

        // 🤍 Wishlist
        else if (message.Contains("wishlist"))
        {
            var items = uof.WishlistItemRepository.getAll().Take(5).ToList();
            if (items.Any())
            {
                dbInfo = string.Join("\n", items.Select(i => $"- Product ID {i.product_id}"));
                contextDescription = "User asked about their wishlist.";
            }
            else dbInfo = "Wishlist is empty.";
        }

        // 🎟️ Support Tickets
        //else if (message.Contains("ticket") || message.Contains("support"))
        //{
        //    var ticket = uof.SupportTicketRepo.getAll().OrderByDescending(t => t.created_at).FirstOrDefault();
        //    if (ticket != null)
        //    {
        //        dbInfo = $"Ticket ID: {ticket.id}, Status: {ticket.status}, Subject: {ticket.subject}";
        //        contextDescription = "User asked about a support ticket.";
        //    }
        //    else dbInfo = "No support tickets found.";
        //}

        // 🧾 Product Reviews
        else if (message.Contains("review") || message.Contains("rating"))
        {
            var reviews = uof.ProductRepository.getAllReviews().Take(5).ToList();
            if (reviews.Any())
            {
                dbInfo = string.Join("\n", reviews.Select(r => $"- Product {r.product_id}: {r.rating} stars - {r.comment}"));
                contextDescription = "User asked about reviews.";
            }
            else dbInfo = "No reviews found.";
        }

        // 💰 Discounts
        else if (message.Contains("discount") || message.Contains("sale"))
        {
            var discounts = uof.DiscountRepository.getAll().Take(3).ToList();
            if (discounts.Any())
            {
                dbInfo = string.Join("\n", discounts.Select(d => $"- {d.id}: {d.value}% off (expires {d.end_date})"));
                contextDescription = "User asked about discounts.";
            }
            else dbInfo = "No active discounts.";
        }

        // 🌐 Fallback/general
        else
        {
            // Let the assistant behave like a general Amazon-style assistant
            contextDescription = "User asked a general shopping or support-related question.";

            dbInfo = @"
                This is an e-commerce website similar to Amazon. 
                The store sells a variety of products across multiple categories such as electronics, fashion, beauty, and home essentials.
                Customers can:
                - Browse and search for products
                - Add items to cart and wishlist
                - Apply discount codes at checkout
                - Track orders and view order history
                - Submit and manage support tickets
                - Read and leave product reviews

                Use this background to answer the user's question appropriately even if no specific product or order data is included.";
        }

        // 🧠 Step 2: Build AI prompt
        string systemPrompt = "You are an AI assistant for an online shopping platform. Use the data provided if relevant to help the user.";

        var requestBody = new
        {
            model = "meta-llama/llama-4-scout-17b-16e-instruct",
            messages = new[]
            {
            new { role = "system", content = systemPrompt },
            new { role = "user", content = $"Question: {message}\n\nContext: {contextDescription}\n\nData:\n{dbInfo}" }
        }
        };

        // 🧠 Step 3: Call Groq API
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", GrokKey);

        var response = await client.PostAsync(
            "https://api.groq.com/openai/v1/chat/completions",
            new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
        );

        if (response.IsSuccessStatusCode)
        {
            using var stream = await response.Content.ReadAsStreamAsync();
            var data = await JsonSerializer.DeserializeAsync<OpenAIResponse>(stream);
            reply = data?.choices?[0]?.message?.content ?? "No reply received.";
        }
        else
        {
            reply = $"Error: {response.StatusCode}";
        }

        return Json(new { reply });
    }


    // Helper class for deserializing OpenAI response
    private class OpenAIResponse
    {
        public List<Choice> choices { get; set; }
        public class Choice
        {
            public Message message { get; set; }
        }
        public class Message
        {
            public string role { get; set; }
            public string content { get; set; }
        }
    }
}