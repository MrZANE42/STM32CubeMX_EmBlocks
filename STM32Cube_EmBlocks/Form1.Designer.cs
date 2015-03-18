namespace STM32Cube_EmBlocks
{
    partial class Form1
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
      this.btnSelectFolder = new System.Windows.Forms.Button();
      this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
      this.tbProjectDirectory = new System.Windows.Forms.TextBox();
      this.lblProjectFolder = new System.Windows.Forms.Label();
      this.btnConvertProject = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // btnSelectFolder
      // 
      this.btnSelectFolder.Location = new System.Drawing.Point(15, 52);
      this.btnSelectFolder.Name = "btnSelectFolder";
      this.btnSelectFolder.Size = new System.Drawing.Size(132, 32);
      this.btnSelectFolder.TabIndex = 0;
      this.btnSelectFolder.Text = "Select Project Folder";
      this.btnSelectFolder.UseVisualStyleBackColor = true;
      this.btnSelectFolder.Click += new System.EventHandler(this.btnSelectFolder_Click_1);
      // 
      // tbProjectDirectory
      // 
      this.tbProjectDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tbProjectDirectory.Location = new System.Drawing.Point(15, 25);
      this.tbProjectDirectory.Name = "tbProjectDirectory";
      this.tbProjectDirectory.Size = new System.Drawing.Size(369, 20);
      this.tbProjectDirectory.TabIndex = 1;
      // 
      // lblProjectFolder
      // 
      this.lblProjectFolder.AutoSize = true;
      this.lblProjectFolder.Location = new System.Drawing.Point(12, 9);
      this.lblProjectFolder.Name = "lblProjectFolder";
      this.lblProjectFolder.Size = new System.Drawing.Size(72, 13);
      this.lblProjectFolder.TabIndex = 2;
      this.lblProjectFolder.Text = "Project folder:";
      // 
      // btnConvertProject
      // 
      this.btnConvertProject.Location = new System.Drawing.Point(252, 52);
      this.btnConvertProject.Name = "btnConvertProject";
      this.btnConvertProject.Size = new System.Drawing.Size(132, 32);
      this.btnConvertProject.TabIndex = 3;
      this.btnConvertProject.Text = "Convert project";
      this.btnConvertProject.UseVisualStyleBackColor = true;
      this.btnConvertProject.Click += new System.EventHandler(this.btnConvertProject_Click);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(396, 96);
      this.Controls.Add(this.btnConvertProject);
      this.Controls.Add(this.lblProjectFolder);
      this.Controls.Add(this.tbProjectDirectory);
      this.Controls.Add(this.btnSelectFolder);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.MaximumSize = new System.Drawing.Size(800, 120);
      this.Name = "Form1";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "STM32CubeMX (TrueSTUDIO)-> EmBlocks project converter";
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSelectFolder;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TextBox tbProjectDirectory;
        private System.Windows.Forms.Label lblProjectFolder;
        private System.Windows.Forms.Button btnConvertProject;
    }
}

