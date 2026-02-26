namespace Lesson_13_WinForms
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            chatDisplay = new RichTextBox();
            inputTextBox = new TextBox();
            clearButton = new Button();
            sendButton = new Button();
            SuspendLayout();
            // 
            // chatDisplay
            // 
            chatDisplay.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            chatDisplay.Font = new Font("Segoe UI", 14F);
            chatDisplay.Location = new Point(20, 20);
            chatDisplay.Name = "chatDisplay";
            chatDisplay.ReadOnly = true;
            chatDisplay.Size = new Size(960, 500);
            chatDisplay.TabIndex = 0;
            chatDisplay.Text = "";
            chatDisplay.WordWrap = true;
            chatDisplay.ScrollBars = RichTextBoxScrollBars.Vertical;
            // 
            // inputTextBox
            // 
            inputTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            inputTextBox.Font = new Font("Segoe UI", 14F);
            inputTextBox.Location = new Point(20, 540);
            inputTextBox.Name = "inputTextBox";
            inputTextBox.Size = new Size(720, 38);
            inputTextBox.TabIndex = 1;
            inputTextBox.KeyDown += inputTextBox_KeyDown;
            // 
            // clearButton
            // 
            clearButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            clearButton.Font = new Font("Segoe UI", 14F);
            clearButton.Location = new Point(750, 540);
            clearButton.Name = "clearButton";
            clearButton.Size = new Size(100, 40);
            clearButton.TabIndex = 2;
            clearButton.Text = "Clear";
            clearButton.UseVisualStyleBackColor = true;
            clearButton.Click += clearButton_Click;
            // 
            // sendButton
            // 
            sendButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            sendButton.Font = new Font("Segoe UI", 14F);
            sendButton.Location = new Point(860, 540);
            sendButton.Name = "sendButton";
            sendButton.Size = new Size(120, 40);
            sendButton.TabIndex = 3;
            sendButton.Text = "Send";
            sendButton.UseVisualStyleBackColor = true;
            sendButton.Click += sendButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 600);
            Controls.Add(sendButton);
            Controls.Add(clearButton);
            Controls.Add(inputTextBox);
            Controls.Add(chatDisplay);
            Name = "Form1";
            Text = "Simple AI Chat";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox chatDisplay;
        private TextBox inputTextBox;
        private Button clearButton;
        private Button sendButton;
    }
}
