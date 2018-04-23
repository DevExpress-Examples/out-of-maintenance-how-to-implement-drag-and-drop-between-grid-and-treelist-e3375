Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports System.Drawing

Namespace DragAndDropBetweenGridAndTreeList
	Friend Class CursorCreator
		<DllImport("user32.dll")> _
		Public Shared Function GetIconInfo(ByVal hIcon As IntPtr, ByRef pIconInfo As IconInfo) As <MarshalAs(UnmanagedType.Bool)> Boolean
		End Function

		<DllImport("user32.dll")> _
		Public Shared Function CreateIconIndirect(ByRef icon As IconInfo) As IntPtr
		End Function

		Public Structure IconInfo
			Public fIcon As Boolean
			Public xHotspot As Integer
			Public yHotspot As Integer
			Public hbmMask As IntPtr
			Public hbmColor As IntPtr
		End Structure

		Public Shared Function CreateCursor(ByVal bmp As Bitmap, ByVal hotspot As Point) As Cursor
			If bmp Is Nothing Then
				Return Cursors.Default
			End If
			Dim ptr As IntPtr = bmp.GetHicon()
			Dim tmp As New IconInfo()
			GetIconInfo(ptr, tmp)
			tmp.fIcon = False
			tmp.xHotspot = hotspot.X
			tmp.yHotspot = hotspot.Y
			ptr = CreateIconIndirect(tmp)
			Return New Cursor(ptr)
		End Function
	End Class
End Namespace
