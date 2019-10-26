using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace TraCuuThongBaoPhatHanh_v2
{
    public partial class Grid : Form
    {
        private Grid()
        {
            InitializeComponent();
        }

        public Grid(DataTable dataSource, string elapsed) : this()
        {
            this.Text = $"Chi tiết thông báo phát hành - [{elapsed}]";
            this.dataGridView.DataSource = dataSource;
            dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font(DataGridView.DefaultFont, FontStyle.Bold);
            
            var activeCol = dataGridView.Columns["Ngày bắt đầu sử dụng"];
            if (activeCol != null)
                activeCol.DefaultCellStyle.ForeColor = Color.OrangeRed;
        }

        private void DataGridView_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            if (dataGridView.Rows[e.RowIndex].Cells["Mã số thuế"]?.Value is string tax && tax.Trim() == "0105987432")
            {
                var centerFormat = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, dataGridView.RowHeadersWidth, e.RowBounds.Height);
                e.Graphics.DrawString("SDS", this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);

                dataGridView.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.OrangeRed;
            }

            if (dataGridView.Rows[e.RowIndex].Cells["Ký hiệu"]?.Value is string serial && serial.Trim().EndsWith("E", System.StringComparison.InvariantCultureIgnoreCase))
            {
                dataGridView.Rows[e.RowIndex].Cells["Ký hiệu"].Style.ForeColor = dataGridView.DefaultCellStyle.SelectionForeColor;
                dataGridView.Rows[e.RowIndex].Cells["Ký hiệu"].Style.BackColor = dataGridView.DefaultCellStyle.SelectionBackColor;
            }
        }
    }
}
