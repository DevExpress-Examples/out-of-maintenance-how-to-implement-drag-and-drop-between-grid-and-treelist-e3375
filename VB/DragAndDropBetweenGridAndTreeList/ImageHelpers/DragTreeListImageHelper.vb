Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Windows.Forms
Imports System.Drawing
Imports DevExpress.XtraTreeList
Imports DevExpress.XtraTreeList.Painter
Imports DevExpress.XtraTreeList.ViewInfo
Imports System.Reflection
Imports DevExpress.XtraTreeList.Handler

Namespace DragAndDropBetweenGridAndTreeList
    Public Class DragTreeListImageHelper
        Inherits TreeListPainter

        Private ReadOnly _treeList As TreeList

        Public Sub New(ByVal treeList As TreeList)
            _treeList = treeList
        End Sub

        Public Function GetDragCursor(ByVal hitInfo As TreeListHitInfo, ByVal e As Point) As Cursor
            Dim Handler As PropertyInfo = _treeList.GetType().GetProperty("Handler", BindingFlags.Instance Or BindingFlags.NonPublic)

            Dim handler_Renamed As TreeListHandler = DirectCast(Handler.GetValue(_treeList, Nothing), TreeListHandler)
            Dim fStateData As FieldInfo = handler_Renamed.GetType().GetField("fStateData", BindingFlags.Instance Or BindingFlags.NonPublic)
            Dim data As StateData = DirectCast(fStateData.GetValue(handler_Renamed), StateData)
            Dim info As TreeListViewInfo = _treeList.ViewInfo
            Dim rowInfo As RowInfo = info.GetRowInfoByPoint(hitInfo.MousePoint)
            Dim result As Bitmap = data.GetNodeDragBitmap(rowInfo.Node, rowInfo)
            Dim offset As New Point(rowInfo.Bounds.X, e.Y - rowInfo.Bounds.Y)
            Return CursorCreator.CreateCursor(result, offset)
        End Function
    End Class
End Namespace
