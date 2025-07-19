              // Sidebar functionality
    const hamburgerMenu = document.getElementById("hamburgerMenu");
    const sidebar = document.getElementById("sidebar");
    const sidebarOverlay = document.getElementById("sidebarOverlay");
    const sidebarClose = document.getElementById("sidebarClose");
    function openSidebar() {
        sidebar.classList.add("show");
    sidebarOverlay.classList.add("show");
    document.body.style.overflow = "hidden";
              }

    function closeSidebar() {
        sidebar.classList.remove("show");
    sidebarOverlay.classList.remove("show");
    document.body.style.overflow = "auto";
              }

    hamburgerMenu.addEventListener("click", openSidebar);
    sidebarClose.addEventListener("click", closeSidebar);
    sidebarOverlay.addEventListener("click", closeSidebar);

    // Close sidebar on escape key
    document.addEventListener("keydown", function (e) {
                if (e.key === "Escape" && sidebar.classList.contains("show")) {
        closeSidebar();
                }
              });

    // Prevent sidebar from closing when clicking inside it
    sidebar.addEventListener("click", function (e) {
        e.stopPropagation();
              });

    // Scroll products function
    function scrollProducts(containerId, direction) {
                const container = document.getElementById(containerId);
    const scrollAmount = 300;
    container.scrollBy({
        left: direction * scrollAmount,
    behavior: "smooth",
                });
              }
    // Add to cart animation
    function addToCart(button) {
        button.innerHTML = '<i class="fas fa-check"></i> Added!';
    button.style.background = "#28a745";
    button.style.borderColor = "#28a745";

                setTimeout(() => {
        button.innerHTML = "Add to Cart";
    button.style.background = "#ff9900";
    button.style.borderColor = "#ff9900";
                }, 2000);
              }

    // Search functionality
    function setupSearch() {
                const searchInput = document.querySelector(".search-input");
    const searchBtn = document.querySelector(".search-btn");

    searchBtn.addEventListener("click", performSearch);
                searchInput.addEventListener("keypress", (e) => {
                  if (e.key === "Enter") {
        performSearch();
                  }
                });
              }

    function performSearch() {
                const searchTerm = document.querySelector(".search-input").value;
    if (searchTerm.trim()) {
                  const searchBtn = document.querySelector(".search-btn");
    searchBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i>';

                  setTimeout(() => {
        searchBtn.innerHTML = '<i class="fas fa-search"></i>';
    alert(`Searching for: ${searchTerm}`);
                  }, 1000);
                }
              }

    // Add click handlers to all add-to-cart buttons
    function setupCartButtons() {
        document.querySelectorAll(".add-to-cart").forEach((button) => {
            button.addEventListener("click", (e) => {
                e.preventDefault();
                addToCart(button);
            });
        });
              }

    // Function to create smoke particles
    function createSmoke() {
                const car = document.getElementById("deliveryCar");
    const smoke = document.createElement("div");
    smoke.className = "car-smoke";

    const carRect = car.getBoundingClientRect();
    const containerRect = car.parentElement.getBoundingClientRect();

    smoke.style.left = carRect.left - containerRect.left - 20 + "px";
    car.parentElement.appendChild(smoke);

                setTimeout(() => {
        smoke.remove();
                }, 800);
              }

    // New function for delivery animation
    function startDeliveryAnimation() {
                const car = document.getElementById("deliveryCar");

    // Start car animation
    car.classList.add("driving");

                // Add smoke effect
                const smokeInterval = setInterval(() => {
                  if (car.classList.contains("driving")) {
        createSmoke();
                  } else {
        clearInterval(smokeInterval);
                  }
                }, 200);

                // Clean up after animation
                setTimeout(() => {
        car.classList.remove("driving");
                }, 4000);
              }

              // Initialize everything when page loads
              document.addEventListener("DOMContentLoaded", () => {
        setupSearch();
    setupCartButtons();

    const mainBox = document.getElementById("mainBox");

                // After all products have flown in, close the box and start delivery
                setTimeout(() => {
        mainBox.classList.add("closing");

                  // Change to closed box icon
                  setTimeout(() => {
        mainBox.classList.remove("fa-box-open");
    mainBox.classList.add("bi", "bi-box-seam-fill");
    mainBox.classList.add("closed");

                    // Start delivery animation after box is closed
                    setTimeout(() => {
        startDeliveryAnimation();
                    }, 500);
                  }, 500);
                }, 5000);

                // Reset animation every 10 seconds
                setInterval(() => {
        mainBox.classList.remove(
            "closing",
            "closed",
            "bi",
            "bi-box-seam-fill"
        );
    mainBox.classList.add("fa-box-open");

    // Reset car animation
    const car = document.getElementById("deliveryCar");
    car.classList.remove("driving");

    // Reset product animations
    const products = document.querySelectorAll(".product-item");
                  products.forEach((product) => {
        product.style.animation = "none";
                    setTimeout(() => {
        product.style.animation = `flyIntoBox 1.5s ease-in-out var(--delay) forwards`;
                    }, 10);
                  });

                  // Close box and start delivery again after products fly in
                  setTimeout(() => {
        mainBox.classList.add("closing");
                    setTimeout(() => {
        mainBox.classList.remove("fa-box-open");
    mainBox.classList.add("bi", "bi-box-seam-fill");
    mainBox.classList.add("closed");

                      // Start delivery animation after box is closed
                      setTimeout(() => {
        startDeliveryAnimation();
                      }, 500);
                    }, 500);
                  }, 5000);
                }, 10000);
              });

              // Add ripple effect to buttons
              document.querySelectorAll("button").forEach((button) => {
        button.addEventListener("click", function (e) {
            const ripple = document.createElement("span");
            const rect = this.getBoundingClientRect();
            const size = Math.max(rect.width, rect.height);
            const x = e.clientX - rect.left - size / 2;
            const y = e.clientY - rect.top - size / 2;

            ripple.style.cssText = `
              position: absolute;
              width: ${size}px;
              height: ${size}px;
              left: ${x}px;
              top: ${y}px;
              background: rgba(255,255,255,0.3);
              border-radius: 50%;
              transform: scale(0);
              animation: ripple 0.6s linear;
              pointer-events: none;
            `;

            this.style.position = "relative";
            this.style.overflow = "hidden";
            this.appendChild(ripple);

            setTimeout(() => {
                ripple.remove();
            }, 600);
        });
              });

    // Add CSS for ripple animation
    const style = document.createElement("style");
    style.textContent = `
    @keyframes ripple {
        to {
        transform: scale(4);
    opacity: 0;
            }
          }
    `;
    document.head.appendChild(style);

    // Back to Top Button
    const backToTopButton = document.getElementById("backToTop");

    // Lock the button's position (prevents ripple effect from breaking it)
    backToTopButton.style.position = "fixed";
    backToTopButton.style.overflow = "visible";

    window.addEventListener("scroll", function () {
        backToTopButton.classList.toggle("visible", window.scrollY > 300);
              });

    backToTopButton.addEventListener("click", function (e) {
        e.preventDefault();
    window.scrollTo({
        top: 0,
    behavior: "smooth",
                });
              });

              // Modify ripple effect to exclude back-to-top button
              document.querySelectorAll("button:not(#backToTop)").forEach((button) => {
        button.addEventListener("click", function (e) {
            // Your existing ripple effect code here
        });
              });
