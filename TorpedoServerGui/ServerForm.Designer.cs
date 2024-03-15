namespace TorpedoServerGui
{
    partial class ServerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            listBox1 = new ListBox();
            listBox2 = new ListBox();
            label2 = new Label();
            label3 = new Label();
            TextBox_Log = new RichTextBox();
            label4 = new Label();
            button1 = new Button();
            button_stop = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(24, 24);
            label1.Name = "label1";
            label1.Size = new Size(59, 23);
            label1.TabIndex = 0;
            label1.Text = "label1";
            label1.Click += label1_Click;
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new Point(24, 78);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(170, 184);
            listBox1.TabIndex = 1;
            listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            // 
            // listBox2
            // 
            listBox2.FormattingEnabled = true;
            listBox2.ItemHeight = 15;
            listBox2.Location = new Point(218, 78);
            listBox2.Name = "listBox2";
            listBox2.Size = new Size(168, 184);
            listBox2.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(24, 60);
            label2.Name = "label2";
            label2.Size = new Size(46, 15);
            label2.TabIndex = 3;
            label2.Text = "Játékok";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(218, 60);
            label3.Name = "label3";
            label3.Size = new Size(58, 15);
            label3.TabIndex = 4;
            label3.Text = "Játékosok";
            // 
            // TextBox_Log
            // 
            TextBox_Log.Location = new Point(392, 78);
            TextBox_Log.Name = "TextBox_Log";
            TextBox_Log.ReadOnly = true;
            TextBox_Log.Size = new Size(463, 328);
            TextBox_Log.TabIndex = 5;
            TextBox_Log.Text = "";
            TextBox_Log.TextChanged += TextBox_Log_TextChanged_1;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(392, 60);
            label4.Name = "label4";
            label4.Size = new Size(27, 15);
            label4.TabIndex = 7;
            label4.Text = "Log";
            // 
            // button1
            // 
            button1.Enabled = false;
            button1.Location = new Point(24, 308);
            button1.Name = "button1";
            button1.Size = new Size(170, 31);
            button1.TabIndex = 8;
            button1.Text = "Kiválasztott játék törlése";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button_stop
            // 
            button_stop.Location = new Point(24, 415);
            button_stop.Name = "button_stop";
            button_stop.Size = new Size(75, 23);
            button_stop.TabIndex = 9;
            button_stop.Text = "Stop";
            button_stop.UseVisualStyleBackColor = true;
            button_stop.Click += button_stop_Click;
            // 
            // ServerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(869, 450);
            Controls.Add(button_stop);
            Controls.Add(button1);
            Controls.Add(label4);
            Controls.Add(TextBox_Log);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(listBox2);
            Controls.Add(listBox1);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Name = "ServerForm";
            Text = "LevTorpedo Szerver";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private ListBox listBox1;
        private ListBox listBox2;
        private Label label2;
        private Label label3;
        private RichTextBox TextBox_Log;
        private Label label4;
        private Button button1;
        private Button button_stop;
    }
}