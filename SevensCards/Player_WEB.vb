Imports System.Net

Public Class Player_WEB
    Inherits Player
    Private LOC_Addr As IPAddress = IPAddress.Parse("127.0.0.1")
    Private WEB_Addr As String = "127.0.0.1"
    Private listener As New Sockets.TcpListener(LOC_Addr, 65535)
    Private client As Sockets.TcpClient

    Public Sub New()
        listener.Start()
    End Sub

    Private Function GetPlayedCard_WEB() As Card
        Dim message As String = ""
        Dim cardReceived As Boolean = False
        Do
            If listener.Pending = True Then
                message = ""
                client = listener.AcceptTcpClient()

                Dim Reader As New IO.StreamReader(client.GetStream())

                While Reader.Peek > -1
                    message &= Convert.ToChar(Reader.Read()).ToString
                End While

                MsgBox(message, MsgBoxStyle.OkOnly)

                cardReceived = True
            End If
        Loop Until cardReceived

        For Each card As Card In hand.GetHand
            If card.GetValid Then Return card
        Next
        Return Nothing
    End Function

    Public Overrides Sub GetMove()
        Dim card As Card = GetPlayedCard_WEB()
        callback(card)
    End Sub

    Public Overrides Sub KillListener()
        listener.Stop()
    End Sub

    Public Overrides Sub SendMove(card As Card, turn As Integer)
        client = New Sockets.TcpClient(WEB_Addr, 65535)

        Dim Writer As New IO.StreamWriter(client.GetStream())
        Writer.Write(turn & " " & card.GetCardText)
        Writer.Flush()
    End Sub
End Class
