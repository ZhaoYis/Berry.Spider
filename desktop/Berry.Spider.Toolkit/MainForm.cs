using System.Diagnostics;

namespace Berry.Spider.Toolkit
{
    public partial class MainForm : Form
    {
        private static readonly List<Process> ProcessList = new List<Process>();

        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 选择启动应用
        /// </summary>、
        private void mbtn_select_app_Click(object sender, EventArgs e)
        {
            if (this.mfile_open_app.ShowDialog() == DialogResult.OK)
            {
                string filePath = this.mfile_open_app.FileName;
                this.mtxt_app_path.Text = filePath;
            }
        }

        /// <summary>
        /// 启动
        /// </summary>
        private void mbtn_start_Click(object sender, EventArgs e)
        {
            string filePath = this.mtxt_app_path.Text;
            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("请选择需要启动的应用..", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            int totalCount = (int)this.mnum_run_app_count.Value;
            for (int i = 0; i < totalCount; i++)
            {
                Process processInfo = Process.Start(filePath);
                ProcessList.Add(processInfo);
                Task.Delay(10);
            }
        }

        /// <summary>
        /// 停止所有
        /// </summary>
        private void mbtn_stop_all_Click(object sender, EventArgs e)
        {
            if (ProcessList.Count > 0)
            {
                foreach (Process process in ProcessList)
                {
                    process.Kill();
                    Task.Delay(10);
                }
            }
        }

        /// <summary>
        /// 退出应用
        /// </summary>
        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}