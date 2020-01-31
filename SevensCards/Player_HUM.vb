Public Class Player_HUM
    Inherits Player

    Private Async Function GetPlayedCard_HUM() As Task(Of Card)
        While playedCard Is Nothing
            Await (Task.Delay(100))
        End While
        Return playedCard
    End Function

    Public Overrides Function GetMove() As Card
        For Each card As Card In hand.GetHand
            If card.GetValid Then Return card
        Next
        Return Nothing
    End Function
End Class
