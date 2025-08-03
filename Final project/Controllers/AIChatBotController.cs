// Enhanced AIChatbotController with product integration - FIXED VERSION
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Final_project.Models;
using Final_project.Repository;
using Final_project.ViewModel.Customer;

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

        // Enhanced product search and recommendation logic
        if (IsProductRelatedQuery(message))
        {
            var productInfo = await GetProductInformation(message);
            dbInfo = productInfo.DatabaseInfo;
            contextDescription = productInfo.ContextDescription;
        }
        // 📦 Order-related queries
        else if (message.Contains("order") || message.Contains("tracking") || message.Contains("shipment"))
        {
            var orderInfo = GetOrderInformation();
            dbInfo = orderInfo.DatabaseInfo;
            contextDescription = orderInfo.ContextDescription;
        }
        // 🛒 Cart-related queries
        else if (message.Contains("cart") || message.Contains("shopping cart"))
        {
            var cartInfo = GetCartInformation();
            dbInfo = cartInfo.DatabaseInfo;
            contextDescription = cartInfo.ContextDescription;
        }
        // 🤍 Wishlist queries
        else if (message.Contains("wishlist") || message.Contains("wish list"))
        {
            var wishlistInfo = GetWishlistInformation();
            dbInfo = wishlistInfo.DatabaseInfo;
            contextDescription = wishlistInfo.ContextDescription;
        }
        // 🧾 Product Reviews
        else if (message.Contains("review") || message.Contains("rating"))
        {
            var reviewInfo = GetReviewInformation();
            dbInfo = reviewInfo.DatabaseInfo;
            contextDescription = reviewInfo.ContextDescription;
        }
        // 💰 Discounts
        else if (message.Contains("discount") || message.Contains("sale") || message.Contains("coupon"))
        {
            var discountInfo = GetDiscountInformation();
            dbInfo = discountInfo.DatabaseInfo;
            contextDescription = discountInfo.ContextDescription;
        }
        // 🌐 General e-commerce assistance
        else
        {
            contextDescription = "User asked a general shopping or support-related question.";
            dbInfo = GetGeneralStoreInformation();
        }

        // Build AI prompt
        string systemPrompt = @"You are an AI assistant for an Amazon-style e-commerce platform. 
                               You help customers find products, track orders, and provide shopping assistance.
                               When recommending products, be enthusiastic and helpful.
                               Keep responses concise but friendly.
                               If you mention specific products, suggest that the user can see product recommendations.";

        var requestBody = new
        {
            model = request.Model,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = $"Question: {message}\n\nContext: {contextDescription}\n\nData:\n{dbInfo}" }
            },
            temperature = request.Temperature
        };

        // Call Groq API
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GrokKey);

        try
        {
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
                reply = $"I'm experiencing some technical difficulties. Please try again in a moment.";
            }
        }
        catch (Exception ex)
        {
            reply = "I'm currently unavailable. Please try again later.";
            // Log the exception here if you have logging configured
        }

        return Json(new { reply });
    }

    // Enhanced product search with better matching
    private async Task<(string DatabaseInfo, string ContextDescription)> GetProductInformation(string message)
    {
        try
        {
            var allProducts = uof.ProductRepository.getProductsWithImagesAndRating();
            var matchedProducts = new List<ProductsVM>();

            // Try to find products by name, brand, or category
            var searchTerms = ExtractSearchTerms(message);

            foreach (var term in searchTerms)
            {
                var products = allProducts.Where(p =>
                    (!string.IsNullOrEmpty(p.name) && p.name.ToLower().Contains(term)) ||
                    (!string.IsNullOrEmpty(p.Brand) && p.Brand.ToLower().Contains(term)) ||
                    (!string.IsNullOrEmpty(p.description) && p.description.ToLower().Contains(term))
                ).Take(5).ToList();

                matchedProducts.AddRange(products);
            }

            if (matchedProducts.Any())
            {
                var distinctProducts = matchedProducts.GroupBy(p => p.id).Select(g => g.First()).Take(3);
                var productInfo = string.Join("\n", distinctProducts.Select(p =>
                    $"- {p.name} by {p.Brand}: ${p.discount_price ?? p.price} (Rating: {p.rating}/5, {p.ReviewsCount} reviews)"));

                return (productInfo, $"User asked about products. Found {distinctProducts.Count()} matching products.");
            }
            else
            {
                // Fallback to popular products
                var popularProducts = allProducts
                    .OrderByDescending(p => p.ReviewsCount)
                    .ThenByDescending(p => p.rating)
                    .Take(3)
                    .Select(p => $"- {p.name} by {p.Brand}: ${p.discount_price ?? p.price}")
                    .ToList();

                return (string.Join("\n", popularProducts), "User asked about products. Showing popular items.");
            }
        }
        catch (Exception)
        {
            return ("No products available at the moment.", "Error retrieving product information.");
        }
    }

    private List<string> ExtractSearchTerms(string message)
    {
        var terms = new List<string>();
        var words = message.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // Common e-commerce terms to ignore
        var ignoreWords = new HashSet<string> { "i", "want", "need", "looking", "for", "show", "me", "find", "search", "buy", "purchase", "get", "any", "some", "good", "best", "cheap", "expensive" };

        foreach (var word in words)
        {
            var cleanWord = word.ToLower().Trim(',', '.', '?', '!');
            if (cleanWord.Length > 2 && !ignoreWords.Contains(cleanWord))
            {
                terms.Add(cleanWord);
            }
        }

        return terms;
    }

    private bool IsProductRelatedQuery(string message)
    {
        var productKeywords = new[] { "product", "item", "buy", "purchase", "price", "brand", "category", "show", "find", "search", "recommend", "suggest", "looking for", "want to buy" };
        return productKeywords.Any(keyword => message.Contains(keyword));
    }

    private (string DatabaseInfo, string ContextDescription) GetOrderInformation()
    {
        try
        {
            if (User.Identity.IsAuthenticated)
            {
                // FIXED: Using the correct property name 'buyer_id' from the order model
                var orders = uof.OrderRepo.getAll()
                    .Where(o => o.buyer_id == User.Identity.Name)
                    .OrderByDescending(o => o.order_date)
                    .Take(3)
                    .ToList();

                if (orders.Any())
                {
                    var orderInfo = string.Join("\n", orders.Select(o =>
                        $"Order #{o.id}: {o.status} - ${o.total_amount} (Date: {o.order_date?.ToString("MM/dd/yyyy") ?? "N/A"})"));
                    return (orderInfo, "User asked about their orders.");
                }
            }
            return ("No recent orders found.", "User asked about orders but none found.");
        }
        catch (Exception ex)
        {
            return ("Unable to retrieve order information.", "Error accessing order data.");
        }
    }

    private (string DatabaseInfo, string ContextDescription) GetCartInformation()
    {
        try
        {
            if (User.Identity.IsAuthenticated)
            {
                // FIXED: First get the user's cart, then get cart items with proper Include
                var userCart = uof.ShoppingCartRepository.GetShoppingCartByUserId(User.Identity.Name);

                if (userCart != null)
                {
                    // Get cart items using LINQ query with proper includes
                    var cartItems = uof.CartItemRepository.getAll()
                        .Where(c => c.cart_id == userCart.id)
                        .ToList();

                    // Load products separately if needed (since Include might not be working in repository)
                    var cartItemsWithProducts = new List<dynamic>();
                    foreach (var item in cartItems.Take(5))
                    {
                        var product = uof.ProductRepository.getById(item.product_id);
                        cartItemsWithProducts.Add(new
                        {
                            ProductName = product?.name ?? "Product",
                            ProductId = item.product_id,
                            Quantity = item.quantity,
                            Size = item.size ?? "N/A",
                            Color = item.color ?? "N/A"
                        });
                    }

                    if (cartItemsWithProducts.Any())
                    {
                        var cartInfo = string.Join("\n", cartItemsWithProducts.Select(c =>
                            $"- {c.ProductName} (ID: {c.ProductId}), Quantity: {c.Quantity}, Size: {c.Size}, Color: {c.Color}"));
                        return (cartInfo, "User asked about their shopping cart.");
                    }
                }
            }
            return ("Your cart is currently empty.", "User asked about cart - empty.");
        }
        catch (Exception ex)
        {
            return ("Unable to retrieve cart information.", "Error accessing cart data.");
        }
    }

    private (string DatabaseInfo, string ContextDescription) GetWishlistInformation()
    {
        try
        {
            if (User.Identity.IsAuthenticated)
            {
                // FIXED: Assuming wishlist items don't have direct user_id
                // You'll need to adjust this based on your actual wishlist model structure
                // Option 1: If wishlist has user relationship through wishlist table
                var wishlistItems = uof.WishlistItemRepository.getAll()
                    .Where(w => w.Wishlist.user_id == User.Identity.Name) // Assuming navigation property
                    .Take(5)
                    .ToList();

                // Option 2: Alternative approach if wishlist structure is different
                // var userWishlist = uof.WishlistRepository.GetWishlistByUserId(User.Identity.Name);
                // var wishlistItems = userWishlist?.WishlistItems?.Take(5).ToList() ?? new List<wishlist_item>();

                if (wishlistItems.Any())
                {
                    var wishlistItemsWithProducts = new List<dynamic>();
                    foreach (var item in wishlistItems)
                    {
                        var product = uof.ProductRepository.getById(item.product_id);
                        wishlistItemsWithProducts.Add(new
                        {
                            ProductName = product?.name ?? "Product",
                            ProductId = item.product_id
                        });
                    }

                    var wishlistInfo = string.Join("\n", wishlistItemsWithProducts.Select(w =>
                        $"- {w.ProductName} (ID: {w.ProductId})"));
                    return (wishlistInfo, "User asked about their wishlist.");
                }
            }
            return ("Your wishlist is currently empty.", "User asked about wishlist - empty.");
        }
        catch (Exception ex)
        {
            return ("Unable to retrieve wishlist information.", "Error accessing wishlist data.");
        }
    }

    private (string DatabaseInfo, String ContextDescription) GetReviewInformation()
    {
        try
        {
            var reviews = uof.ProductRepository.getAllReviews()
                .OrderByDescending(r => r.created_at)
                .Take(5)
                .Select(r => $"Product {r.product_id}: {r.rating}/5 stars - \"{r.comment}\"")
                .ToList();

            var reviewInfo = reviews.Any() ? string.Join("\n", reviews) : "No reviews available.";
            return (reviewInfo, "User asked about product reviews.");
        }
        catch
        {
            return ("Unable to retrieve review information.", "Error accessing review data.");
        }
    }

    private (string DatabaseInfo, string ContextDescription) GetDiscountInformation()
    {
        try
        {
            var discounts = uof.DiscountRepository.getAll()
                .Where(d => d.end_date > DateTime.Now)
                .Take(3)
                .Select(d => $"- Code: {d.id} - {d.value}% off (expires {d.end_date:MM/dd/yyyy})")
                .ToList();

            var discountInfo = discounts.Any() ? string.Join("\n", discounts) : "No active discounts available.";
            return (discountInfo, "User asked about discounts and sales.");
        }
        catch
        {
            return ("Unable to retrieve discount information.", "Error accessing discount data.");
        }
    }

    private string GetGeneralStoreInformation()
    {
        return @"This is an Amazon-style e-commerce platform offering:
                - Wide variety of products across multiple categories
                - Electronics, Fashion, Beauty, Home & Garden, and more
                - Competitive prices with regular discounts
                - Fast shipping and reliable customer service
                - Secure shopping with multiple payment options
                - Product reviews and ratings from verified customers
                - Wishlist and cart functionality for easy shopping";
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
