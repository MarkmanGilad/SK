document.addEventListener('DOMContentLoaded', function () {
    // Show spinner and disable button on upload submit (keep file input enabled)
    const uploadForm = document.getElementById('uploadForm');
    const uploadBtn = document.getElementById('uploadBtn');
    const uploadSpinner = document.getElementById('uploadSpinner');
    const uploadText = document.getElementById('uploadText');

    if (uploadForm) {
        uploadForm.addEventListener('submit', function () {
            if (uploadSpinner) uploadSpinner.classList.remove('d-none');
            if (uploadText) uploadText.textContent = '📚 Processing...';
            if (uploadBtn) uploadBtn.disabled = true;
        });
    }

    
    const chatArea = document.getElementById('chatArea');
    if (chatArea) {
        chatArea.scrollTop = chatArea.scrollHeight;
    }

    const messageInput = document.getElementById('messageInput');
    if (messageInput && !messageInput.disabled) {
        messageInput.focus();
        messageInput.addEventListener('keypress', function (e) {
            if (e.key === 'Enter' && !e.shiftKey && !this.disabled) {
                e.preventDefault();
                const form = this.closest('form');
                if (form) form.submit();
            }
        });
    }
});