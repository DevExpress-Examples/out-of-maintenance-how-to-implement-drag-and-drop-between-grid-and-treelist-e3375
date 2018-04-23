using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace DragAndDropBetweenGridAndTreeList
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            InitData();
            new GridManager(gridControl1);
            new TreeListManager(treeList1);
        }

        void InitData()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Name");
            table.Columns.Add("Address");
            for (int i = 0; i < 10; i++)
            {
                table.Rows.Add(new object[] { "Customer" + i, "Address" + i });
            }
            gridControl1.DataSource = table;
        }
    }
}
