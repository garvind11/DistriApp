Imports System.Data.SqlClient

Public Class frmOrderEntry
    Dim cust_ID As Integer
    Dim Item_code As Integer
    Dim kgprice As Decimal
    Dim PerPackprice As Decimal

    Private Sub frmOrderEntry_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If ReadServerID() = False Then
            Application.Exit()
        ElseIf SQLConnection() = False Then
            Application.Exit()
        Else

            cmd.CommandText = "select Max  (OrderNo) from tblOrderHead "
            lblOrderNo.Text = Val(cmd.ExecuteScalar()) + 1

            LoadComboBoxWithCode(cmbCustomerName, "select CustName  from tblCustMaster ")
            LoadComboBoxWithCode(cmbItem, "select IT_NM  from tblItemMaster ")
            cmbCustomerName.Focus()

        End If


    End Sub

    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        Application.Exit()
    End Sub

    Private Sub cmbCustomerName_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbCustomerName.SelectedIndexChanged
        If cmbCustomerName.Text <> "" Then
            cmd.CommandText = "select CustCode   from tblCustMaster where CustName ='" & cmbCustomerName.Text & "' "
            cust_ID = Val(cmd.ExecuteScalar)
            cmbItem.Focus()
        Else
            MsgBox("Please select the Customer Name.")
            cmbCustomerName.Focus()
        End If
    End Sub

    Private Sub cmbItem_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbItem.SelectedIndexChanged
        If cmbItem.Text <> "" Then

            Dim dTable As New Data.DataTable
            dTable.Clear()
            Dim dAdpt As New SqlDataAdapter("select *  from tblItemMaster  where IT_NM ='" & cmbItem.Text & "'", con)
            dAdpt.Fill(dTable)
            If dTable.Rows.Count > 0 Then
                Item_code = dTable.Rows(0).Item("IT_CODE").ToString
                PerPackprice = Val(dTable.Rows(0).Item("RatePerPC"))
                kgprice = Val(dTable.Rows(0).Item("RatePerKg"))
                cmbKGPics.Focus()
            End If
            dTable.Clear()
        Else
            MsgBox("Please select the Item (Material).")
            cmbItem.Focus()
        End If

    End Sub

    Private Sub txtQty_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtQty.KeyPress
        If Not Char.IsNumber(e.KeyChar) AndAlso Not e.KeyChar = "." AndAlso Not Char.IsControl(e.KeyChar) Then
            e.KeyChar = ""
        End If
    End Sub

    Private Sub txtQty_TextChanged(sender As Object, e As EventArgs) Handles txtQty.TextChanged
        If cmbItem.Text <> "" Then
            If cmbKGPics.Text = "KG" Then
                lblTotal.Text = Val(txtQty.Text) * kgprice

            ElseIf cmbKGPics.Text = "PerPack" Then
                lblTotal.Text = Val(txtQty.Text) * PerPackprice
            End If
        Else
            MsgBox("Please select the Item (Material)")
            cmbItem.Focus()
        End If
    End Sub

    Private Sub cmbKGPics_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbKGPics.SelectedIndexChanged
        txtQty.Text = 0
        lblTotal.Text = 0
        txtQty.Focus()
    End Sub

    Private Sub btnPlace_Click(sender As Object, e As EventArgs) Handles btnPlace.Click
        If cmbCustomerName.Text = "" Then
            MsgBox("Please select the Customer Name.")
            cmbCustomerName.Focus()
            Exit Sub
        ElseIf cmbItem.Text = "" Then
            MsgBox("Please select the Item (Material). ")
            cmbItem.Focus()
            Exit Sub
        ElseIf Replicationcheck(" select * from tblOrderHead where OrderNo = " & lblOrderNo.Text & " and Status = 1") = True Then
            MsgBox("Order Already Placed, you can not add another Item.")
            Exit Sub
        ElseIf Val(lblTotal.Text) > 0 Then
            Dim pending As Double = 0
            If Replicationcheck("select * from tblOrderHead where OrderNo = " & lblOrderNo.Text & "") = False Then
                cmd.CommandText = "insert into tblOrderHead values(" & lblOrderNo.Text & "," & Item_code & "," & Val(lblTotal.Text) & ",0,getdate(),getdate(),'',0)"
                cmd.ExecuteNonQuery()

                cmd.CommandText = "insert into tblOrderSub values(" & lblOrderNo.Text & "," & Item_code & "," & Val(txtQty.Text) & "," & Val(lblTotal.Text) & ")"
                cmd.ExecuteNonQuery()

                MsgBox("Order details Saved.")

            Else
                cmd.CommandText = "update tblOrderHead set GrossAmount = GrossAmount + " & Val(lblTotal.Text) & " where OrderNo = " & lblOrderNo.Text & " and Status = 0"
                cmd.ExecuteNonQuery()

                cmd.CommandText = "insert into tblOrderSub values(" & lblOrderNo.Text & "," & Item_code & "," & Val(txtQty.Text) & "," & Val(lblTotal.Text) & ")"
                cmd.ExecuteNonQuery()
                MsgBox("Order details Saved.")
            End If
            lblTotal.Text = 0
            txtQty.Text = 0
            txtRemark.Text = ""
            LoadComboBoxWithCode(cmbItem, "select IT_NM  from tblItemMaster ")
            cmbItem.Focus()

        End If
    End Sub

    Private Sub txtAdvanceM_KeyPress(sender As Object, e As KeyPressEventArgs)
        If Not Char.IsNumber(e.KeyChar) AndAlso Not e.KeyChar = "." AndAlso Not Char.IsControl(e.KeyChar) Then
            e.KeyChar = ""
        End If
    End Sub
    Private Sub btnOrderPlace_Click(sender As Object, e As EventArgs) Handles btnOrderPlace.Click
        If Replicationcheck(" select * from tblOrderHead where OrderNo = " & lblOrderNo.Text & " and Status = 1") = True Then
            MsgBox("Order Already Placed, you can not add another Item.")
            Exit Sub
        Else
            cmd.CommandText = "Update tblOrderHead set status = 1 where OrderNo=" & lblOrderNo.Text & ""
            cmd.ExecuteNonQuery()
            MsgBox("Order Commited")
            cmd.CommandText = "select Max  (OrderNo) from tblOrderHead "
            lblOrderNo.Text = Val(cmd.ExecuteScalar()) + 1
            lblTotal.Text = 0
            txtQty.Text = 0
            cmd.CommandText = "select Max  (OrderNo) from tblOrderHead "
            lblOrderNo.Text = Val(cmd.ExecuteScalar()) + 1
            LoadComboBoxWithCode(cmbCustomerName, "select CustName  from tblCustMaster ")
            LoadComboBoxWithCode(cmbItem, "select IT_NM  from tblItemMaster ")
            cmbCustomerName.Text = ""
            cmbCustomerName.Focus()
        End If
    End Sub

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        lblTotal.Text = 0
        txtQty.Text = 0
        cmd.CommandText = "select Max  (OrderNo) from tblOrderHead "
        lblOrderNo.Text = Val(cmd.ExecuteScalar()) + 1
        LoadComboBoxWithCode(cmbCustomerName, "select CustName  from tblCustMaster ")
        LoadComboBoxWithCode(cmbItem, "select IT_NM  from tblItemMaster ")
        cmbCustomerName.Text = ""
        cmbCustomerName.Focus()
    End Sub
End Class
