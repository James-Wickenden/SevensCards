Public MustInherit Class Player
    Protected hand As Hand

    Public MustOverride Async Function GetMove() As Task(Of Card)

    Public Function GetHand() As Hand
        Return hand
    End Function
End Class
