Imports System.IO
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
    Private AI_difficulty As Integer = 2
    Private wc As WebController

    Public Sub New(menu As Form, mode As Integer, Optional wc As WebController = Nothing)
        Me.mode = mode
        gameView = New GameView()
        gameView.SetGameModel(Me)
        gameView.Show()
        menu.Close()

        Me.wc = wc
        If Not Me.mode = FunctionPool.Mode.ONLINE Then
            LocalGameSetup()
        Else
            OnlineGameSetup()
        End If
        gameView.DrawView(board, players, turn, mode)
        GameLoop()
    End Sub

    Private Sub LocalGameSetup()
        Dim deck As New Deck()
        deck.ShuffleDeck()

        For i As Integer = 0 To 3
            Select Case mode
                Case FunctionPool.Mode.OFFLINE
                    If i = 0 Then players(i) = New Player_HUM
                    If i > 0 Then players(i) = New Player_COM(AI_difficulty)
                Case FunctionPool.Mode.COM
                    players(i) = New Player_COM(AI_difficulty)
                    players(i).SetCanSeeHand(True)
                    gameView.KillSkip()
                Case FunctionPool.Mode.HUM
                    players(i) = New Player_HUM
            End Select

            players(i).SetCallback(AddressOf ResultCallback)
            Dim playerHand As New List(Of Card)
            For j As Integer = 0 To 11
                playerHand.Add(deck.GetCard((12 * i) + j))
            Next
            players(i).SetHand(New Hand(playerHand.ToArray))
            players(i).GetHand.SortHand()
        Next
        'Randomize()
        turn = Int((4) * Rnd())

        Dim gameStr As String = "Started new game: "
        Select Case mode
            Case FunctionPool.Mode.OFFLINE : gameStr &= "OFFLINE"
            Case FunctionPool.Mode.COM : gameStr &= "COMS"
            Case FunctionPool.Mode.HUM : gameStr &= "HUMANS"
            Case FunctionPool.Mode.ONLINE : gameStr &= "ONLINE"
        End Select
        If mode = FunctionPool.Mode.COM Or mode = FunctionPool.Mode.OFFLINE Then gameStr &= vbCrLf & " -AI Difficulty is set to " & AI_difficulty
        gameStr &= vbCrLf & " -Player " & turn & " begins." & vbCrLf

        If mode = FunctionPool.Mode.ONLINE Then
            Dim hostName = Dns.GetHostName()
            Dim addresses As IPAddress() = Dns.GetHostEntry(hostName).AddressList()
            For Each hostAdr As IPAddress In addresses
                gameStr &= vbCrLf & "Name: " & hostName & " IP Address: " & hostAdr.ToString()
            Next
        End If

        gameView.WriteToLog(gameStr)
    End Sub

    Private Sub OnlineGameSetup()
        If wc.GetIsClient Then
            While Not wc.GetClientGameReady
                Threading.Thread.Sleep(1000)
                gameView.WriteToLog("Waiting for the server to set up the game...")
            End While
        Else
            SetupServerGame()
        End If
    End Sub

    Private Sub SetupServerGame()
        Dim deck As New Deck()
        deck.ShuffleDeck()

        For i As Integer = 0 To 3
            players(i) = New Player_COM(AI_difficulty)
            players(i).SetCanSeeHand(True)
            gameView.KillSkip()

            players(i).SetCallback(AddressOf ResultCallback)

            Dim playerHand As New List(Of Card)
            For j As Integer = 0 To 11
                playerHand.Add(deck.GetCard((12 * i) + j))
            Next
            players(i).SetHand(New Hand(playerHand.ToArray))
            players(i).GetHand.SortHand()
        Next

        'Randomize()
        turn = Int((4) * Rnd())
        wc.SendToClients("BOARD:test")
    End Sub

    Private Sub GameLoop()
        moveThread = New Thread(AddressOf players(turn).GetMove)
        moveThread.Start()
        players(turn).SetIsMyMove(True)
    End Sub

    Public Sub GameClose()

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

        Dim moveStr As String = ""
        Dim finisherStr As String = ""
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
            finisherStr = "   Player " & turn & " finishes in position " & finishers & "!"
            If finishers = 4 Then finisherStr &= vbCrLf & vbCrLf & "    GAME OVER!"
            gameView.Finisher(finishers, turn)
        End If

        Dim cardStr As String = "PASS"
        If card IsNot Nothing Then cardStr = card.GetCardText
        moveStr = "Player " & turn & " plays " & cardStr
        If turn >= newTurn Then moveStr &= vbCrLf

        gameView.WriteToLog(moveStr)
        If finisherStr <> "" Then gameView.WriteToLog(finisherStr)

        turn = newTurn

        If finishers < 4 Then GameLoop()
    End Sub
End Class
