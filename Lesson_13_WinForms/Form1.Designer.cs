namespace Lesson_13_WinForms
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
            chatDisplay = new RichTextBox();
            inputTextBox = new TextBox();
            sendButton = new Button();
            SuspendLayout();
            // 
            // chatDisplay
            // 
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
            inputTextBox.Font = new Font("Segoe UI", 14F);
            inputTextBox.Location = new Point(20, 540);
            inputTextBox.Name = "inputTextBox";
            inputTextBox.Size = new Size(840, 38);
            inputTextBox.TabIndex = 1;
            inputTextBox.KeyDown += inputTextBox_KeyDown;
            // 
            // sendButton
            // 
            sendButton.Font = new Font("Segoe UI", 14F);
            sendButton.Location = new Point(870, 540);
            sendButton.Name = "sendButton";
            sendButton.Size = new Size(110, 40);
            sendButton.TabIndex = 2;
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
        private Button sendButton;
    }
}
