Public Class Player_COM
    Inherits Player
    'Difficulty 0: The COM plays whatever valid card it comes across first.
    'Difficulty 1: The COM weighs cards towards the extremities more heavily, and plays to get rid of them above more central plays. 
    'Difficulty 2: The COM weighs cards like mode 1, but also weighs how many cards it can play towards one extremity and plays to that first.
    Private difficulty As Integer = 0

    Public Sub SetDifficulty(difficulty As Integer)
        Me.difficulty = difficulty
    End Sub

    Private Function GetPlayedCard_COM() As Card
        For Each card As Card In hand.GetHand
            If card.GetValid Then
                Threading.Thread.Sleep(1)
                Return card
            End If
        Next
        Return Nothing
    End Function

    Public Overrides Sub GetMove()
        While Not isMyMove
            If skipMove Then
                skipMove = False
                callback(Nothing)
            End If
        End While
        Dim card As Card = GetPlayedCard_COM()
        callback(card)
    End Sub
End Class
