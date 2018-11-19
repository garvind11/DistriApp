Imports System
Imports System.Data.SqlClient

Module Connections
    Public CompName As String = "Godrej"
    Public MyVersion As String = "V20150922"
    Public ScannerName As String = "NAME NOT FOUND"
    Public LoginEmpname As String = ""
    Public ServerIP As String = ""
    Public DBname As String = ""
    Public DBUserID As String = ""
    Public DBPassword As String = ""
    Public LoginUserPlanCode As String = ""
    Public port As String = ""
    Public ScannerIP As String = ""
    Public ScannerTimer As Integer = 2000
    Public con As New SqlConnection
    Public cmd As New SqlCommand
    Public cmd1 As New SqlCommand
    Public cmd2 As New SqlCommand
    Public ComboDR As SqlDataReader
    Public dr As SqlDataReader
    Public MyTran As SqlTransaction
    Public Function SQLConnection() As Boolean
        Try
            If con.State = Data.ConnectionState.Open Then
                con.Close()
            End If
            con.ConnectionString = "Data Source=" & ServerIP & ";Initial Catalog=" & DBname & ";User Id=" & DBUserID & ";PWD=" & DBPassword
            con.Open()
            cmd.Connection = con
            cmd1.Connection = con
            cmd2.Connection = con
            SQLConnection = True
        Catch ex As Exception
            MsgBox(ex.Message)
            SQLConnection = False
        End Try
    End Function
    Public Sub LoadComboBoxWithCode(ByVal Combo As ComboBox, ByVal SQLQuery As String)
        cmd.CommandText = SQLQuery
        ComboDR = cmd.ExecuteReader
        Combo.Items.Clear()
        While ComboDR.Read
            Combo.Items.Add(ComboDR(0))
        End While
        ComboDR.Close()
    End Sub
    Public Function SQLRecConnection() As Boolean
        Try
            If con.State = Data.ConnectionState.Closed Then
                con.ConnectionString = "Data Source=" & ServerIP & ";Initial Catalog=" & DBname & ";User Id=" & DBUserID & ";PWD=" & DBPassword
                con.Open()
                cmd.Connection = con
                SQLRecConnection = True
            Else
                SQLRecConnection = True
            End If
        Catch ex As Exception
            MsgBox("Check Internet Connection or SQL Setup Files...")
            SQLRecConnection = False
        End Try
    End Function
    Public Function ReadServerID() As Boolean
        ReadServerID = False
        Dim path As String = Application.StartupPath
        path = path & "\Server.txt"
        If IO.File.Exists(path) = False Then
            WarningMessage("Server.txt File Missing")
            Exit Function
        End If
        Dim myreader As System.IO.StreamReader
        myreader = New System.IO.StreamReader(path)
        Dim mindex As Integer = 0
        While myreader.Peek <> -1
            mindex = mindex + 1
            If mindex = 1 Then
                ServerIP = myreader.ReadLine.Trim
            ElseIf mindex = 2 Then
                DBname = myreader.ReadLine.Trim
            ElseIf mindex = 3 Then
                DBUserID = myreader.ReadLine.Trim
            ElseIf mindex = 4 Then
                DBPassword = myreader.ReadLine.Trim
           
           
            Else
                Exit While
            End If
        End While
        myreader.Close()

        If ServerIP = "" Then
            WarningMessage("Invalid IP Address")
        ElseIf DBname = "" Then
            WarningMessage("Invalid DataBase Name")
        ElseIf DBUserID = "" Then
            WarningMessage("Invalid DB User ID")
        ElseIf DBPassword = "" Then
            WarningMessage("Invalid DB Password")
 
   
        Else
            ReadServerID = True
        End If
    End Function
    Public Sub numtextbox(ByVal cntr As TextBox, ByVal e As KeyPressEventArgs, Optional ByVal dotacceptable As Boolean = True, Optional ByVal ReadOnlyText As Boolean = False)
        If ReadOnlyText = False Then
            If e.KeyChar = Chr(Keys.Back) Then

                Exit Sub
            End If
            If e.KeyChar = "." Then
                If dotacceptable = False Then
                    e.Handled = True
                Else
                    If cntr.Text.IndexOf(".") = -1 Then
                        e.Handled = False
                        Exit Sub
                    Else
                        e.Handled = True
                    End If
                End If
            End If
            If Not Char.IsDigit(e.KeyChar) Then
                e.Handled = True
            Else
                e.Handled = False
            End If
        Else
            e.Handled = True
        End If
    End Sub
    Public Function Replicationcheck(ByVal SQLQuery As String) As Boolean
        cmd.CommandText = SQLQuery
        dr = cmd.ExecuteReader
        If dr.Read() = False Then
            Replicationcheck = False
        Else
            Replicationcheck = True
        End If
        dr.Close()
        Return Replicationcheck
    End Function
    Public Sub WarningMessage(ByVal Mess As String)
        MsgBox(Mess, MsgBoxStyle.Critical, CompName)
    End Sub

    Public Function TransactionCode(ByVal PrimaryKeyField As String, ByVal FinYear As Integer, ByVal TableName As String) As Double
        cmd.CommandText = "select isnull (MAX(" & PrimaryKeyField & ")," & FinYear & ") + 1 from " & TableName & " "
        cmd.Connection = con
        dr = cmd.ExecuteReader
        dr.Read()
        TransactionCode = dr(0)
        dr.Close()
        Return TransactionCode

    End Function
    Public Function MWVersion() As String
        cmd.CommandText = "select * from VERS Where status=0 "
        cmd.Connection = con
        dr = cmd.ExecuteReader
        dr.Read()
        MWVersion = dr(0)
        dr.Close()
        Return MWVersion

    End Function
    Public Function QueryExe(ByVal query As String) As Boolean
        Try
            Dim Result As Integer = 0
            Dim cmd As SqlCommand = New SqlCommand()
            cmd.CommandText = query
            cmd.CommandType = Data.CommandType.Text
            cmd.Connection = con
            Result = cmd.ExecuteNonQuery()
            If Result > 0 Then
                QueryExe = True
            Else
                QueryExe = False
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
            QueryExe = False
        End Try

    End Function


End Module
