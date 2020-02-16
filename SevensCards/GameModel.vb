Imports System.Net
Imports System.Threading

Public Class GameModel
    Private gameView As GameView
    Private turn As Integer = 1
    Private players(3) As Player
    Private board As New Board
    Private finishers As Integer = 0
    Private moveThread As Threading.Thread
    Private mode As FunctionPool.Mode

    Public Sub New(menu As Menu, mode As Integer)
        Me.mode = mode
        gameView = New GameView()
        gameView.SetGameModel(Me)
        gameView.Show()
        menu.Close()

        GameSetup()
        gameView.DrawView(board, players, turn, mode)
        GameLoop()
    End Sub

    Private Sub GameSetup()
        Dim deck As New Deck()
        deck.ShuffleDeck()

        For i As Integer = 0 To 3
            Select Case mode
                Case FunctionPool.Mode.OFFLINE
                    If i = 0 Then players(i) = New Player_HUM
                    If i > 0 Then players(i) = New Player_COM
                Case FunctionPool.Mode.COM
                    players(i) = New Player_COM
                    players(i).SetCanSeeHand(True)
                    gameView.KillSkip()
                Case FunctionPool.Mode.HUM
                    players(i) = New Player_HUM
                Case FunctionPool.Mode.ONLINE
                    If i = 0 Then players(i) = New Player_HUM
                    If i = 1 Then players(i) = New Player_WEB
                    If i > 1 Then players(i) = New Player_COM
            End Select
            players(i).SetCallback(AddressOf ResultCallback)
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
        moveThread = New Thread(AddressOf players(turn).GetMove)
        moveThread.Start()
        players(turn).SetIsMyMove(True)
    End Sub

    Public Sub GameClose()
        For i As Integer = 0 To 3
            players(i).KillListener()
        Next
        End
    End Sub

    Public Sub ResultCallback(card As Card)
        Move(card)
    End Sub

    Public Function GetModeString() As String
        Select Case mode
            Case FunctionPool.Mode.OFFLINE : Return "OFFLINE MATCH"
            Case FunctionPool.Mode.COM : Return "COMPUTER MATCH"
            Case FunctionPool.Mode.HUM : Return "HUMAN MATCH"
            Case Else : Return "ONLINE"
        End Select
    End Function

    Private Sub UpdateValidCards(card As Card)
        If card.GetValue <> CardEnums.Value.KING And card.GetValue <> CardEnums.Value.ACE Then card.GetAdjCard.SetValid(True)
    End Sub

    Public Function GetHand(index As Integer) As List(Of Card)
        Return players(index).GetHandCards
    End Function

    Public Function GetTurn() As Integer
        Return turn
    End Function

    Public Sub Skip()
        players(turn).Skip()
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
        Dim skipped As Boolean = False
        If card Is Nothing Then skipped = True

        Dim newTurn As Integer = GetNextPlayer()

        If Not skipped Then
            board.GetSuit(card.GetSuit).AddCard(card)
            gameView.RemoveCardFromHand(players(turn).GetHandCards, card)
            gameView.DrawCardOnBoard(card)
            players(turn).GetHand.RemoveCard(card)
            UpdateValidCards(card)
        End If

        Dim isOldHandVisible As Boolean = players(turn).GetCanSeeHand
        Dim isNewHandVisible As Boolean = players(newTurn).GetCanSeeHand
        If mode = FunctionPool.Mode.HUM Then isOldHandVisible = False

        gameView.ChangePlayer(players(turn).GetHandCards, players(newTurn).GetHandCards, newTurn, isOldHandVisible, isNewHandVisible)

        If players(turn).GetHandCards.Count = 0 Then
            finishers += 1
            gameView.Finisher(finishers, turn)
        End If

        If mode = FunctionPool.Mode.ONLINE Then
            For Each player As Player In players
                If player.GetType = GetType(Player_WEB) Then
                    player.SendMove(card, turn)
                End If
            Next
        End If

        turn = newTurn

        If finishers < 4 Then GameLoop()
    End Sub
End Class
