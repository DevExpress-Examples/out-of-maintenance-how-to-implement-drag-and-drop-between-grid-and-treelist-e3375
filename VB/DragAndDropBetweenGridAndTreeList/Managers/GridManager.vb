Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports DevExpress.XtraGrid
Imports System.Data
Imports DevExpress.XtraTreeList.Nodes
Imports System.Windows.Forms
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraGrid.Views.Grid.ViewInfo
Imports System.Drawing
Imports DevExpress.XtraTreeList

Namespace DragAndDropBetweenGridAndTreeList
	Friend Class GridManager
		Private ReadOnly _gridControl As GridControl
		Private _dragStartHitInfo As GridHitInfo
		Private _dragRowCursor As Cursor
		Private ReadOnly _imageHelper As DragGridImageHelper

		Public Sub New(ByVal grid As GridControl)
			_gridControl = grid
			SetUpGrid(grid)
			_imageHelper = New DragGridImageHelper(TryCast(grid.FocusedView, GridView))
		End Sub

		Public Sub SetUpGrid(ByVal grid As GridControl)
			grid.AllowDrop = True
			AddHandler grid.DragOver, AddressOf grid_DragOver
			AddHandler grid.DragDrop, AddressOf grid_DragDrop
			AddHandler grid.DragLeave, AddressOf grid_DragLeave
			AddHandler grid.Paint, AddressOf grid_Paint
			AddHandler grid.GiveFeedback, AddressOf grid_GiveFeedback
			Dim view As GridView = TryCast(grid.MainView, GridView)
			view.OptionsBehavior.Editable = False
			AddHandler view.MouseMove, AddressOf view_MouseMove
			AddHandler view.MouseDown, AddressOf view_MouseDown
		End Sub

		Private Sub view_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs)
			Dim view As GridView = TryCast(sender, GridView)
			_dragStartHitInfo = Nothing
			Dim hitInfo As GridHitInfo = view.CalcHitInfo(New Point(e.X, e.Y))
			If Control.ModifierKeys <> Keys.None Then
				Return
			End If
			If e.Button = MouseButtons.Left AndAlso hitInfo.RowHandle >= 0 Then
				_dragStartHitInfo = hitInfo
			End If
		End Sub

		Private Sub view_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)
			Dim view As GridView = TryCast(sender, GridView)
			If e.Button = MouseButtons.Left AndAlso _dragStartHitInfo IsNot Nothing Then
				Dim dragSize As Size = SystemInformation.DragSize
				Dim dragRect As New Rectangle(New Point(_dragStartHitInfo.HitPoint.X - dragSize.Width \ 2, _dragStartHitInfo.HitPoint.Y - dragSize.Height \ 2), dragSize)
				If (Not dragRect.Contains(New Point(e.X, e.Y))) Then
					_dragRowCursor = _imageHelper.GetDragCursor(_dragStartHitInfo.RowHandle, e.Location)
					Dim row As DataRow = view.GetDataRow(_dragStartHitInfo.RowHandle)
					view.GridControl.DoDragDrop(row, DragDropEffects.Move)
					_dragStartHitInfo = Nothing
					DevExpress.Utils.DXMouseEventArgs.GetMouseArgs(e).Handled = True
				End If
			End If
		End Sub

		Private dropTargetRowHandle_Renamed As Integer = -1
		Private Property DropTargetRowHandle() As Integer
			Get
				Return dropTargetRowHandle_Renamed
			End Get
			Set(ByVal value As Integer)
				dropTargetRowHandle_Renamed = value
				_gridControl.Invalidate()
			End Set
		End Property

		Private Sub grid_DragDrop(ByVal sender As Object, ByVal e As DragEventArgs)
			Dim grid As GridControl = TryCast(sender, GridControl)
			Dim table As DataTable = TryCast(grid.DataSource, DataTable)
			Dim node As TreeListNode = TryCast(e.Data.GetData(GetType(TreeListNode)), TreeListNode)
			If node.RootNode Is node Then
				Return
			End If
			If node IsNot Nothing AndAlso table IsNot Nothing Then
				Dim itemArray As New List(Of Object)()
				For Each column As DataColumn In table.Columns
					itemArray.Add(node.GetValue(column))
				Next column
				Dim tmp(itemArray.Count - 1) As Object
				itemArray.CopyTo(tmp)
				Dim row As DataRow = table.NewRow()
				row.ItemArray = tmp
				table.Rows.InsertAt(row, DropTargetRowHandle)
				Dim treeList As TreeList = node.TreeList
				treeList.DeleteNode(node)
			End If
			DropTargetRowHandle = -1
		End Sub

		Private Sub grid_DragOver(ByVal sender As Object, ByVal e As DragEventArgs)
			Dim grid As GridControl = CType(sender, GridControl)
			Dim pt As New Point(e.X, e.Y)
			pt = grid.PointToClient(pt)
			Dim view As GridView = TryCast(grid.GetViewAt(pt), GridView)
			If view Is Nothing Then
				Return
			End If
			Dim hitInfo As GridHitInfo = view.CalcHitInfo(pt)
			If hitInfo.RowHandle = GridControl.InvalidRowHandle Then
				DropTargetRowHandle = view.DataRowCount
			Else
				DropTargetRowHandle = hitInfo.RowHandle
			End If
			Dim node As TreeListNode = TryCast(e.Data.GetData(GetType(TreeListNode)), TreeListNode)
			If DropTargetRowHandle >= 0 AndAlso e.Data.GetDataPresent(GetType(TreeListNode)) AndAlso node.RootNode IsNot node Then
				e.Effect = DragDropEffects.Move
			Else
				e.Effect = DragDropEffects.None
			End If
		End Sub

		Private Sub grid_DragLeave(ByVal sender As Object, ByVal e As EventArgs)
			DropTargetRowHandle = -1
		End Sub

		Private Sub grid_Paint(ByVal sender As Object, ByVal e As PaintEventArgs)
			If DropTargetRowHandle < 0 Then
				Return
			End If
			Dim grid As GridControl = CType(sender, GridControl)
			Dim view As GridView = CType(grid.MainView, GridView)
			Dim isBottomLine As Boolean = DropTargetRowHandle = view.DataRowCount
			Dim viewInfo As GridViewInfo = TryCast(view.GetViewInfo(), GridViewInfo)
			Dim rowInfo As GridRowInfo = viewInfo.GetGridRowInfo(If(isBottomLine, DropTargetRowHandle - 1, DropTargetRowHandle))
			If rowInfo Is Nothing Then
				Return
			End If
			Dim p1, p2 As Point
			If isBottomLine Then
				p1 = New Point(rowInfo.Bounds.Left, rowInfo.Bounds.Bottom - 1)
				p2 = New Point(rowInfo.Bounds.Right, rowInfo.Bounds.Bottom - 1)
			Else
				p1 = New Point(rowInfo.Bounds.Left, rowInfo.Bounds.Top - 1)
				p2 = New Point(rowInfo.Bounds.Right, rowInfo.Bounds.Top - 1)
			End If
			e.Graphics.DrawLine(Pens.Blue, p1, p2)
		End Sub

		Private Sub grid_GiveFeedback(ByVal sender As Object, ByVal e As GiveFeedbackEventArgs)
			If _dragStartHitInfo IsNot Nothing Then
				e.UseDefaultCursors = False
				Cursor.Current = _dragRowCursor
			End If
		End Sub
	End Class
End Namespace
