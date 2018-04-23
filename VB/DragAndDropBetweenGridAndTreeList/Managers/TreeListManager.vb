' Developer Express Code Central Example:
' How to implement drag-and-drop between Grid and TreeList
' 
' This example demonstrates how to create helper classes for implementing
' drag-and-drop between Grid and TreeList. For more information please refer
' to:
' http://www.devexpress.com/scid=A2343
' http://www.devexpress.com/scid=K18056
' http://www.devexpress.com/scid=A1444
' 
' You can find sample updates and versions for different programming languages here:
' http://www.devexpress.com/example=E3375


Imports Microsoft.VisualBasic
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
		Private _dragStartHitInfo As TreeListHitInfo
		Private ReadOnly _imageHelper As DragTreeListImageHelper

		Public Sub New(ByVal treeList As TreeList)
			SetUpTreeList(treeList)
			_imageHelper = New DragTreeListImageHelper(treeList)
		End Sub

		Public Sub SetUpTreeList(ByVal treeList As TreeList)
			treeList.AllowDrop = True
			treeList.OptionsBehavior.DragNodes = True
			AddHandler treeList.DragOver, AddressOf treeList_DragOver
			AddHandler treeList.DragDrop, AddressOf treeList_DragDrop
			treeList.OptionsBehavior.Editable = False
			AddHandler treeList.MouseMove, AddressOf treeList_MouseMove
			AddHandler treeList.MouseDown, AddressOf treeList_MouseDown
			AddHandler treeList.GiveFeedback, AddressOf treeList_GiveFeedback
		End Sub

		Private Sub treeList_GiveFeedback(ByVal sender As Object, ByVal e As GiveFeedbackEventArgs)
			If _dragStartHitInfo IsNot Nothing Then
				e.UseDefaultCursors = False
				Cursor.Current = _dragRowCursor
			End If
		End Sub

		Private Sub treeList_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)
			If e.Button = MouseButtons.Left AndAlso _dragStartHitInfo IsNot Nothing AndAlso _dragStartHitInfo.HitInfoType = HitInfoType.Cell Then
				Dim dragSize As Size = SystemInformation.DragSize
				Dim dragRect As New Rectangle(New Point(_dragStartHitInfo.MousePoint.X - dragSize.Width \ 2, _dragStartHitInfo.MousePoint.Y - dragSize.Height \ 2), dragSize)

				If (Not dragRect.Contains(New Point(e.X, e.Y))) Then
					_dragRowCursor = _imageHelper.GetDragCursor(_dragStartHitInfo, e.Location)
					Dim dragObject As TreeListNode = _dragStartHitInfo.Node
					TryCast(sender, TreeList).DoDragDrop(dragObject, DragDropEffects.Move)
					_dragStartHitInfo = Nothing
					DevExpress.Utils.DXMouseEventArgs.GetMouseArgs(e).Handled = True
				End If
			End If
		End Sub

		Private Sub treeList_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs)
			If e.Button = MouseButtons.Left AndAlso Control.ModifierKeys = Keys.None Then
				_dragStartHitInfo = (TryCast(sender, TreeList)).CalcHitInfo(New Point(e.X, e.Y))
			Else
				_dragStartHitInfo = Nothing
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
