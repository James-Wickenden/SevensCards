Public Class DebugGameModel
    Private view As DebugGameView
    Private turn As Integer = 1
    Private players(3) As Player
    Private board As New Board
    Private finishers As Integer = 0

    'TODO:
    '   Make players a class with human and COM inheritors
    '   Implement networking and refactor for decentralised hands
    '   Move view into dedicated thread!

    Public Sub New(view As DebugGameView)
        Me.view = view

        GameSetup()
        Me.view.DrawView(board, GetHands, turn)
        GameLoop()
    End Sub

    Private Sub GameSetup()
        Dim deck As New Deck()
        deck.ShuffleDeck()

        For i As Integer = 0 To 3
            players(i) = New Player_HUM()
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
        Do
            Dim card As Card = players(turn).GetMove
            If card IsNot Nothing Then
                Move(card)
                players(turn).SetPlayedCard(Nothing)
            Else
                Skip()
            End If
        Loop Until finishers = 4
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
        view.ChangePlayer(players(turn).GetHandCards, players(newTurn).GetHandCards, newTurn)
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

    Private Sub Move(card As Card)
        Dim newTurn As Integer = GetNextPlayer()
        If Not card.GetValid Then Exit Sub
        board.GetSuit(card.GetSuit).AddCard(card)

        view.RemoveCardFromHand(players(turn).GetHandCards, card)
        view.DrawCardOnBoard(card)
        view.ChangePlayer(players(turn).GetHandCards, players(newTurn).GetHandCards, newTurn)
        players(turn).GetHand.RemoveCard(card)

        If players(turn).GetHandCards.Count = 0 Then
            finishers += 1
            view.Finisher(finishers, turn)
        End If
        UpdateValidCards(card)
        turn = newTurn
    End Sub
End Class
