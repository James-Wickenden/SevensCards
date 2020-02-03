Public Class DebugGameModel
    Private debugGameView As DebugGameView
    Private turn As Integer = 1
    Private players(3) As Player
    Private board As New Board
    Private finishers As Integer = 0
    Private moveThread As Threading.Thread
    'TODO:
    '   Make players a class with human and COM inheritors
    '   Implement networking and refactor for decentralised hands
    '   Move view into dedicated thread!

    Public Sub New(menu As Menu)
        debugGameView = New DebugGameView()
        debugGameView.SetDebugGameModel(Me)
        debugGameView.Show()
        menu.Close()

        GameSetup()
        debugGameView.DrawView(board, GetHands, turn)
        GameLoop()
    End Sub

    Private Sub GameSetup()
        Dim deck As New Deck()
        deck.ShuffleDeck()

        For i As Integer = 0 To 3
            players(i) = New Player_HUM()
            players(i).setCallback(AddressOf ResultCallback)
            Dim playerHand As New List(Of Card)
            For j As Integer = 0 To 11
                playerHand.Add(deck.GetCard((12 * i) + j))
            Next
            players(i).SetHand(New Hand(playerHand.ToArray))
            players(i).GetHand.SortHand()
        Next
        Randomize()
        turn = Int((4) * Rnd())
    End Sub

    Private Sub GameLoop()
        moveThread = New System.Threading.Thread(AddressOf players(turn).GetMove)
        moveThread.Start()

    End Sub

    Private Function GetCorrespondingCard(chosenCard As Card) As Card

        For i As Integer = 0 To 3
            For Each card As Card In players(i).GetHandCards
                If chosenCard.Equals(card) Then Return card
            Next
        Next
        Return Nothing
    End Function

    Public Sub ResultCallback(chosenCard As Card)

        Dim card As Card = GetCorrespondingCard(chosenCard)
        Move(card)
    End Sub

    Private Function GetHands() As Hand()
        Dim hands As New List(Of Hand)
        For i As Integer = 0 To 3
            hands.Add(players(i).GetHand)
        Next
        Return hands.ToArray
    End Function

    Private Sub UpdateValidCards(card As Card)
        If card.GetValue <> CardEnums.Value.KING And card.GetValue <> CardEnums.Value.ACE Then card.GetadjCard.SetValid(True)
    End Sub

    Public Function GetHand(index As Integer) As List(Of Card)
        Return players(index).GetHandCards
    End Function

    Public Function GetTurn() As Integer
        Return turn
    End Function

    Public Sub Skip()
        Dim newTurn As Integer = GetNextPlayer()
        debugGameView.ChangePlayer(players(turn).GetHandCards, players(newTurn).GetHandCards, newTurn)
        turn = newTurn
    End Sub

    Private Function GetNextPlayer() As Integer
        Dim x As Integer = turn
        Do
            x = (x + 1) Mod 4
        Loop Until players(x).GetHandCards.Count > 0
        Return x
    End Function

    Public Sub PlayCard(card As Card)
        players(turn).SetPlayedCard(card)
    End Sub

    Public Sub Move(card As Card)
        Dim newTurn As Integer = GetNextPlayer()
        If Not card.GetValid Then Exit Sub
        board.GetSuit(card.GetSuit).AddCard(card)

        debugGameView.RemoveCardFromHand(players(turn).GetHandCards, card)
        debugGameView.DrawCardOnBoard(card)
        debugGameView.ChangePlayer(players(turn).GetHandCards, players(newTurn).GetHandCards, newTurn)
        players(turn).GetHand.RemoveCard(card)

        If players(turn).GetHandCards.Count = 0 Then
            finishers += 1
            debugGameView.Finisher(finishers, turn)
        End If
        UpdateValidCards(card)
        turn = newTurn

    End Sub
End Class
