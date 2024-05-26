Imports System.Configuration
Imports System.Data.SqlClient
Imports System.Runtime.Remoting.Messaging
Imports System.Text.RegularExpressions
Imports System.Web.UI

Public Class Form1
    'DatabaseConnectクラスのインスタンスを生成
    Dim dbCon As New Database
    '---------------------------------------------------------------------------------------------------------
    '「データベース接続ボタン」を押した時
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            dbCon.DBOpen()
            Button1.Enabled = False
            Button1.BackColor = Color.Green
            Button2.Enabled = True
            Button2.BackColor = SystemColors.Control
            MessageBox.Show("接続成功")
        Catch ex As Exception
            MessageBox.Show("正常に接続出来ませんでした。" & vbCr &
                            "エラー内容：" & ex.Message & vbCrLf)
        End Try
    End Sub

    '---------------------------------------------------------------------------------------------------------
    '「データベース切断ボタン」を押した時
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Try
            dbCon.DBClose()
            Button2.Enabled = False
            Button1.Enabled = True
            Button1.BackColor = SystemColors.Control
            MessageBox.Show("切断成功")
        Catch ex As Exception
            MessageBox.Show("正常に閉じることが出来ませんでした。" & vbCr &
                       "エラー内容：" & ex.Message & vbCrLf)
        End Try
    End Sub

    '---------------------------------------------------------------------------------------------------------
    'テキストボックスに入力された文字列をSQLコマンドとして実行
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        TextBox2.Clear()

        'テキストボックスの入力文字列を取得
        Dim inputText As String = TextBox1.Text
        Dim selectStatement As String = "\bSELECT\b"

        'IsValidSqlメソッドを実行しTrueである場合は、コマンド実行する
        If dbCon.IsValidSql(inputText) Then
            'SELECT文の場合はテキストボックスに表示する
            If Regex.IsMatch(inputText, selectStatement) Then
                'Databaseクラスのcommand1メソッドを実行した時の戻り値を取得
                Dim result As List(Of String()) = dbCon.command1(inputText)
                'ループで取得したリストの各要素をテキストボックスに表示する
                For Each row As String() In result
                    Dim rowText As String = String.Join(",", row)
                    TextBox2.AppendText(rowText & vbCrLf)
                Next
            End If
            'SELECT文以外の場合は実行して変更行を表示
            Dim rows As Integer = dbCon.command(inputText)
            MessageBox.Show(rows & "行目が変更されました。")
        Else
            MessageBox.Show("実行出来ませんでした。")
        End If

    End Sub

    '---------------------------------------------------------------------------------------------------------
    'フォームをクローズする時にリソースを解放する
    Private Sub Form1_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        dbCon.DBRelease()
    End Sub

End Class
