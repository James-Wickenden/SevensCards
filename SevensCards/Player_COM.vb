Public Class Player_COM
    Inherits Player
    'The COM should prioritise playing outer cards over inner ones
    'At higher difficulties, the COM should put higher importance on cards with more extreme values 
    '   ie a player with aces in a suit should try to play towards those aces
    '   a player with only mid values in a suit should try and keep those cards spare to impede others
    'At lower difficulties, the COM should play randomly.

    Private difficulty As Integer = 0

    Public Sub SetDifficulty(difficulty As Integer)
        Me.difficulty = difficulty
    End Sub

    Public Overrides Sub GetMove()
        While Not isMyMove
        End While
        isMyMove = False
        For i As Integer = 0 To hand.GetHand.Count - 1
            If hand.GetHand(i).GetValid Then
                playedCard = Nothing
                callback(hand.GetHand(i))
            End If
        Next
        callback(Nothing)
    End Sub
End Class
