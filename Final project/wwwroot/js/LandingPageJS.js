// ========================================================================================
// RIPPLE EFFECT SYSTEM
// ========================================================================================
// Adds Material Design ripple effects to buttons (excluding back-to-top)

function setupRippleEffect() {
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

    // Apply ripple effect to all buttons except back-to-top
    document
        .querySelectorAll("button:not(#backToTop)")
        .forEach((button) => {
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
}

// ========================================================================================
// BOX ANIMATION SYSTEM
// ========================================================================================
// Handles the main box opening/closing animation with product flying effects

function setupBoxAnimation() {
    const mainBox = document.getElementById("mainBox");

    // Initial animation sequence
    function runBoxSequence() {
        // After products fly in, close the box and start delivery
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
    }

    // Reset and repeat animation
    function resetBoxAnimation() {
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

        // Run the sequence again
        runBoxSequence();
    }

    // Start initial sequence
    runBoxSequence();

    // Repeat animation every 10 seconds
    setInterval(resetBoxAnimation, 10000);
}


// ========================================================================================
// MAIN INITIALIZATION
// ========================================================================================
// Initialize all functionality when the DOM is fully loaded

document.addEventListener("DOMContentLoaded", () => {
    setupRippleEffect();
    setupBoxAnimation();
});