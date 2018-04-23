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
using System.Windows.Forms;
using System.Drawing;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Painter;
using DevExpress.XtraTreeList.ViewInfo;
using System.Reflection;
using DevExpress.XtraTreeList.Handler;

namespace DragAndDropBetweenGridAndTreeList
{
    public class DragTreeListImageHelper : TreeListPainter
    {
        private readonly TreeList _treeList;

        public DragTreeListImageHelper(TreeList treeList)
        {
            _treeList = treeList;
        }

        public Cursor GetDragCursor(TreeListHitInfo hitInfo, Point e)
        {
            PropertyInfo Handler = _treeList.GetType().GetProperty("Handler", BindingFlags.Instance | BindingFlags.NonPublic);
            TreeListHandler handler = (TreeListHandler)Handler.GetValue(_treeList, null);
            FieldInfo fStateData = handler.GetType().GetField("fStateData", BindingFlags.Instance | BindingFlags.NonPublic);
            StateData data = (StateData)fStateData.GetValue(handler);
            TreeListViewInfo info = _treeList.ViewInfo;
            RowInfo rowInfo = info.GetRowInfoByPoint(hitInfo.MousePoint);
            Bitmap result = data.GetNodeDragBitmap(rowInfo.Node, rowInfo.VisibleIndex, rowInfo.IndentInfo.Bounds.Width);
            Point offset = new Point(rowInfo.Bounds.X, e.Y - rowInfo.Bounds.Y);
            return CursorCreator.CreateCursor(result, offset);
        }
    }
}
