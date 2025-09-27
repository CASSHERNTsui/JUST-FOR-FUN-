using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 工师校园地貌
{
    public partial class Attribute : DevComponents.DotNetBar.Office2007Form
    {
        public Attribute()
        {
            InitializeComponent();
        }

        private void Attribute_Load(object sender, EventArgs e)
        {
            this.ClientSize = this.dataGrid1.Size;//使尺寸相等
        }

        public DataGridView GetDataGrid
        {
            get
            {
                return dataGrid1;
            }
        }

    
    }
}
