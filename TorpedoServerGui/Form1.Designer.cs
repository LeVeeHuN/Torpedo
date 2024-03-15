namespace TorpedoServerGui
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
            button_startServer = new Button();
            textBox_serverPort = new TextBox();
            label1 = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // button_startServer
            // 
            button_startServer.Location = new Point(176, 131);
            button_startServer.Name = "button_startServer";
            button_startServer.Size = new Size(100, 29);
            button_startServer.TabIndex = 0;
            button_startServer.Text = "Start szerver";
            button_startServer.UseVisualStyleBackColor = true;
            button_startServer.Click += button_startServer_Click;
            // 
            // textBox_serverPort
            // 
            textBox_serverPort.Location = new Point(176, 91);
            textBox_serverPort.MaxLength = 5;
            textBox_serverPort.Name = "textBox_serverPort";
            textBox_serverPort.Size = new Size(100, 23);
            textBox_serverPort.TabIndex = 1;
            textBox_serverPort.Text = "5100";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(176, 73);
            label1.Name = "label1";
            label1.Size = new Size(72, 15);
            label1.TabIndex = 2;
            label1.Text = "Szerver port:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(124, 22);
            label2.Name = "label2";
            label2.Size = new Size(205, 30);
            label2.TabIndex = 3;
            label2.Text = "LevTorpedó Szerver";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(444, 224);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(textBox_serverPort);
            Controls.Add(button_startServer);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Name = "Form1";
            Text = "LevTorpedo Szerver";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button_startServer;
        private TextBox textBox_serverPort;
        private Label label1;
        private Label label2;
    }
}
