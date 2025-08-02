// Create a function to call OpenAI API using fetch
// Use gpt-3.5-turbo or another supported OpenAI model and display responses in the chat window
// Append both user and assistant messages to the chat history

const chatHistory = [];
const previousChats = []; // Store previous chats


const OPENAI_API_KEY = "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";
const OPENAI_CHAT_API_URL = "https://api.openai.com/v1/chat/completions";
const OPENAI_EMBEDDING_API_URL = "https://api.openai.com/v1/embeddings";
let selectedModel = "o1-mini"; // Default OpenAI chat model

async function callOpenAI(userMessage) {
    chatHistory.push({ role: "user", content: userMessage });
    const timestamp = new Date().toLocaleTimeString();

    const response = await fetch("/AIChatbot/Ask", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${OPENAI_API_KEY}`
        },
        body: JSON.stringify({
            model: selectedModel,
            message: userMessage,
            temperature: 0.7
        })
    });

    const data = await response.json();
    console.log(data);
    //if (!response.ok || !data.choices || !data.choices[0]?.message?.content) {
    //    console.error("OpenAI API Error:", data);
    //    alert(`OpenAI Error: ${data.error?.message || response.statusText}`);
    //    return;
    //}

    const assistantMessage = data.reply;
    chatHistory.push({ role: "assistant", content: `${assistantMessage} (${timestamp})` });
    displayChat();

}

document.addEventListener("DOMContentLoaded", async () => {
    const sendBtn = document.getElementById("send-btn");
    const input = document.getElementById("message-input");
    const newChatBtn = document.getElementById("new-chat-btn");
    const modelSelect = document.getElementById("model-select");


    if (modelSelect) {
        modelSelect.addEventListener("change", (e) => {
            selectedModel = modelSelect.value;
        });
    }

    if (sendBtn && input) {
        sendBtn.addEventListener("click", async () => {
            const message = input.value.trim();
            console.log(message);


            if (message) {
                await callOpenAI(message);  // still sends regular message
                input.value = "";
            }
        });

        document.getElementById('message-input').addEventListener('keypress', function (e) {
            if (e.key === 'Enter') {
                sendBtn.click();
            }
        });

    }

    if (newChatBtn) {
        newChatBtn.addEventListener("click", () => {
            if (chatHistory.length > 0) {
                previousChats.push([...chatHistory]); // Save current chat
                displayPreviousChats();               // Refresh dropdown
            }
            chatHistory.length = 0;                   // Start fresh chat
            displayChat();
        });

    }

});

// Display current chat
function displayChat() {
    const chatWindow = document.getElementById("chat-window");
    if (!chatWindow) return;
    chatWindow.innerHTML = "";

    chatHistory.forEach(msg => {
        const div = document.createElement("div");
        div.classList.add("message");

        if (msg.role === "user") {
            div.classList.add("user");
        } else if (msg.role === "assistant") {
            div.classList.add("assistant");
        }

        div.textContent = msg.content;
        chatWindow.appendChild(div);
    });

    // Auto-scroll to bottom
    chatWindow.scrollTop = chatWindow.scrollHeight;
}


// Display previous chats
function displayPreviousChats() {
    const prevSelect = document.getElementById("previous-chats");
    if (!prevSelect) return;

    // Clear current options
    prevSelect.innerHTML = "";
    const defaultOption = document.createElement("option");
    defaultOption.text = "Select a previous chat...";
    defaultOption.disabled = true;
    defaultOption.selected = true;
    prevSelect.appendChild(defaultOption);

    // Fill dropdown
    previousChats.forEach((chat, idx) => {
        const summary = chat
            .filter(m => m.role === "user")
            .map(m => m.content.substring(0, 20))
            .join(" | ");
        const option = document.createElement("option");
        option.value = idx;
        option.text = `Chat ${idx + 1}: ${summary}`;
        prevSelect.appendChild(option);
    });

    // Restore selected chat on change
    prevSelect.onchange = function () {
        const selectedIndex = this.value;
        if (selectedIndex !== null && previousChats[selectedIndex]) {
            chatHistory.length = 0;
            chatHistory.push(...previousChats[selectedIndex]);
            displayChat();
        }
    };
}

function cosineSimilarity(vec1, vec2) {
    const dot = vec1.reduce((acc, val, i) => acc + val * vec2[i], 0);
    const norm1 = Math.sqrt(vec1.reduce((acc, val) => acc + val * val, 0));
    const norm2 = Math.sqrt(vec2.reduce((acc, val) => acc + val * val, 0));
    return dot / (norm1 * norm2);
}