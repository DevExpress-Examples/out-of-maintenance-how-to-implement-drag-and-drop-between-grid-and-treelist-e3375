Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Linq
Imports System.Windows.Forms

Namespace DragAndDropBetweenGridAndTreeList
    Partial Public Class frmMain
        Inherits Form

        Public Sub New()
            InitializeComponent()
            InitData()
            Dim tempVar As New GridManager(gridControl1)
            Dim tempVar2 As New TreeListManager(treeList1)
        End Sub

        Private Sub InitData()
            Dim table As New DataTable()
            table.Columns.Add("Name")
            table.Columns.Add("Address")
            For i As Integer = 0 To 9
                table.Rows.Add(New Object() { "Customer" & i, "Address" & i })
            Next i
            gridControl1.DataSource = table
        End Sub
    End Class
End Namespace
