        // Advanced Particle System
    function createAdvancedParticles() {
          const container = document.getElementById("particleSystem");
    const particleCount = 80;

    for (let i = 0; i < particleCount; i++) {
            const particle = document.createElement("div");
    particle.className = "particle";

    // Assign random size class
    const sizeClasses = ["large", "medium", "small"];
    const randomSize =
    sizeClasses[Math.floor(Math.random() * sizeClasses.length)];
    particle.classList.add(randomSize);

    // Random positioning and timing
    particle.style.left = Math.random() * 100 + "%";
    particle.style.animationDelay = Math.random() * 20 + "s";
    particle.style.animationDuration = Math.random() * 15 + 15 + "s";

    container.appendChild(particle);
          }
        }

    // Create Geometric Background
    function createGeometricShapes() {
          const container = document.getElementById("geometryContainer");
    const shapes = ["polygon", "circle", "rect", "ellipse"];

    for (let i = 0; i < 15; i++) {
            const shape = document.createElement("div");
    shape.className = "geo-shape";

    // Create SVG
    const svg = document.createElementNS(
    "http://www.w3.org/2000/svg",
    "svg"
    );
    svg.setAttribute("width", Math.random() * 100 + 50);
    svg.setAttribute("height", Math.random() * 100 + 50);
    svg.setAttribute("viewBox", "0 0 100 100");

    const element = document.createElementNS(
    "http://www.w3.org/2000/svg",
    shapes[Math.floor(Math.random() * shapes.length)]
    );

    if (element.tagName === "polygon") {
        element.setAttribute("points", "50,10 90,90 10,90");
            } else if (element.tagName === "circle") {
        element.setAttribute("cx", "50");
    element.setAttribute("cy", "50");
    element.setAttribute("r", "40");
            } else if (element.tagName === "rect") {
        element.setAttribute("x", "20");
    element.setAttribute("y", "20");
    element.setAttribute("width", "60");
    element.setAttribute("height", "60");
    element.setAttribute("rx", "10");
            }

    element.setAttribute(
    "fill",
    `hsl(${Math.random() * 60 + 20}, 70%, 60%)`
    );
    element.setAttribute("opacity", "0.3");

    svg.appendChild(element);
    shape.appendChild(svg);

    // Random positioning
    shape.style.left = Math.random() * 100 + "%";
    shape.style.top = Math.random() * 100 + "%";
    shape.style.animationDelay = Math.random() * 12 + "s";
    shape.style.animationDuration = Math.random() * 8 + 8 + "s";

    container.appendChild(shape);
          }
        }

    // Enhanced Mouse Interaction
    function setupMouseInteraction() {
        let mouseX = 0,
    mouseY = 0;

    document.addEventListener("mousemove", function (e) {
        mouseX = (e.clientX / window.innerWidth - 0.5) * 2;
    mouseY = (e.clientY / window.innerHeight - 0.5) * 2;

    // Affect container with parallax
    const container = document.querySelector(".container");
    container.style.transform = `translateX(${
        mouseX * 20
    }px) translateY(${mouseY * 20}px) rotateY(${mouseX * 5}deg) rotateX(${
        -mouseY * 5
    }deg)`;

    // Affect orbits
    const orbits = document.querySelectorAll(".orbit");
            orbits.forEach((orbit, index) => {
              const factor = (index + 1) * 0.1;
    orbit.style.transform = `translate(-50%, -50%) rotate(${
        mouseX * factor * 10
    }deg)`;
            });
          });
        }

    // Energy Wave Effect
    function createEnergyWave() {
          const wave = document.getElementById("energyWave");

          setInterval(() => {
        wave.style.left = Math.random() * 100 + "%";
    wave.style.top = Math.random() * 100 + "%";
    wave.style.animation = "none";
            setTimeout(() => {
        wave.style.animation = "energyPulse 3s ease-out";
            }, 10);
          }, 4000);
        }

    // Enhanced Home Button
    function goHome() {
          const container = document.querySelector(".container");
    const particles = document.getElementById("particleSystem");

    // Create exit animation
    container.style.transform = "scale(0.9) rotateY(180deg)";
    container.style.opacity = "0";
    container.style.filter = "blur(10px)";
    container.style.transition =
    "all 0.8s cubic-bezier(0.25, 0.46, 0.45, 0.94)";

    particles.style.opacity = "0";
    particles.style.transition = "opacity 0.5s ease";

          setTimeout(() => {
            // Create loading overlay
            const loader = document.createElement("div");
    loader.style.cssText = `
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: radial-gradient(circle, var(--amazon-orange), var(--amazon-dark));
    z-index: 9999;
    display: flex;
    align-items: center;
    justify-content: center;
    opacity: 0;
    transition: opacity 0.5s ease;
    `;

    const spinner = document.createElement("div");
    spinner.style.cssText = `
    width: 60px;
    height: 60px;
    border: 4px solid rgba(255,255,255,0.3);
    border-top: 4px solid white;
    border-radius: 50%;
    animation: spin 1s linear infinite;
    `;

    const style = document.createElement("style");
    style.textContent =
    "@keyframes spin {0 % { transform: rotate(0deg); } 100% {transform: rotate(360deg); } }";
    document.head.appendChild(style);

    loader.appendChild(spinner);
    document.body.appendChild(loader);

            setTimeout(() => {
        loader.style.opacity = "1";
            }, 10);

            setTimeout(() => {
        window.location.href = "/";
            }, 1500);
          }, 800);
        }

    // Keyboard Shortcuts
    function setupKeyboardShortcuts() {
        document.addEventListener("keydown", function (e) {
            if (e.key === "Enter" || e.key === " ") {
                e.preventDefault();
                goHome();
            } else if (e.key === "Escape") {
                // Add easter egg
                document.body.style.filter = "hue-rotate(180deg)";
                setTimeout(() => {
                    document.body.style.filter = "none";
                }, 2000);
            }
        });
        }

    // Performance monitoring
    function optimizePerformance() {
          if (window.innerWidth < 768) {
        document.getElementById("particleSystem").remove();
    document.getElementById("geometryContainer").remove();
          }
        }

    // Initialize everything
    document.addEventListener("DOMContentLoaded", function () {
        optimizePerformance();
    createAdvancedParticles();
    createGeometricShapes();
    setupMouseInteraction();
    createEnergyWave();
    setupKeyboardShortcuts();

    // Add focus management for accessibility
    document.querySelector(".home-button").focus();
        });

    // Add touch gestures for mobile
    if ("ontouchstart" in window) {
        let startY = 0;
    document.addEventListener("touchstart", function (e) {
        startY = e.touches[0].clientY;
          });

    document.addEventListener("touchend", function (e) {
            const endY = e.changedTouches[0].clientY;
            if (startY - endY > 100) {
        // Swipe up
        goHome();
            }
          });
        }
