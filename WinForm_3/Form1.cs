namespace WinForm_3
{
    public partial class Form1 : Form
    {
        private Gemini_SDK? _gemini;
        private OpenAI_SDK_Response? _openAi;
        private string _activeProvider = string.Empty;
        private string _activeSystemPrompt = string.Empty;

        public Form1()
        {
            InitializeComponent();
            txtInput.KeyDown += TxtInput_KeyDown;
            cmbProvider.Items.AddRange(["Gemini", "OpenAI"]);
            cmbProvider.SelectedIndex = 0;
        }

        private async void TxtInput_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                e.SuppressKeyPress = true;
                await SendMessageAsync();
            }
        }

        private async void BtnSend_Click(object? sender, EventArgs e) =>
            await SendMessageAsync();

        private void BtnClear_Click(object? sender, EventArgs e)
        {
            rtbChat.Clear();
            _gemini?.ClearHistory();
            _openAi?.ClearHistory();
            lblStatus.Text = "Ready";
        }

        private async Task SendMessageAsync()
        {
            var userMessage = txtInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(userMessage)) return;

            SetUiBusy(true);
            AppendMessage("You", userMessage, Color.Blue);
            txtInput.Clear();

            try
            {
                EnsureServices();

                var aiName = _activeProvider == "OpenAI" ? "OpenAI" : "Gemini";
                StartAssistantMessage(aiName, Color.DarkGreen);

                if (_activeProvider == "OpenAI")
                {
                    await foreach (var chunk in _openAi!.CallStream(userMessage))
                    {
                        AppendAssistantChunk(chunk);
                    }
                }
                else
                {
                    await foreach (var chunk in _gemini!.CallStream(userMessage))
                    {
                        AppendAssistantChunk(chunk);
                    }
                }

                EndAssistantMessage();
            }
            catch (Exception ex)
            {
                EndAssistantMessage();
                AppendMessage("Error", ex.Message, Color.Red);
            }
            finally
            {
                SetUiBusy(false);
            }
        }

        private void EnsureServices()
        {
            var provider = cmbProvider.SelectedItem?.ToString() ?? "Gemini";
            var systemPrompt = txtSystemPrompt.Text.Trim();

            var providerChanged = !_activeProvider.Equals(provider, StringComparison.Ordinal);
            var promptChanged = !_activeSystemPrompt.Equals(systemPrompt, StringComparison.Ordinal);

            if (!providerChanged && !promptChanged) return;

            _activeProvider = provider;
            _activeSystemPrompt = systemPrompt;

            if (provider == "OpenAI")
            {
                _openAi = new OpenAI_SDK_Response("gpt-5.2", systemPrompt);
            }
            else
            {
                _gemini = new Gemini_SDK("gemini-2.5-flash", systemPrompt);
            }
        }

        private void StartAssistantMessage(string sender, Color color)
        {
            rtbChat.SelectionStart = rtbChat.TextLength;
            rtbChat.SelectionLength = 0;
            rtbChat.SelectionColor = color;
            rtbChat.SelectionFont = new Font(rtbChat.Font, FontStyle.Bold);
            rtbChat.AppendText($"{sender}:\n");
            rtbChat.SelectionFont = new Font(rtbChat.Font, FontStyle.Regular);
            rtbChat.SelectionColor = Color.Black;
        }

        private void AppendAssistantChunk(string chunk)
        {
            rtbChat.SelectionStart = rtbChat.TextLength;
            rtbChat.SelectionLength = 0;
            rtbChat.SelectionColor = Color.Black;
            rtbChat.AppendText(chunk);
            rtbChat.SelectionStart = rtbChat.TextLength;
            rtbChat.ScrollToCaret();
        }

        private void EndAssistantMessage()
        {
            rtbChat.SelectionStart = rtbChat.TextLength;
            rtbChat.SelectionLength = 0;
            rtbChat.SelectionColor = Color.Black;
            rtbChat.AppendText("\n\n");
            rtbChat.SelectionStart = rtbChat.TextLength;
            rtbChat.ScrollToCaret();
        }

        private void AppendMessage(string sender, string message, Color color)
        {
            rtbChat.SelectionStart = rtbChat.TextLength;
            rtbChat.SelectionLength = 0;
            rtbChat.SelectionColor = color;
            rtbChat.SelectionFont = new Font(rtbChat.Font, FontStyle.Bold);
            rtbChat.AppendText($"{sender}:\n");
            rtbChat.SelectionFont = new Font(rtbChat.Font, FontStyle.Regular);
            rtbChat.SelectionColor = Color.Black;
            rtbChat.AppendText($"{message}\n\n");
            rtbChat.SelectionStart = rtbChat.TextLength;
            rtbChat.ScrollToCaret();
        }

        private void SetUiBusy(bool busy)
        {
            btnSend.Enabled = !busy;
            txtInput.Enabled = !busy;
            cmbProvider.Enabled = !busy;
            txtSystemPrompt.Enabled = !busy;
            progressBar.Visible = busy;
            progressBar.Style = busy ? ProgressBarStyle.Marquee : ProgressBarStyle.Blocks;
            lblStatus.Text = busy ? "Streaming..." : "Ready";
        }
    }
}
