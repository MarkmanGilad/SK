let isUploading = false;

// Upload form validation and spinner
document.getElementById('uploadForm').addEventListener('submit', function (e) {
    const fileInput = document.getElementById('fileInput');
    const uploadBtn = document.getElementById('uploadBtn');
    const uploadSpinner = document.getElementById('uploadSpinner');
    const uploadText = document.getElementById('uploadText');
    const processingSection = document.getElementById('processingSection');
    const clearBtn = document.getElementById('clearBtn');
    const messageInput = document.getElementById('messageInput');
    const fileHint = document.getElementById('fileHint');

    // Validate file selection
    if (!fileInput.files || !fileInput.files.length) {
        e.preventDefault();
        alert('Please select a PDF file to upload.');
        return false;
    }

    // Validate file type
    const file = fileInput.files[0];
    if (!file.name.toLowerCase().endsWith('.pdf')) {
        e.preventDefault();
        alert('Please select a PDF file only.');
        return false;
    }

    // Prevent double submission
    if (isUploading) {
        e.preventDefault();
        return false;
    }

    isUploading = true;
    console.log('Starting upload process for:', file.name);

    // Show button spinner
    uploadSpinner.classList.remove('d-none');
    uploadText.textContent = '📚 Processing...';
    uploadBtn.disabled = true;

    // Show main processing section if it exists
    if (processingSection) {
        processingSection.classList.remove('d-none');
    }

    // Update hint text
    fileHint.textContent = `Processing: ${file.name}`;
    fileHint.classList.add('text-info');

    // Disable other controls (DO NOT disable the file input or it won't be posted)
    clearBtn.disabled = true;
    // fileInput.disabled = true; // <-- Remove/keep commented
    // If you want to block interaction without breaking post, use CSS:
    fileInput.style.pointerEvents = 'none';
    fileInput.classList.add('disabled'); // optional styling hook

    if (messageInput) messageInput.disabled = true;

    // Animate processing steps if they exist
    setTimeout(() => document.getElementById('step1')?.classList.add('text-success'), 1000);
    setTimeout(() => document.getElementById('step2')?.classList.add('text-success'), 3000);
    setTimeout(() => document.getElementById('step3')?.classList.add('text-success'), 5000);
    setTimeout(() => document.getElementById('step4')?.classList.add('text-success'), 7000);

    // Allow form submission
    return true;
});

// Reset upload state when page loads (in case of errors)
window.addEventListener('load', function () {
    isUploading = false;

    // Check if we have success or error messages by looking for alert elements
    const hasSuccessMessage = document.querySelector('.alert-success') !== null;
    const hasErrorMessage = document.querySelector('.alert-danger') !== null;
    const hasMessage = hasSuccessMessage || hasErrorMessage;

    if (hasMessage) {
        const processingSection = document.getElementById('processingSection');
        const uploadBtn = document.getElementById('uploadBtn');
        const uploadSpinner = document.getElementById('uploadSpinner');
        const uploadText = document.getElementById('uploadText');
        const clearBtn = document.getElementById('clearBtn');
        const fileInput = document.getElementById('fileInput');
        const messageInput = document.getElementById('messageInput');
        const fileHint = document.getElementById('fileHint');

        // Hide processing section
        if (processingSection) processingSection.classList.add('d-none');

        // Reset button state
        if (uploadBtn) uploadBtn.disabled = false;
        if (uploadSpinner) uploadSpinner.classList.add('d-none');
        if (uploadText) uploadText.textContent = '📚 Load PDF';

        // Reset other controls
        if (clearBtn) clearBtn.disabled = false;
        if (fileInput) {
            // Re-enable interaction if we blocked it with CSS
            fileInput.style.pointerEvents = '';
            fileInput.classList.remove('disabled');
        }
        if (messageInput) messageInput.disabled = false;
        if (fileHint) {
            fileHint.textContent = 'Select a PDF file to upload and index for RAG chat.';
            fileHint.classList.remove('text-info');
        }
    }
});

// Auto-scroll chat area to bottom
const chatArea = document.getElementById('chatArea');
if (chatArea) {
    chatArea.scrollTop = chatArea.scrollHeight;
}

// Auto-focus on message input when page loads
const messageInput = document.getElementById('messageInput');
if (messageInput && !messageInput.disabled) {
    messageInput.focus();
}

// Handle enter key in message input
document.getElementById('messageInput')?.addEventListener('keypress', function (e) {
    if (e.key === 'Enter' && !e.shiftKey && !this.disabled) {
        e.preventDefault();
        this.closest('form').submit();
    }
});