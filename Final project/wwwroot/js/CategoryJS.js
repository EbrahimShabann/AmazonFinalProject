// ========================================================================================
// NAVIGATION AND PAGE DISPLAY
// ========================================================================================
// Handles category page navigation and smooth scrolling

function setupNavigation() {
    document
        .querySelectorAll(".nav-link, .sidebar-section a")
        .forEach((link) => {
            link.addEventListener("click", function (e) {
                e.preventDefault();
                document.getElementById("categoryPage").style.display = "block";
                document
                    .getElementById("categoryPage")
                    .scrollIntoView({ behavior: "smooth" });
            });
        });
}

// ========================================================================================
// SORTING AND PAGINATION
// ========================================================================================
// Handles product sorting and pagination controls

function setupSortingAndPagination() {
    // Sort functionality
    document
        .getElementById("sortSelect")
        .addEventListener("change", function () {
            console.log("Sorting by:", this.value);
        });

    // Pagination functionality
    document.querySelectorAll(".page-link").forEach((link) => {
    link.addEventListener("click", function (e) {
        if (!this.getAttribute("href") || this.closest(".disabled")) {
            e.preventDefault();
            return;
        }

        e.preventDefault();
        const pageNum = this.textContent;
        console.log("Going to page:", pageNum);

        // Update active page
        document.querySelectorAll(".page-item").forEach((item) => {
            item.classList.remove("active");
        });

        if (pageNum === "Next") {
            const currentActive = document.querySelector(".page-item.active");
            const nextPage = currentActive.nextElementSibling;
            if (nextPage && !nextPage.classList.contains("disabled")) {
                nextPage.classList.add("active");
            }
        } else if (pageNum === "Previous") {
            const currentActive = document.querySelector(".page-item.active");
            const prevPage = currentActive.previousElementSibling;
            if (prevPage && !prevPage.classList.contains("disabled")) {
                prevPage.classList.add("active");
            }
        } else {
            this.closest(".page-item").classList.add("active");
        }
    });
    });
}

// ========================================================================================
// CATEGORY AND FILTER FUNCTIONALITY
// ========================================================================================
// Handles category navigation, filter sections, and department links

function updateCategoryHeader(categoryName) {
    const categoryHeader = document.querySelector(".category-header h4");
    if (categoryHeader) {
        categoryHeader.textContent = categoryName;
    }
}

// Setup filter section collapsible functionality
function setupFilterSections() {
    const filterHeaders = document.querySelectorAll(".filter-section h5");

    filterHeaders.forEach((header) => {
        header.addEventListener("click", function () {
            const filterSection = this.parentElement;
            filterSection.classList.toggle("collapsed");
        });
    });

    // Initially collapse all sections except Department
    const allFilterSections = document.querySelectorAll(".filter-section");
    allFilterSections.forEach((section, index) => {
        if (index > 0) {
            section.classList.add("collapsed");
        }
    });
}

// Setup department links functionality
function setupDepartmentLinks() {
    const departmentLinks = document.querySelectorAll(
        ".filter-section .filter-list .filter-item"
    );

    departmentLinks.forEach((link) => {
        link.addEventListener("click", function (e) {
            e.preventDefault();

            const categoryName = this.textContent.trim();
            updateCategoryHeader(categoryName);

            // Update active states
            departmentLinks.forEach((l) => l.classList.remove("active"));
            this.classList.add("active");
        });
    });
}
// ========================================================================================
// MAIN INITIALIZATION
// ========================================================================================
// Initialize all functionality when the DOM is fully loaded

document.addEventListener("DOMContentLoaded", () => {
    setupFilterSections();
    setupDepartmentLinks();
    setupNavigation();
    setupSortingAndPagination();
});