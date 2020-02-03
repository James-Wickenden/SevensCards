Public MustInherit Class Player
    Protected playedCard As Card = Nothing
    Protected hand As Hand
    Protected view As DebugGameView
    Protected callback As FunctionPool.moveToBeMade

    Public MustOverride Sub GetMove()

    Public Function GetHand() As Hand
        Return hand
    End Function
    Public Sub SetHand(hand As Hand)
        Me.hand = hand
    End Sub

    Public Sub setCallback(callbackDelegate As FunctionPool.moveToBeMade)
        callback = callbackDelegate
    End Sub
    Public Sub SetPlayedCard(playedCard As Card)
        Me.playedCard = playedCard
    End Sub

    Public Function GetHandCards() As List(Of Card)
        Return hand.GetHand
    End Function
End Class
