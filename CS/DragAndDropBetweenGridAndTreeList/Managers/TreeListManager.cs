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
        bool useCustomCursor;

        public TreeListManager(TreeList treeList)
        {
            SetUpTreeList(treeList);
        }

        public void SetUpTreeList(TreeList treeList)
        {
            treeList.AllowDrop = true;
            treeList.OptionsDragAndDrop.DragNodesMode = DragNodesMode.Single;
            treeList.DragOver += treeList_DragOver;
            treeList.DragDrop += treeList_DragDrop;
            treeList.DragEnter += TreeList_DragEnter;
            treeList.DragLeave += TreeList_DragLeave;
            treeList.OptionsBehavior.Editable = false;
            treeList.GiveFeedback += treeList_GiveFeedback;
        }

        private void TreeList_DragEnter(object sender, DragEventArgs e)
        {
            useCustomCursor = false;
            var provider = e.Data.GetData(typeof(IDragNodesProvider)) as IDragRowsInfoProvider;
            if (provider == null) return;
            _dragRowCursor = CursorCreator.CreateCursor(provider.GetDragPreview(), new Point());
        }

        private void TreeList_DragLeave(object sender, EventArgs e)
        {
            useCustomCursor = true;
        }

        void treeList_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (useCustomCursor)
            {
                e.UseDefaultCursors = false;
                Cursor.Current = _dragRowCursor;
            }
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
