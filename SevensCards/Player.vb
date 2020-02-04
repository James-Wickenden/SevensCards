Public MustInherit Class Player
    Protected playedCard As Card = Nothing
    Protected hand As Hand
    Protected canSeeHand As Boolean = False

    Protected view As GameView
    Protected callback As FunctionPool.moveToBeMade
    Protected skipMove As Boolean = False
    Protected isMyMove As Boolean = False
    Public MustOverride Sub GetMove()

    Public Sub Skip()
        skipMove = True
    End Sub

    Public Sub SetIsMyMove(isMyMove As Boolean)
        Me.isMyMove = isMyMove
    End Sub

    Public Function GetCanSeeHand() As Boolean
        Return canSeeHand
    End Function
    Public Sub SetCanSeeHand(canSeeHand As Boolean)
        Me.canSeeHand = canSeeHand
    End Sub

    Public Function GetHand() As Hand
        Return hand
    End Function
    Public Sub SetHand(hand As Hand)
        Me.hand = hand
    End Sub

    Public Sub SetCallback(callbackDelegate As FunctionPool.moveToBeMade)
        callback = callbackDelegate
    End Sub
    Public Sub SetPlayedCard(playedCard As Card)
        Me.playedCard = playedCard
    End Sub

    Public Function GetHandCards() As List(Of Card)
        Return hand.GetHand
    End Function
End Class
