Public Class Player_HUM
    Inherits Player

    Public Event HUM_Move(card As Card)

    Public Sub New(hand As Hand)
        Me.hand = hand
    End Sub

    Private Function getMoveCard(card As Card) As Card Handles Me.HUM_Move
        Return card
    End Function

    Public Overrides Async Function GetMove() As Task(Of Card)
        Return hand.getHand(0)
    End Function
End Class
