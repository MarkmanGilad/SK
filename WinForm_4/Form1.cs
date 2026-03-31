namespace WinForm_4
{
    public partial class Form1 : Form
    {
        private readonly Gemini_SDK gemini = new("gemini-2.5-flash");

        public Form1()
        {
            InitializeComponent();
            lblStatus.Text = "Ready";
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            await SendMessageAsync();
        }

        private async void txtUserMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter || e.Shift)
            {
                return;
            }

            e.SuppressKeyPress = true;
            e.Handled = true;
            await SendMessageAsync();
        }

        private async Task SendMessageAsync()
        {
            if (!btnSend.Enabled)
            {
                return;
            }

            var userMessage = txtUserMessage.Text.Trim();

            if (string.IsNullOrWhiteSpace(userMessage))
            {
                txtUserMessage.Focus();
                return;
            }

            AppendMessage("You", userMessage);
            txtUserMessage.Clear();
            SetBusyState(true);

            try
            {
                var response = await gemini.Call(userMessage);
                AppendMessage("AI", response);
                lblStatus.Text = "Ready";
            }
            catch (Exception ex)
            {
                AppendMessage("System", $"Error: {ex.Message}");
                lblStatus.Text = "Request failed";
            }
            finally
            {
                SetBusyState(false);
                txtUserMessage.Focus();
            }
        }

        private void AppendMessage(string role, string message)
        {
            if (txtConversation.TextLength > 0)
            {
                txtConversation.AppendText(Environment.NewLine + Environment.NewLine);
            }

            txtConversation.AppendText($"{role}:{Environment.NewLine}{message}");
            txtConversation.SelectionStart = txtConversation.TextLength;
            txtConversation.ScrollToCaret();
        }

        private void SetBusyState(bool isBusy)
        {
            btnSend.Enabled = !isBusy;
            txtUserMessage.Enabled = !isBusy;
            lblStatus.Text = isBusy ? "Sending..." : "Ready";
        }
    }
}
