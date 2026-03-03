namespace WinForm_3
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.RichTextBox rtbChat;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Panel pnlInput;
        private System.Windows.Forms.Panel pnlStatus;
        private System.Windows.Forms.Panel pnlSettings;
        private System.Windows.Forms.ComboBox cmbProvider;
        private System.Windows.Forms.Label lblProvider;
        private System.Windows.Forms.TextBox txtSystemPrompt;
        private System.Windows.Forms.Label lblSystemPrompt;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.rtbChat = new System.Windows.Forms.RichTextBox();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblStatus = new System.Windows.Forms.Label();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.pnlSettings = new System.Windows.Forms.Panel();
            this.cmbProvider = new System.Windows.Forms.ComboBox();
            this.lblProvider = new System.Windows.Forms.Label();
            this.txtSystemPrompt = new System.Windows.Forms.TextBox();
            this.lblSystemPrompt = new System.Windows.Forms.Label();
            this.pnlInput = new System.Windows.Forms.Panel();
            this.pnlStatus = new System.Windows.Forms.Panel();

            this.tableLayoutPanel.SuspendLayout();
            this.pnlSettings.SuspendLayout();
            this.pnlInput.SuspendLayout();
            this.pnlStatus.SuspendLayout();
            this.SuspendLayout();

            // rtbChat
            this.rtbChat.BackColor = System.Drawing.Color.White;
            this.rtbChat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbChat.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.rtbChat.Name = "rtbChat";
            this.rtbChat.ReadOnly = true;
            this.rtbChat.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtbChat.TabIndex = 0;
            this.rtbChat.Text = string.Empty;

            // pnlSettings
            this.pnlSettings.Controls.Add(this.lblProvider);
            this.pnlSettings.Controls.Add(this.cmbProvider);
            this.pnlSettings.Controls.Add(this.lblSystemPrompt);
            this.pnlSettings.Controls.Add(this.txtSystemPrompt);
            this.pnlSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSettings.Name = "pnlSettings";
            this.pnlSettings.Size = new System.Drawing.Size(950, 44);
            this.pnlSettings.TabIndex = 1;

            // lblProvider
            this.lblProvider.AutoSize = true;
            this.lblProvider.Location = new System.Drawing.Point(8, 14);
            this.lblProvider.Name = "lblProvider";
            this.lblProvider.Size = new System.Drawing.Size(55, 15);
            this.lblProvider.Text = "Provider:";

            // cmbProvider
            this.cmbProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProvider.FormattingEnabled = true;
            this.cmbProvider.Location = new System.Drawing.Point(69, 10);
            this.cmbProvider.Name = "cmbProvider";
            this.cmbProvider.Size = new System.Drawing.Size(140, 23);
            this.cmbProvider.TabIndex = 0;

            // lblSystemPrompt
            this.lblSystemPrompt.AutoSize = true;
            this.lblSystemPrompt.Location = new System.Drawing.Point(235, 14);
            this.lblSystemPrompt.Name = "lblSystemPrompt";
            this.lblSystemPrompt.Size = new System.Drawing.Size(88, 15);
            this.lblSystemPrompt.Text = "System Prompt:";

            // txtSystemPrompt
            this.txtSystemPrompt.Anchor = System.Windows.Forms.AnchorStyles.Top
                                       | System.Windows.Forms.AnchorStyles.Left
                                       | System.Windows.Forms.AnchorStyles.Right;
            this.txtSystemPrompt.Location = new System.Drawing.Point(335, 10);
            this.txtSystemPrompt.Margin = new System.Windows.Forms.Padding(12, 3, 3, 3);
            this.txtSystemPrompt.Name = "txtSystemPrompt";
            this.txtSystemPrompt.PlaceholderText = "Optional system prompt";
            this.txtSystemPrompt.Size = new System.Drawing.Size(604, 23);
            this.txtSystemPrompt.TabIndex = 1;

            // txtInput
            this.txtInput.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                                 | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.txtInput.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtInput.Location = new System.Drawing.Point(3, 3);
            this.txtInput.Multiline = true;
            this.txtInput.Name = "txtInput";
            this.txtInput.PlaceholderText = "Type a message... (Enter to send, Shift+Enter new line)";
            this.txtInput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtInput.Size = new System.Drawing.Size(862, 82);
            this.txtInput.TabIndex = 2;

            // btnSend
            this.btnSend.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnSend.Location = new System.Drawing.Point(871, 3);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 38);
            this.btnSend.TabIndex = 3;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.BtnSend_Click);

            // btnClear
            this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnClear.Location = new System.Drawing.Point(871, 47);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 38);
            this.btnClear.TabIndex = 4;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.BtnClear_Click);

            // pnlInput
            this.pnlInput.Controls.Add(this.txtInput);
            this.pnlInput.Controls.Add(this.btnSend);
            this.pnlInput.Controls.Add(this.btnClear);
            this.pnlInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlInput.Name = "pnlInput";
            this.pnlInput.Size = new System.Drawing.Size(950, 90);
            this.pnlInput.TabIndex = 2;

            // progressBar
            this.progressBar.Anchor = System.Windows.Forms.AnchorStyles.Top
                                    | System.Windows.Forms.AnchorStyles.Left
                                    | System.Windows.Forms.AnchorStyles.Right;
            this.progressBar.Location = new System.Drawing.Point(3, 5);
            this.progressBar.MarqueeAnimationSpeed = 30;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(800, 18);
            this.progressBar.TabIndex = 0;
            this.progressBar.Visible = false;

            // lblStatus
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblStatus.Location = new System.Drawing.Point(810, 7);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Text = "Ready";

            // pnlStatus
            this.pnlStatus.Controls.Add(this.progressBar);
            this.pnlStatus.Controls.Add(this.lblStatus);
            this.pnlStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Size = new System.Drawing.Size(950, 30);
            this.pnlStatus.TabIndex = 3;

            // tableLayoutPanel
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.pnlSettings, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.rtbChat, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.pnlInput, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.pnlStatus, 0, 3);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 4;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(950, 650);
            this.tableLayoutPanel.TabIndex = 0;

            // Form1
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(950, 650);
            this.Controls.Add(this.tableLayoutPanel);
            this.MinimumSize = new System.Drawing.Size(700, 500);
            this.Name = "Form1";
            this.Text = "Streaming Chat - Gemini / OpenAI";

            this.tableLayoutPanel.ResumeLayout(false);
            this.pnlSettings.ResumeLayout(false);
            this.pnlSettings.PerformLayout();
            this.pnlInput.ResumeLayout(false);
            this.pnlInput.PerformLayout();
            this.pnlStatus.ResumeLayout(false);
            this.pnlStatus.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
