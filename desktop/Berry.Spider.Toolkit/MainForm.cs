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
        /// ѡ������Ӧ��
        /// </summary>��
        private void mbtn_select_app_Click(object sender, EventArgs e)
        {
            if (this.mfile_open_app.ShowDialog() == DialogResult.OK)
            {
                string filePath = this.mfile_open_app.FileName;
                this.mtxt_app_path.Text = filePath;
            }
        }

        /// <summary>
        /// ����
        /// </summary>
        private void mbtn_start_Click(object sender, EventArgs e)
        {
            string filePath = this.mtxt_app_path.Text;
            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("��ѡ����Ҫ������Ӧ��..", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        /// ֹͣ����
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
        /// �˳�Ӧ��
        /// </summary>
        private void �˳�ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}