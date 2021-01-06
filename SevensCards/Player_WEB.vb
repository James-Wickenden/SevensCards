Imports System.Net

Public Class Player_WEB
    Inherits Player

    Private readyMoves As Queue

    Public Sub New(readyMoves As Queue)
        isWeb = True
        Me.readyMoves = readyMoves
    End Sub

    'Public Sub SetReadyMoves(readyMoves As Queue)

    'End Sub

    Private Function GetPlayedCard_WEB() As Card
        While readyMoves.Count = 0
            Threading.Thread.Sleep(300)
        End While

        Dim moveStr As String = readyMoves.Dequeue

        If moveStr = "SKIP" Then Return Nothing
        For Each card As Card In hand.GetHand
            If card.GetSuit = moveStr.Split("_")(0) And card.GetValue = moveStr.Split("_")(1) Then
                If card.GetValid Then Return card
            End If
        Next

        Return Nothing
    End Function

    'Public Sub RecieveWebMove(q As Queue)
    '    While q.Count = 0
    '        Threading.Thread.Sleep(300)
    '    End While
    '    Dim moveStr As String = q.Dequeue

    '    If Not moveStr = "SKIP" Then
    '        Dim tmpCard As New Card(moveStr.Split("_")(0), moveStr.Split("_")(1), False)
    '        webMoveCard = tmpCard
    '    End If
    '    readyToPlay = True
    'End Sub

    Public Overrides Sub GetMove()
        Dim card As Card = GetPlayedCard_WEB()
        callback(card)
    End Sub

End Class
