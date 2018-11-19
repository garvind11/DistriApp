Imports System.Reflection
Imports System.Net

Module Validation
    Public Scanned_IT_NM As String = ""
    Public Scanned_BT_NO As String = ""
    Public shudScan_IT_NM As String = ""
    Public shudScan_BT_NO As String = ""


    Public NumData As String
    Public UserData As String
    Public UserType As String = 0
    Public UserCode As String = 0
    Public aprroval As Boolean = False
    Public Approved As Boolean = False
    Public Intrchd_Item As String = ""
    Public DelLine As String = ""
    Public path As String = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)
    Public ErrorMsg As New System.Media.SoundPlayer(path & "\SysAlert.wav")
    Public SuccessMsg As New System.Media.SoundPlayer(path & "\SysPerfect.wav")

    Function Version() As String
        Dim txt As String = Assembly.GetExecutingAssembly.GetName.Version.Major.ToString() + "." + Assembly.GetExecutingAssembly.GetName.Version.Minor.ToString() + "." + Assembly.GetExecutingAssembly().GetName().Version.Build.ToString()
        If txt.Length > 0 Then
            Return txt
        Else
            Return String.Empty
        End If
    End Function
    Public Function StringTextBoxValidation(ByVal ErrMessage As String, ByVal CurrentControlName As Control, ByVal NextControlName As Control) As Boolean
        Try
            If Trim(CurrentControlName.Text) = "" Then
                MsgBox("Please Enter " & ErrMessage, vbCritical, CompName)
                CurrentControlName.Focus()
                StringTextBoxValidation = True
            Else
                NextControlName.Focus()
                StringTextBoxValidation = False
            End If
        Catch ex As Exception
            ErrorMsg.Load()
            ErrorMsg.Play()
            MsgBox(ex.Message)
            StringTextBoxValidation = True
        End Try
    End Function
    Public Function FunctionDate(ByVal formatdate As Date)
        FunctionDate = "'" & Format(CDate(formatdate), "dd-MMM-yyyy") & "'"
    End Function
    Public Function FunctionTime(ByVal formattime As Date)
        FunctionTime = "'" & TimeValue(formattime) & "'"
    End Function
    Public Function EmptyText(ByVal MessageText As String, ByVal CurrentControl As Control, ByVal NextControl As Control) As Boolean
        If (CurrentControl.Text) = "" Then
            ErrorMsg.Load()
            ErrorMsg.Play()
            MsgBox("Please Enter " & MessageText, MsgBoxStyle.Critical, CompName)
            CurrentControl.Text = ""
            CurrentControl.Focus()
            EmptyText = True
        Else
            NextControl.Focus()
            EmptyText = False
        End If
    End Function
    Public Function NumText(ByVal MessageText As String, ByVal CurrentControl As Control, ByVal NextControl As Control) As Boolean
        If Val(CurrentControl.Text) = 0 Then
            ErrorMsg.Load()
            ErrorMsg.Play()
            MsgBox("Please Enter " & MessageText, MsgBoxStyle.Critical, CompName)
            CurrentControl.Text = ""
            CurrentControl.Focus()
            NumText = True
        Else
            NextControl.Focus()
            NumText = False
        End If
    End Function
    Public Function ComboValidation(ByVal MessageText As String, ByVal CurrentControl As Control, ByVal NextControl As Control) As Boolean
        If (CurrentControl.Text) = "" Then
            ErrorMsg.Load()
            ErrorMsg.Play()
            MsgBox("Please Select " & MessageText, MsgBoxStyle.Critical, CompName)
            CurrentControl.Text = ""
            CurrentControl.Focus()
            ComboValidation = True
        Else
            NextControl.Focus()
            ComboValidation = False
        End If
    End Function
    Public Function EmptyText1(ByVal MessageText As String, ByVal CurrentControl As Control, ByVal NextControl As Control) As Boolean
        If (CurrentControl.Text) = "" Then
            MsgBox("Please Enter " & MessageText, MsgBoxStyle.Critical, CompName)
            CurrentControl.Text = ""
            CurrentControl.Focus()
            EmptyText1 = True
        Else
            NextControl.Focus()
            EmptyText1 = False
        End If
    End Function
    Public Function NumCheck(ByVal x As Integer) As Boolean
        NumData = "0123456789"
        UserData = Chr(x)
        If x = 8 Then
            NumCheck = True
        Else
            NumCheck = InStr(1, NumData, UserData, vbTextCompare)
        End If
    End Function

    Public Function CodeNOVerification(ByVal Query As String, ByVal CurrentControl As Control, ByVal Message As String) As Boolean

        Try
            Dim Table As New Data.DataTable
            Table.Clear()
            Dim Adpt As New Data.SqlClient.SqlDataAdapter(Query, con)
            Adpt.Fill(Table)
            If Table.Rows.Count = 0 Then
                CodeNOVerification = True
                MsgBox("Invalid " & Message, MsgBoxStyle.Critical, CompName)
                CurrentControl.Text = ""
                CurrentControl.Focus()
            Else
                CodeNOVerification = False
            End If
        Catch ex As Exception
            CodeNOVerification = True
            MsgBox(ex.Message)
        End Try
    End Function
    Public Function SWAPBarcode(ByVal Scanned As String, ByVal shudscan As String) As Boolean
        Dim swapped As Boolean = False
        cmd.CommandText = "Select IT_NM,BT_NO  from TRMT Where SS_CC ='" & Scanned & "'"
        dr = cmd.ExecuteReader
        Do While dr.Read
            Scanned_IT_NM = dr(0)
            Scanned_BT_NO = dr(1)
        Loop
        dr.Close()
        cmd.CommandText = "Select IT_NM,BT_NO  from TRMT Where SS_CC ='" & shudscan & "'"
        dr = cmd.ExecuteReader
        Do While dr.Read
            shudScan_IT_NM = dr(0)
            shudScan_BT_NO = dr(1)
        Loop
        dr.Close()

        cmd.CommandText = "Update TRMT Set IT_NM='" & shudScan_IT_NM & "',BT_NO='" & shudScan_BT_NO & "' where SS_CC='" & Scanned & "'"
        cmd.ExecuteNonQuery()
        cmd.CommandText = "Update TRMT Set IT_NM='" & Scanned_IT_NM & "',BT_NO='" & Scanned_BT_NO & "' where SS_CC='" & shudscan & "'"
        If cmd.ExecuteNonQuery() > 0 Then
            swapped = True
        End If
        Return swapped
    End Function
    Public Function Usercheck(ByVal UserName As String, ByVal UserPWD As String) As Boolean

        Usercheck = True
        cmd.CommandText = "select US_CD,US_NM,PL_SL from USMT Where US_NM = '" & LTrim(UserName) & "' and US_PD = '" & LTrim(UserPWD) & "'  and US_DL <>1"
        dr = cmd.ExecuteReader
        Do While dr.Read
            Usercheck = False
            'UserType = dr(0)
            UserCode = dr(0)
            LoginEmpname = dr(1)
            LoginUserPlanCode = dr(2)
        Loop
        
        If Usercheck <> False Then
            Usercheck = True
        End If
        dr.Close()
        If Usercheck = False Then
            If Replicationcheck("select * from PLMT where (PL_TY ='P' OR  PL_CD='C123')  and PL_SL=" & LoginUserPlanCode) = True Then
                UserType = "P"
            Else
                UserType = "W"
            End If
        End If
        Return Usercheck
    End Function
    'Public Function ReplicationCheck(ByVal Query As String, ByVal CurrentControl As Control) As Boolean
    '    Dim DaTable As New Data.DataTable
    '    DaTable.Clear()
    '    Dim DaAdpt As New Data.SqlClient.SqlDataAdapter(Query, con)
    '    DaAdpt.Fill(DaTable)
    '    If DaTable.Rows.Count = 0 Then
    '        ReplicationCheck = True
    '        MsgBox("Invalid ", MsgBoxStyle.Critical, Compname)
    '        CurrentControl.Text = ""
    '        CurrentControl.Focus()
    '    Else
    '        ReplicationCheck = False
    '    End If
    'End Function
    Public Function GetServerDate(ByVal DtFormat As Integer) As Integer
        Dim SDate As Date
        Dim StFormat As String = ""
        If DtFormat = 1 Then
            StFormat = "yy000000"
        ElseIf DtFormat = 2 Then
            StFormat = "yyMM0000"
        ElseIf DtFormat = 3 Then
            StFormat = "yyMMdd000"
        End If

        Dim TbTable As New Data.DataTable
        TbTable.Clear()
        Dim TbAdpt As New Data.SqlClient.SqlDataAdapter("select getdate()", con)
        TbAdpt.Fill(TbTable)
        If TbTable.Rows.Count = 0 Then
            SDate = TbTable.Rows(0).Item(0)
        End If
        GetServerDate = CInt(Format(SDate, StFormat))
    End Function

End Module
