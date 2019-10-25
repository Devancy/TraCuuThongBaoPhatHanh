using System.Data;
using System.Windows.Forms;

namespace TraCuuThongBaoPhatHanh_v2
{
    public partial class Grid : Form
    {
        private Grid()
        {
            InitializeComponent();
        }

        public Grid(DataTable dataSource) : this()
        {
            this.dataGridView1.DataSource = dataSource;
        }
    }
}
