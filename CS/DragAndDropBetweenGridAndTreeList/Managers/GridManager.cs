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
using System.Linq;
using DevExpress.XtraGrid;
using System.Data;
using DevExpress.XtraTreeList.Nodes;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System.Drawing;
using DevExpress.XtraTreeList;

namespace DragAndDropBetweenGridAndTreeList
{
    class GridManager
    {
        readonly GridControl _gridControl;
        GridHitInfo _dragStartHitInfo;
        Cursor _dragRowCursor;
        readonly DragGridImageHelper _imageHelper;

        public GridManager(GridControl grid)
        {
            _gridControl = grid;
            SetUpGrid(grid);
            _imageHelper = new DragGridImageHelper(grid.FocusedView as GridView);
        }

        public void SetUpGrid(GridControl grid)
        {
            grid.AllowDrop = true;
            grid.DragOver += grid_DragOver;
            grid.DragDrop += grid_DragDrop;
            grid.DragLeave += grid_DragLeave;
            grid.Paint += grid_Paint;
            grid.GiveFeedback += grid_GiveFeedback;
            GridView view = grid.MainView as GridView;
            view.OptionsBehavior.Editable = false;
            view.MouseMove += view_MouseMove;
            view.MouseDown += view_MouseDown;
        }

        private void view_MouseDown(object sender, MouseEventArgs e)
        {
            GridView view = sender as GridView;
            _dragStartHitInfo = null;
            GridHitInfo hitInfo = view.CalcHitInfo(new Point(e.X, e.Y));
            if (Control.ModifierKeys != Keys.None)
                return;
            if (e.Button == MouseButtons.Left && hitInfo.RowHandle >= 0)
                _dragStartHitInfo = hitInfo;
        }

        private void view_MouseMove(object sender, MouseEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Button == MouseButtons.Left && _dragStartHitInfo != null)
            {
                Size dragSize = SystemInformation.DragSize;
                Rectangle dragRect = new Rectangle(new Point(_dragStartHitInfo.HitPoint.X - dragSize.Width / 2, _dragStartHitInfo.HitPoint.Y - dragSize.Height / 2), dragSize);
                if (!dragRect.Contains(new Point(e.X, e.Y)))
                {
                    _dragRowCursor = _imageHelper.GetDragCursor(_dragStartHitInfo.RowHandle, e.Location);
                    DataRow row = view.GetDataRow(_dragStartHitInfo.RowHandle);
                    view.GridControl.DoDragDrop(row, DragDropEffects.Move);
                    _dragStartHitInfo = null;
                    DevExpress.Utils.DXMouseEventArgs.GetMouseArgs(e).Handled = true;
                }
            }
        }

        int dropTargetRowHandle = -1;
        int DropTargetRowHandle
        {
            get
            {
                return dropTargetRowHandle;
            }
            set
            {
                dropTargetRowHandle = value;
                _gridControl.Invalidate();
            }
        }

        private void grid_DragDrop(object sender, DragEventArgs e)
        {
            GridControl grid = sender as GridControl;
            DataTable table = grid.DataSource as DataTable;
            TreeListNode node = e.Data.GetData(typeof(TreeListNode)) as TreeListNode;
            if (node.RootNode == node)
                return;
            if (node != null && table != null)
            {
                List<object> itemArray = new List<object>();
                foreach (DataColumn column in table.Columns)
                {
                    itemArray.Add(node.GetValue(column));
                }
                object[] tmp = new object[itemArray.Count];
                itemArray.CopyTo(tmp);
                DataRow row = table.NewRow();
                row.ItemArray = tmp;
                table.Rows.InsertAt(row, DropTargetRowHandle);
                TreeList treeList = node.TreeList;
                treeList.DeleteNode(node);
            }
            DropTargetRowHandle = -1;
        }

        private void grid_DragOver(object sender, DragEventArgs e)
        {
            GridControl grid = (GridControl)sender;
            Point pt = new Point(e.X, e.Y);
            pt = grid.PointToClient(pt);
            GridView view = grid.GetViewAt(pt) as GridView;
            if (view == null)
                return;
            GridHitInfo hitInfo = view.CalcHitInfo(pt);
            if (hitInfo.RowHandle == GridControl.InvalidRowHandle)
                DropTargetRowHandle = view.DataRowCount;
            else
                DropTargetRowHandle = hitInfo.RowHandle;
            TreeListNode node = e.Data.GetData(typeof(TreeListNode)) as TreeListNode;
            if (DropTargetRowHandle >= 0 && e.Data.GetDataPresent(typeof(TreeListNode)) && node.RootNode != node)
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }

        private void grid_DragLeave(object sender, EventArgs e)
        {
            DropTargetRowHandle = -1;
        }

        private void grid_Paint(object sender, PaintEventArgs e)
        {
            if (DropTargetRowHandle < 0)
                return;
            GridControl grid = (GridControl)sender;
            GridView view = (GridView)grid.MainView;
            bool isBottomLine = DropTargetRowHandle == view.DataRowCount;
            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
            GridRowInfo rowInfo = viewInfo.GetGridRowInfo(isBottomLine ? DropTargetRowHandle - 1 : DropTargetRowHandle);
            if (rowInfo == null)
                return;
            Point p1, p2;
            if (isBottomLine)
            {
                p1 = new Point(rowInfo.Bounds.Left, rowInfo.Bounds.Bottom - 1);
                p2 = new Point(rowInfo.Bounds.Right, rowInfo.Bounds.Bottom - 1);
            }
            else
            {
                p1 = new Point(rowInfo.Bounds.Left, rowInfo.Bounds.Top - 1);
                p2 = new Point(rowInfo.Bounds.Right, rowInfo.Bounds.Top - 1);
            }
            e.Graphics.DrawLine(Pens.Blue, p1, p2);
        }

        void grid_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (_dragStartHitInfo != null)
            {
                e.UseDefaultCursors = false;
                Cursor.Current = _dragRowCursor;
            }
        }
    }
}
