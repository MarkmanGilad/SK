namespace WinForm_2
{
    public partial class Form1 : Form
    {
        private readonly Gemini_SDK _geminiSdk = new("gemini-2.5-flash");

        public Form1()
        {
            InitializeComponent();
            txtInput.KeyDown += TxtInput_KeyDown;
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
            _geminiSdk.ClearHistory();
        }

        private async Task SendMessageAsync()
        {
            var userMessage = txtInput.Text.Trim();
            if (string.IsNullOrEmpty(userMessage)) return;

            SetUiBusy(true);
            AppendMessage("אתה", userMessage, Color.Blue);
            txtInput.Clear();

            try
            {
                var response = await _geminiSdk.Call(userMessage);
                AppendMessage("Gemini", response, Color.DarkGreen);
            }
            catch (Exception ex)
            {
                AppendMessage("שגיאה", ex.Message, Color.Red);
            }
            finally
            {
                SetUiBusy(false);
            }
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
            progressBar.Visible = busy;
            progressBar.Style = busy ? ProgressBarStyle.Marquee : ProgressBarStyle.Blocks;
            lblStatus.Text = busy ? "שולח..." : "מוכן";
        }
    }
}
