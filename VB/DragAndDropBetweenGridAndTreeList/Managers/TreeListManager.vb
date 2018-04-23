Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports DevExpress.XtraTreeList
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Data
Imports DevExpress.XtraTreeList.Nodes

Namespace DragAndDropBetweenGridAndTreeList
    Friend Class TreeListManager
        Private _dragRowCursor As Cursor
        Private useCustomCursor As Boolean

        Public Sub New(ByVal treeList As TreeList)
            SetUpTreeList(treeList)
        End Sub

        Public Sub SetUpTreeList(ByVal treeList As TreeList)
            treeList.AllowDrop = True
            treeList.OptionsDragAndDrop.DragNodesMode = DragNodesMode.Single
            AddHandler treeList.DragOver, AddressOf treeList_DragOver
            AddHandler treeList.DragDrop, AddressOf treeList_DragDrop
            AddHandler treeList.DragEnter, AddressOf TreeList_DragEnter
            AddHandler treeList.DragLeave, AddressOf TreeList_DragLeave
            treeList.OptionsBehavior.Editable = False
            AddHandler treeList.GiveFeedback, AddressOf treeList_GiveFeedback
        End Sub

        Private Sub TreeList_DragEnter(ByVal sender As Object, ByVal e As DragEventArgs)
            useCustomCursor = False
            Dim provider = TryCast(e.Data.GetData(GetType(IDragNodesProvider)), IDragRowsInfoProvider)
            If provider Is Nothing Then
                Return
            End If
            _dragRowCursor = CursorCreator.CreateCursor(provider.GetDragPreview(), New Point())
        End Sub

        Private Sub TreeList_DragLeave(ByVal sender As Object, ByVal e As EventArgs)
            useCustomCursor = True
        End Sub

        Private Sub treeList_GiveFeedback(ByVal sender As Object, ByVal e As GiveFeedbackEventArgs)
            If useCustomCursor Then
                e.UseDefaultCursors = False
                Cursor.Current = _dragRowCursor
            End If
        End Sub

        Private Sub treeList_DragOver(ByVal sender As Object, ByVal e As DragEventArgs)
            If e.Data.GetDataPresent(GetType(DataRow)) Then
                e.Effect = DragDropEffects.Move
            Else
                e.Effect = DragDropEffects.None
            End If
        End Sub

        Private Sub treeList_DragDrop(ByVal sender As Object, ByVal e As DragEventArgs)
            Dim treeList As TreeList = TryCast(sender, TreeList)
            Dim row As DataRow = TryCast(e.Data.GetData(GetType(DataRow)), DataRow)
            If row IsNot Nothing Then
                Dim hitInfo As TreeListHitInfo = treeList.CalcHitInfo(treeList.PointToClient(New Point(e.X, e.Y)))
                If hitInfo.HitInfoType <> HitInfoType.Cell Then
                    Return
                End If
                treeList.AppendNode(row, hitInfo.Node.RootNode)
                row.Delete()
            End If
        End Sub
    End Class
End Namespace
