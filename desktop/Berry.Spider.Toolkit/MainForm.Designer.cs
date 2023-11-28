namespace Berry.Spider.Toolkit
{
    partial class MainForm
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
            menuStrip1 = new MenuStrip();
            toolStripMenuItem1 = new ToolStripMenuItem();
            退出ToolStripMenuItem = new ToolStripMenuItem();
            label1 = new Label();
            mtxt_app_path = new TextBox();
            mbtn_select_app = new Button();
            label2 = new Label();
            mnum_run_app_count = new NumericUpDown();
            mbtn_start = new Button();
            mbtn_stop_all = new Button();
            mfile_open_app = new OpenFileDialog();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)mnum_run_app_count).BeginInit();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { toolStripMenuItem1 });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(732, 25);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { 退出ToolStripMenuItem });
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(44, 21);
            toolStripMenuItem1.Text = "文件";
            // 
            // 退出ToolStripMenuItem
            // 
            退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            退出ToolStripMenuItem.Size = new Size(180, 22);
            退出ToolStripMenuItem.Text = "退出";
            退出ToolStripMenuItem.Click += 退出ToolStripMenuItem_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(18, 38);
            label1.Name = "label1";
            label1.Size = new Size(92, 17);
            label1.TabIndex = 1;
            label1.Text = "启动程序路径：";
            // 
            // mtxt_app_path
            // 
            mtxt_app_path.Location = new Point(116, 35);
            mtxt_app_path.Name = "mtxt_app_path";
            mtxt_app_path.ReadOnly = true;
            mtxt_app_path.Size = new Size(491, 23);
            mtxt_app_path.TabIndex = 2;
            // 
            // mbtn_select_app
            // 
            mbtn_select_app.Cursor = Cursors.Hand;
            mbtn_select_app.Location = new Point(613, 35);
            mbtn_select_app.Name = "mbtn_select_app";
            mbtn_select_app.Size = new Size(108, 23);
            mbtn_select_app.TabIndex = 3;
            mbtn_select_app.Text = "选择启动程序..";
            mbtn_select_app.UseVisualStyleBackColor = true;
            mbtn_select_app.Click += mbtn_select_app_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(18, 84);
            label2.Name = "label2";
            label2.Size = new Size(92, 17);
            label2.TabIndex = 4;
            label2.Text = "启动程序数量：";
            // 
            // mnum_run_app_count
            // 
            mnum_run_app_count.Location = new Point(117, 81);
            mnum_run_app_count.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            mnum_run_app_count.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            mnum_run_app_count.Name = "mnum_run_app_count";
            mnum_run_app_count.ReadOnly = true;
            mnum_run_app_count.Size = new Size(198, 23);
            mnum_run_app_count.TabIndex = 5;
            mnum_run_app_count.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // mbtn_start
            // 
            mbtn_start.Cursor = Cursors.Hand;
            mbtn_start.Location = new Point(142, 143);
            mbtn_start.Name = "mbtn_start";
            mbtn_start.Size = new Size(177, 66);
            mbtn_start.TabIndex = 6;
            mbtn_start.Text = "启动";
            mbtn_start.UseVisualStyleBackColor = true;
            mbtn_start.Click += mbtn_start_Click;
            // 
            // mbtn_stop_all
            // 
            mbtn_stop_all.Cursor = Cursors.Hand;
            mbtn_stop_all.Location = new Point(402, 143);
            mbtn_stop_all.Name = "mbtn_stop_all";
            mbtn_stop_all.Size = new Size(177, 66);
            mbtn_stop_all.TabIndex = 7;
            mbtn_stop_all.Text = "停止所有";
            mbtn_stop_all.UseVisualStyleBackColor = true;
            mbtn_stop_all.Click += mbtn_stop_all_Click;
            // 
            // mfile_open_app
            // 
            mfile_open_app.FileName = "请选择需要启动的应用";
            mfile_open_app.Filter = "EXE可执行文件|*.exe";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(732, 256);
            Controls.Add(mbtn_stop_all);
            Controls.Add(mbtn_start);
            Controls.Add(mnum_run_app_count);
            Controls.Add(label2);
            Controls.Add(mbtn_select_app);
            Controls.Add(mtxt_app_path);
            Controls.Add(label1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "浆果小助手";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)mnum_run_app_count).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem 退出ToolStripMenuItem;
        private Label label1;
        private TextBox mtxt_app_path;
        private Button mbtn_select_app;
        private Label label2;
        private NumericUpDown mnum_run_app_count;
        private Button mbtn_start;
        private Button mbtn_stop_all;
        private OpenFileDialog mfile_open_app;
    }
}
