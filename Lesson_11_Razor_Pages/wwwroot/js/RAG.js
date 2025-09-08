document.addEventListener('DOMContentLoaded', function () {
    // Upload spinner
    const uploadForm = document.getElementById('uploadForm');
    const uploadBtn = document.getElementById('uploadBtn');
    const uploadSpinner = document.getElementById('uploadSpinner');
    const uploadText = document.getElementById('uploadText');

    if (uploadForm) {
        uploadForm.addEventListener('submit', function () {
            if (uploadSpinner) uploadSpinner.classList.remove('d-none');
            if (uploadText) uploadText.textContent = '📚 Processing...';
            if (uploadBtn) uploadBtn.disabled = true;
            // Keep file input enabled so the file is posted
        });
    }

    // Chat (LLM) spinner
    const chatForm = document.getElementById('chatForm');
    const chatBtn = document.getElementById('chatBtn');
    const chatSpinner = document.getElementById('chatSpinner');
    const chatText = document.getElementById('chatText');

    if (chatForm) {
        chatForm.addEventListener('submit', function () {
            if (chatSpinner) chatSpinner.classList.remove('d-none');
            if (chatText) chatText.textContent = 'Sending...';
            if (chatBtn) chatBtn.disabled = true;
            // Do not disable messageInput; disabled inputs are not posted
        });
    }

    // Auto-scroll chat to bottom
    const chatArea = document.getElementById('chatArea');
    if (chatArea) chatArea.scrollTop = chatArea.scrollHeight;

    // Focus and Enter-to-send with submit event (so spinner shows)
    const messageInput = document.getElementById('messageInput');
    if (messageInput && !messageInput.disabled) {
        messageInput.focus();
        messageInput.addEventListener('keypress', function (e) {
            if (e.key === 'Enter' && !e.shiftKey && !this.disabled) {
                e.preventDefault();
                const form = this.closest('form');
                if (form) {
                    if (typeof form.requestSubmit === 'function') form.requestSubmit();
                    else form.submit();
                }
            }
        });
    }
});