using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraTreeList;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using DevExpress.XtraTreeList.Nodes;

namespace DragAndDropBetweenGridAndTreeList
{
    class TreeListManager
    {
        Cursor _dragRowCursor;
        TreeListHitInfo _dragStartHitInfo;
        readonly DragTreeListImageHelper _imageHelper;

        public TreeListManager(TreeList treeList)
        {
            SetUpTreeList(treeList);
            _imageHelper = new DragTreeListImageHelper(treeList);
        }

        public void SetUpTreeList(TreeList treeList)
        {
            treeList.AllowDrop = true;
            treeList.OptionsBehavior.DragNodes = true;
            treeList.DragOver += treeList_DragOver;
            treeList.DragDrop += treeList_DragDrop;
            treeList.OptionsBehavior.Editable = false;
            treeList.MouseMove += treeList_MouseMove;
            treeList.MouseDown += treeList_MouseDown;
            treeList.GiveFeedback += treeList_GiveFeedback;
        }

        void treeList_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (_dragStartHitInfo != null)
            {
                e.UseDefaultCursors = false;
                Cursor.Current = _dragRowCursor;
            }
        }

        private void treeList_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _dragStartHitInfo != null && _dragStartHitInfo.HitInfoType == HitInfoType.Cell)
            {
                Size dragSize = SystemInformation.DragSize;
                Rectangle dragRect = new Rectangle(new Point(_dragStartHitInfo.MousePoint.X - dragSize.Width / 2, _dragStartHitInfo.MousePoint.Y - dragSize.Height / 2), dragSize);

                if (!dragRect.Contains(new Point(e.X, e.Y)))
                {
                    _dragRowCursor = _imageHelper.GetDragCursor(_dragStartHitInfo, e.Location);
                    TreeListNode dragObject = _dragStartHitInfo.Node;
                    (sender as TreeList).DoDragDrop(dragObject, DragDropEffects.Move);
                    _dragStartHitInfo = null;
                    DevExpress.Utils.DXMouseEventArgs.GetMouseArgs(e).Handled = true;
                }
            }
        }

        private void treeList_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && Control.ModifierKeys == Keys.None)
                _dragStartHitInfo = (sender as TreeList).CalcHitInfo(new Point(e.X, e.Y));
            else
                _dragStartHitInfo = null;
        }

        private void treeList_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DataRow)))
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }

        private void treeList_DragDrop(object sender, DragEventArgs e)
        {
            TreeList treeList = sender as TreeList;
            DataRow row = e.Data.GetData(typeof(DataRow)) as DataRow;
            if (row != null)
            {
                TreeListHitInfo hitInfo = treeList.CalcHitInfo(treeList.PointToClient(new Point(e.X, e.Y)));
                if (hitInfo.HitInfoType != HitInfoType.Cell)
                    return;
                treeList.AppendNode(row, hitInfo.Node.RootNode);
                row.Delete();
            }
        }
    }
}
