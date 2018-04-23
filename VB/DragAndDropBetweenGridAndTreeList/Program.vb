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
Imports System.Windows.Forms

Namespace DragAndDropBetweenGridAndTreeList
	Friend NotInheritable Class Program
		''' <summary>
		''' The main entry point for the application.
		''' </summary>
		Private Sub New()
		End Sub
		<STAThread> _
		Shared Sub Main()
			Application.EnableVisualStyles()
			Application.SetCompatibleTextRenderingDefault(False)
			Application.Run(New frmMain())
		End Sub
	End Class
End Namespace
