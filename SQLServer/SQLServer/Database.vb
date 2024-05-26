Imports System.Configuration
Imports System.Data.SqlClient
Imports Microsoft.SqlServer.Server
Imports System.Data.SqlTypes
Imports System.Text.RegularExpressions
Imports System.Threading

Public Class Database
    'App.configからデータベース接続文字列の読み込みを行う
    Dim connectString As String = ConfigurationManager.ConnectionStrings("SQLServer.My.MySettings.lessonConnectionString").ConnectionString
    'SqlConnectionクラスの新しいインスタンスを初期化
    Dim connection As New SqlConnection(connectString)

    'データベース接続をするメソッド
    Public Sub DBOpen()
        If connection.State = ConnectionState.Closed Then
            connection.Open()
        Else
            Exit Sub
        End If
    End Sub

    'データベース接続を遮断するメソッド
    Public Sub DBClose()
        If connection.State = ConnectionState.Open Then
            connection.Close()
        Else
            Exit Sub
        End If
    End Sub

    '入力されたコマンドがSQL文かどうかを正規表現を使って確認する
    Public Function IsValidSql(inputText As String) As Boolean
        'NULLもしくは空文字でないかを確認
        If Not String.IsNullOrEmpty(inputText) Then
            '文章にSELECT,INSERT,UPDATE,ALTER,DELETEのワードがあるかどうかを調べる正規表現
            Dim sqlRegex As String = "\b(SELECT|INSERT|UPDATE|DELETE|ALTER|CREATE|DROP)\b"
            Try
                Dim match As MatchCollection = System.Text.RegularExpressions.Regex.Matches(inputText, sqlRegex)
                Dim matchSql As Match = match(0)

                'もし一致していたらTrueを返す
                If matchSql.Success Then
                    Return True
                End If

            Catch ex As Exception
                MessageBox.Show("SQLコマンドに必要な要素を含んでません。" & vbCr &
                                "エラー内容：" & ex.Message)
                Return False
            End Try
        Else
            Return False
        End If

        Return False
    End Function

    'コマンド（SELECT以外）を実行するメソッド
    Public Function command(cmdText As String) As Integer
        '引数を使ってSqlCommandクラスのインスタンスを生成
        Dim commandRun As New SqlCommand(cmdText, connection)
        'ステートメントを実行し、変更した行数を変数に格納
        Dim returnRows As Integer = commandRun.ExecuteNonQuery()
        Return returnRows
    End Function

    'コマンド（SELECT）を実行するメソッド
    Public Function command1(cmdText As String)
        '文字列の配列を格納するリスト
        Dim result As New List(Of String())

        Using command As New SqlCommand(cmdText, connection)

            Using reader As SqlDataReader = command.ExecuteReader()
                While reader.Read()
                    Dim values(reader.FieldCount - 1) As String
                    For i = 0 To reader.FieldCount - 1
                        values(i) = reader(i).ToString()
                    Next
                    result.Add(values)
                End While
            End Using

        End Using
        Return result
    End Function

    'リソースを解放するメソッド
    Public Sub DBRelease()
        connection.Dispose()
    End Sub
End Class
