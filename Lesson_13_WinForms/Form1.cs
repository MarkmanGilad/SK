using DotNetEnv;

namespace Lesson_13_WinForms
{
    public partial class Form1 : Form
    {
        private OpenAIClient? _client;
        private ChatHistory? _history;

        public Form1()
        {
            InitializeComponent();
            InitializeChat();
        }

        private void InitializeChat()
        {
            Env.Load(@"C:\Users\Gilad\source\repos\SK\.env");
            var apiKey = Environment.GetEnvironmentVariable("OpenAIKey");

            _client = new OpenAIClient(apiKey, "gpt-5-mini");
            _history = new ChatHistory();
            _history.AddSystemMessage("You are a helpful assistant.");

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

        private async Task SendMessageAsync()
        {
            var userMessage = inputTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(userMessage)) return;

            sendButton.Enabled = false;
            inputTextBox.Enabled = false;

            chatDisplay.AppendText($"You: {userMessage}\n");
            inputTextBox.Clear();

            _history!.AddUserMessage(userMessage);

            var response = await _client!.GetCompletionAsync(_history);
            _history.AddAssistantMessage(response);
            chatDisplay.AppendText($"Assistant: {response}\n\n");

            sendButton.Enabled = true;
            inputTextBox.Enabled = true;
            inputTextBox.Focus();
            chatDisplay.SelectionStart = chatDisplay.TextLength;
            chatDisplay.ScrollToCaret();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _client?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
