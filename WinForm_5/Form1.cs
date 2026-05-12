namespace WinForm_5
{
    public partial class Form1 : Form
    {
        private Gemini_SDK gemini;

        public Form1()
        {
            InitializeComponent();
            InitializeGemini();
            txtInput.KeyDown += TxtInput_KeyDown;
        }

        private void InitializeGemini()
        {
            var envPath = Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\..\\.env");
            var apiKey = LoadApiKey(envPath);
            Environment.SetEnvironmentVariable("GOOGLE_API_KEY", apiKey);
            gemini = new Gemini_SDK("gemini-2.5-flash");
        }

        private string LoadApiKey(string envPath)
        {
            var lines = File.ReadAllLines(envPath);
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (line.StartsWith("GeminiAPIKey=") || line.StartsWith("GOOGLE_API_KEY="))
                {
                    return line.Split('=')[1].Trim();
                }
            }
            throw new Exception("API Key not found in .env file");
        }

        private void TxtInput_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                e.SuppressKeyPress = true;
                btnSend_Click(sender, e);
            }
        }

        private async void btnSend_Click(object? sender, EventArgs e)
        {
            var userMessage = txtInput.Text.Trim();
            if (string.IsNullOrEmpty(userMessage))
                return;

            txtInput.Clear();
            btnSend.Enabled = false;
            txtInput.Enabled = false;

            AppendMessage("אתה: ", userMessage, System.Drawing.Color.Blue);

            try
            {
                var response = await gemini.Call(userMessage);
                AppendMessage("Gemini: ", response, System.Drawing.Color.Green);
            }
            catch (Exception ex)
            {
                AppendMessage("שגיאה: ", ex.Message, System.Drawing.Color.Red);
            }
            finally
            {
                btnSend.Enabled = true;
                txtInput.Enabled = true;
                txtInput.Focus();
            }
        }

        private void AppendMessage(string prefix, string message, System.Drawing.Color color)
        {
            txtChat.SelectionStart = txtChat.TextLength;
            txtChat.SelectionLength = 0;

            txtChat.SelectionColor = color;
            txtChat.SelectionFont = new Font(txtChat.Font, FontStyle.Bold);
            txtChat.AppendText(prefix);

            txtChat.SelectionFont = new Font(txtChat.Font, FontStyle.Regular);
            txtChat.AppendText(message + Environment.NewLine + Environment.NewLine);

            txtChat.SelectionColor = txtChat.ForeColor;
            txtChat.ScrollToCaret();
        }
    }
}
