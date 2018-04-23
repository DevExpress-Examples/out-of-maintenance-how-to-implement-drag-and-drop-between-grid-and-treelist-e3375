Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Windows.Forms
Imports DevExpress.XtraGrid.Views.Grid.ViewInfo
Imports System.Drawing
Imports DevExpress.XtraGrid.Views.Grid.Drawing
Imports DevExpress.Utils.Drawing

Namespace DragAndDropBetweenGridAndTreeList
	Public Class DragGridImageHelper
		Inherits GridPainter
		Private ReadOnly _view As DevExpress.XtraGrid.Views.Grid.GridView

		Public Sub New(ByVal view As DevExpress.XtraGrid.Views.Grid.GridView)
			MyBase.New(view)
			_view = view
		End Sub

		Public Function GetDragCursor(ByVal rowHandle As Integer, ByVal e As Point) As Cursor
			Dim info As GridViewInfo = TryCast(_view.GetViewInfo(), GridViewInfo)
			Dim rowInfo As GridRowInfo = info.GetGridRowInfo(rowHandle)
			Dim result As Bitmap = GetRowDragBitmap(rowHandle)
			Dim offset As New Point(rowInfo.Bounds.X, e.Y - rowInfo.Bounds.Y)
			Return CursorCreator.CreateCursor(result, offset)
		End Function

		Public Function GetRowDragBitmap(ByVal rowHandle As Integer) As Bitmap
			Dim bmpView As Bitmap = Nothing
			Dim bmpRow As Bitmap = Nothing
			Dim info As GridViewInfo = TryCast(_view.GetViewInfo(), GridViewInfo)
			Dim totalBounds As Rectangle = info.Bounds
			Dim ri As GridRowInfo = info.GetGridRowInfo(rowHandle)
			Dim imageBounds As New Rectangle(New Point(0, 0), ri.Bounds.Size)
			Try
				bmpView = New Bitmap(totalBounds.Width, totalBounds.Height)
				Using gView As Graphics = Graphics.FromImage(bmpView)
					Using grView As XtraBufferedGraphics = XtraBufferedGraphicsManager.Current.Allocate(gView, New Rectangle(Point.Empty, bmpView.Size))
						Dim color As Color = If(ri.Appearance.BackColor = Color.Transparent, Color.White, ri.Appearance.BackColor)
						grView.Graphics.Clear(color)
						Dim args As New GridViewDrawArgs(New GraphicsCache(grView.Graphics), info, totalBounds)
						DrawRow(args, ri)
						grView.Graphics.FillRectangle(args.Cache.GetSolidBrush(Color.Transparent), ri.Bounds)
						grView.Render()
						bmpRow = New Bitmap(ri.Bounds.Width, ri.Bounds.Height)
						Using gRow As Graphics = Graphics.FromImage(bmpRow)
							Using grRow As XtraBufferedGraphics = XtraBufferedGraphicsManager.Current.Allocate(gRow, New Rectangle(Point.Empty, bmpRow.Size))
								grRow.Graphics.Clear(color)
								grRow.Graphics.DrawImage(bmpView, imageBounds, ri.Bounds, GraphicsUnit.Pixel)
								grRow.Render()
							End Using
						End Using
					End Using
				End Using
			Catch
			End Try
			Return bmpRow
		End Function
	End Class
End Namespace
