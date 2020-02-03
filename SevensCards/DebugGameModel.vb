Public Class DebugGameModel
    Private debugGameView As DebugGameView
    Private turn As Integer = 1
    Private players(3) As Hand
    Private board As New Board
    Private finishers As Integer = 0

    Public Sub New(debugGameView As DebugGameView)
        Me.debugGameView = debugGameView
        GameSetup()
        debugGameView.DrawView(board, GetHands, turn)
    End Sub

    Private Sub GameSetup()
        Dim deck As New Deck()
        deck.ShuffleDeck()

        For i As Integer = 0 To 3
            Dim playerHand As New List(Of Card)
            For j As Integer = 0 To 11
                playerHand.Add(deck.GetCard((12 * i) + j))
            Next
            players(i) = New Hand(playerHand.ToArray)
            players(i).SortHand()
        Next
        Randomize()
        turn = Int((4) * Rnd())
    End Sub

    Private Function GetCorrespondingCard(chosenCard As Card) As Card
        For i As Integer = 0 To 3
            For Each card As Card In players(i).GetHand
                If chosenCard.Equals(card) Then Return card
            Next
        Next
        Return Nothing
    End Function

    Private Function GetHands() As Hand()
        Dim hands As New List(Of Hand)
        For i As Integer = 0 To 3
            hands.Add(players(i))
        Next
        Return hands.ToArray
    End Function

    Private Sub UpdateValidCards(card As Card)
        If card.GetValue <> CardEnums.Value.KING And card.GetValue <> CardEnums.Value.ACE Then card.GetadjCard.SetValid(True)
    End Sub

    Public Function GetHand(index As Integer) As List(Of Card)
        Return players(index).GetHand
    End Function

    Public Function GetTurn() As Integer
        Return turn
    End Function

    Public Sub Skip()
        Dim newTurn As Integer = GetNextPlayer()
        debugGameView.ChangePlayer(players(turn).GetHand, players(newTurn).GetHand, newTurn)
        turn = newTurn
    End Sub

    Private Function GetNextPlayer() As Integer
        Dim x As Integer = turn
        Do
            x = (x + 1) Mod 4
        Loop Until players(x).GetHand.Count > 0
        Return x
    End Function

    Public Sub Move(card As Card)
        Dim newTurn As Integer = GetNextPlayer()
        If Not card.GetValid Then Exit Sub
        board.GetSuit(card.GetSuit).AddCard(card)
        debugGameView.RemoveCardFromHand(players(turn).GetHand, card)
        debugGameView.DrawCardOnBoard(card)
        debugGameView.ChangePlayer(players(turn).GetHand, players(newTurn).GetHand, newTurn)
        players(turn).RemoveCard(card)
        If players(turn).GetHand.Count = 0 Then
            finishers += 1
            debugGameView.Finisher(finishers, turn)
        End If
        UpdateValidCards(card)
        turn = newTurn
    End Sub
End Class
