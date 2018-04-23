// Developer Express Code Central Example:
// How to implement drag-and-drop between Grid and TreeList
// 
// This example demonstrates how to create helper classes for implementing
// drag-and-drop between Grid and TreeList. For more information please refer
// to:
// http://www.devexpress.com/scid=A2343
// http://www.devexpress.com/scid=K18056
// http://www.devexpress.com/scid=A1444
// 
// You can find sample updates and versions for different programming languages here:
// http://www.devexpress.com/example=E3375

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
