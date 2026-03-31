namespace WinForm_4
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            mainLayout = new TableLayoutPanel();
            txtConversation = new RichTextBox();
            bottomLayout = new TableLayoutPanel();
            txtUserMessage = new TextBox();
            btnSend = new Button();
            lblStatus = new Label();
            mainLayout.SuspendLayout();
            bottomLayout.SuspendLayout();
            SuspendLayout();

            mainLayout.ColumnCount = 1;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainLayout.Controls.Add(txtConversation, 0, 0);
            mainLayout.Controls.Add(bottomLayout, 0, 1);
            mainLayout.Controls.Add(lblStatus, 0, 2);
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.Location = new Point(12, 12);
            mainLayout.Name = "mainLayout";
            mainLayout.RowCount = 3;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 90F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            mainLayout.Size = new Size(760, 437);
            mainLayout.TabIndex = 0;

            txtConversation.BackColor = SystemColors.Window;
            txtConversation.Dock = DockStyle.Fill;
            txtConversation.Location = new Point(3, 3);
            txtConversation.Name = "txtConversation";
            txtConversation.ReadOnly = true;
            txtConversation.Size = new Size(754, 311);
            txtConversation.TabIndex = 0;
            txtConversation.Text = "";

            bottomLayout.ColumnCount = 2;
            bottomLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            bottomLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            bottomLayout.Controls.Add(txtUserMessage, 0, 0);
            bottomLayout.Controls.Add(btnSend, 1, 0);
            bottomLayout.Dock = DockStyle.Fill;
            bottomLayout.Location = new Point(3, 320);
            bottomLayout.Name = "bottomLayout";
            bottomLayout.RowCount = 1;
            bottomLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            bottomLayout.Size = new Size(754, 84);
            bottomLayout.TabIndex = 1;

            txtUserMessage.Dock = DockStyle.Fill;
            txtUserMessage.Location = new Point(3, 3);
            txtUserMessage.Multiline = true;
            txtUserMessage.Name = "txtUserMessage";
            txtUserMessage.ScrollBars = ScrollBars.Vertical;
            txtUserMessage.Size = new Size(628, 78);
            txtUserMessage.TabIndex = 0;
            txtUserMessage.KeyDown += txtUserMessage_KeyDown;

            btnSend.Dock = DockStyle.Fill;
            btnSend.Location = new Point(637, 3);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(114, 78);
            btnSend.TabIndex = 1;
            btnSend.Text = "Send";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;

            lblStatus.AutoSize = true;
            lblStatus.Dock = DockStyle.Fill;
            lblStatus.Location = new Point(3, 407);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(754, 30);
            lblStatus.TabIndex = 2;
            lblStatus.Text = "Ready";
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;

            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 461);
            Controls.Add(mainLayout);
            MinimumSize = new Size(600, 400);
            Name = "Form1";
            Padding = new Padding(12);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Gemini Chat";
            mainLayout.ResumeLayout(false);
            mainLayout.PerformLayout();
            bottomLayout.ResumeLayout(false);
            bottomLayout.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel mainLayout;
        private RichTextBox txtConversation;
        private TableLayoutPanel bottomLayout;
        private TextBox txtUserMessage;
        private Button btnSend;
        private Label lblStatus;
    }
}
