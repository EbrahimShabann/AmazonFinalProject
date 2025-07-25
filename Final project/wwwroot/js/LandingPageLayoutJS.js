    // ========================================================================================
    // SIDEBAR FUNCTIONALITY
    // ========================================================================================
    // Handles opening/closing of mobile sidebar menu with overlay and keyboard controls

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

    // Event listeners for sidebar controls
    if (hamburgerMenu) hamburgerMenu.addEventListener("click", openSidebar);
    if (sidebarClose) sidebarClose.addEventListener("click", closeSidebar);
    if (sidebarOverlay) sidebarOverlay.addEventListener("click", closeSidebar);

    // Close sidebar on escape key
    document.addEventListener("keydown", function (e) {
        if (e.key === "Escape" && sidebar.classList.contains("show")) {
        closeSidebar();
        }
      });

    // Prevent sidebar from closing when clicking inside it
    if (sidebar) {
        sidebar.addEventListener("click", function (e) {
            e.stopPropagation();
        });
    }

    // ========================================================================================
    // PRODUCT SCROLLING FUNCTIONALITY
    // ========================================================================================
    // Enables horizontal scrolling of product containers with smooth animation

    function scrollProducts(containerId, direction) {
        const container = document.getElementById(containerId);
    const scrollAmount = 300;
    container.scrollBy({
        left: direction * scrollAmount,
    behavior: "smooth",
        });
      }

    // ========================================================================================
    // ADD TO CART FUNCTIONALITY
    // ========================================================================================
    // Handles add to cart button animations and feedback

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

    // Setup event listeners for all add-to-cart buttons
    function setupCartButtons() {
        document.querySelectorAll(".add-to-cart").forEach((button) => {
            button.addEventListener("click", (e) => {
                e.preventDefault();
                addToCart(button);
            });
        });
      }

    // ========================================================================================
    // SEARCH FUNCTIONALITY
    // ========================================================================================
    // Handles search input and button interactions with loading states

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

    // ========================================================================================
    // DELIVERY ANIMATION SYSTEM
    // ========================================================================================
    // Creates animated delivery car with smoke effects

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

   

    // ========================================================================================
    // NAVIGATION AND PAGE DISPLAY
    // ========================================================================================
    // Handles category page navigation and smooth scrolling

    //function setupNavigation() {
    //    document
    //        .querySelectorAll(".nav-link, .sidebar-section a")
    //        .forEach((link) => {
    //            link.addEventListener("click", function (e) {
    //                e.preventDefault();
    //                document.getElementById("categoryPage").style.display = "block";
    //                document
    //                    .getElementById("categoryPage")
    //                    .scrollIntoView({ behavior: "smooth" });
    //            });
    //        });
    //  }

    // ========================================================================================
    // SORTING AND PAGINATION
    // ========================================================================================
    // Handles product sorting and pagination controls

    //function setupSortingAndPagination() {
    //    // Sort functionality
    //    document
    //        .getElementById("sortSelect")
    //        .addEventListener("change", function () {
    //            console.log("Sorting by:", this.value);
    //        });

    //    // Pagination functionality
    //    document.querySelectorAll(".page-link").forEach((link) => {
    //    link.addEventListener("click", function (e) {
    //        if (!this.getAttribute("href") || this.closest(".disabled")) {
    //            e.preventDefault();
    //            return;
    //        }

    //        e.preventDefault();
    //        const pageNum = this.textContent;
    //        console.log("Going to page:", pageNum);

    //        // Update active page
    //        document.querySelectorAll(".page-item").forEach((item) => {
    //            item.classList.remove("active");
    //        });

    //        if (pageNum === "Next") {
    //            const currentActive = document.querySelector(".page-item.active");
    //            const nextPage = currentActive.nextElementSibling;
    //            if (nextPage && !nextPage.classList.contains("disabled")) {
    //                nextPage.classList.add("active");
    //            }
    //        } else if (pageNum === "Previous") {
    //            const currentActive = document.querySelector(".page-item.active");
    //            const prevPage = currentActive.previousElementSibling;
    //            if (prevPage && !prevPage.classList.contains("disabled")) {
    //                prevPage.classList.add("active");
    //            }
    //        } else {
    //            this.closest(".page-item").classList.add("active");
    //        }
    //    });
    //    });
    //  }

    // ========================================================================================
    // FILTER LIST INTERACTIONS
    // ========================================================================================
    // Handles active states for filter list items

    function setupFilterLists() {
        const filterLists = document.querySelectorAll(".filter-list");

        filterLists.forEach((filterList) => {
          const listItems = filterList.querySelectorAll("li");

          listItems.forEach((item) => {
        item.addEventListener("click", function (e) {
            if (e.target.tagName === "A") {
                e.preventDefault();
            }

            // Remove active class from all items in this filter list
            listItems.forEach((li) => {
                const link = li.querySelector("a");
                if (link) link.classList.remove("active");
            });

            // Add active class to clicked item
            const clickedLink = this.querySelector("a") || this;
            clickedLink.classList.add("active");
        });
          });
        });
      }

    // ========================================================================================
    // BACK TO TOP BUTTON
    // ========================================================================================
    // Handles scroll-to-top functionality with visibility toggle

    function setupBackToTop() {
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
      }

   

   

      // ========================================================================================
      // MAIN INITIALIZATION
      // ========================================================================================
      // Initialize all functionality when the DOM is fully loaded

      document.addEventListener("DOMContentLoaded", () => {
        // Initialize all components
        setupSearch();
    setupCartButtons();
    //setupFilterSections();
    //setupDepartmentLinks();
    //setupNavigation();
    //setupSortingAndPagination();
    setupFilterLists();
    setupBackToTop();
    //setupRippleEffect();
    //setupBoxAnimation();
      });





