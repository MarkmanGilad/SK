using DotNetEnv;

namespace Lesson_13_WinForms
{
    public partial class Form1 : Form
    {
        private OpenAI_SDK_Response? _client;

        public Form1()
        {
            InitializeComponent();
            InitializeChat();
        }

        private void InitializeChat()
        {
            Env.TraversePath().Load();

            _client = new OpenAI_SDK_Response("gpt-5.2");

            chatDisplay.AppendText("Chat ready. Type your message and press Send.\n\n");
        }

        private async void sendButton_Click(object sender, EventArgs e)
        {
            await SendMessageAsync();
        }

        private async void inputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                await SendMessageAsync();
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            _client!.ClearHistory();
            chatDisplay.Clear();
            chatDisplay.AppendText("Chat cleared. Start a new conversation.\n\n");
        }

        private async Task SendMessageAsync()
        {
            var userMessage = inputTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(userMessage)) return;

            sendButton.Enabled = false;
            inputTextBox.Enabled = false;

            chatDisplay.AppendText($"You: {userMessage}\n");
            inputTextBox.Clear();

            var response = await _client!.Call(userMessage);
            chatDisplay.AppendText($"Assistant: {response}\n\n");

            sendButton.Enabled = true;
            inputTextBox.Enabled = true;
            inputTextBox.Focus();
            chatDisplay.SelectionStart = chatDisplay.TextLength;
            chatDisplay.ScrollToCaret();
        }
    }
}
