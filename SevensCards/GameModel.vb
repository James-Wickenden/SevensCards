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
    Private AI_difficulty As Integer
    Private wc As WebController
    Private dnsModel As DNSModel

    Public Sub New(menu As Form, mode As Integer, Optional wc As WebController = Nothing, Optional dnsModel As DNSModel = Nothing, Optional difficulty As Integer = 1)
        Me.mode = mode
        gameView = New GameView()
        gameView.SetGameModel(Me)
        gameView.Show()
        menu.Close()
        AI_difficulty = difficulty

        Me.wc = wc
        Me.dnsModel = dnsModel
        If Not Me.mode = FunctionPool.Mode.ONLINE Then
            LocalGameSetup()
        Else
            dnsModel.SetGameModel(Me)
            OnlineGameSetup()
        End If
        gameView.DrawView(board, players, turn, mode)

        If Me.mode = FunctionPool.Mode.ONLINE Then
            If wc.GetIsClient Then
                wc.SendToServer("READYCLIENT:")
            Else
                While dnsModel.GetReadyClients <> wc.GetClientUsernames.Split(",").Length - 1
                    Threading.Thread.Sleep(200)
                End While
            End If
        End If

        GameLoop()
    End Sub

    Private Sub PrintGameIntro()
        Dim gameStr As String = "Started new game: "
        Select Case mode
            Case FunctionPool.Mode.OFFLINE : gameStr &= "OFFLINE"
            Case FunctionPool.Mode.COM : gameStr &= "COMS"
            Case FunctionPool.Mode.HUM : gameStr &= "HUMANS"
            Case FunctionPool.Mode.ONLINE : gameStr &= "ONLINE"
        End Select
        If Not mode = FunctionPool.Mode.HUM Then
            gameStr &= vbCrLf & " -AI Difficulty is set to " & AI_difficulty
            Dim diff_dict As New Dictionary(Of Integer, String) From {{-1, "UNDEF->MEDIUM"}, {0, "EASY"}, {1, "MEDIUM"}, {2, "HARD"}}
            gameStr &= " (" & diff_dict(AI_difficulty) & ")"
        End If

        gameStr &= vbCrLf & GetNameRef(turn) & " begins."
        If mode = FunctionPool.Mode.OFFLINE Then
            gameStr &= " You are " & GetNameRef(0) & "." & vbCrLf
        ElseIf mode = FunctionPool.Mode.ONLINE Then
            Dim myTurn As Integer = 0
            gameStr &= " You are " & dnsModel.GetUsername & vbCrLf
        Else
            gameStr &= vbCrLf
        End If

        If mode = FunctionPool.Mode.ONLINE Then
            If turn = GetMyTurnIndex() Then gameStr &= "YOU START!" & vbCrLf
        End If
            'If mode = FunctionPool.Mode.ONLINE Then
            '    Dim hostName = Dns.GetHostName()
            '    Dim addresses As IPAddress() = Dns.GetHostEntry(hostName).AddressList()
            '    For Each hostAdr As IPAddress In addresses
            '        gameStr &= vbCrLf & "Name: " & hostName & " IP Address: " & hostAdr.ToString()
            '    Next
            'End If

            gameView.WriteToLog(gameStr)
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

        PrintGameIntro()
    End Sub

    Private Sub OnlineGameSetup()
        If wc.GetIsClient Then
            Threading.ThreadPool.QueueUserWorkItem(AddressOf wc.Reconnect)
            While Not wc.GetClientGameReady
                Threading.Thread.Sleep(1000)
                gameView.WriteToLog("Waiting for the server to set up the game...")
            End While
        Else
            wc.Reconnect()
            SetupServerGame()
        End If

        PrintGameIntro()
    End Sub

    Private Sub SetupServerGame()
        Dim deck As New Deck()
        deck.ShuffleDeck()

        For i As Integer = 0 To 3
            If i = 0 Then
                players(i) = New Player_HUM
            ElseIf i < wc.GetClientUsernames.Split(",").Length Then
                players(i) = New Player_WEB(dnsModel.GetReadyMoves)
            Else
                players(i) = New Player_COM(AI_difficulty)
            End If
            players(i).SetCanSeeHand(True)

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
        'turn = 0
        Dim deckStr As String = GetDeckString(deck)
        Dim usernames As String = dnsModel.GetUsername & "," & wc.GetClientUsernames

        wc.SendToClients("GAMEINFO:" & turn & " " & deckStr & " " & usernames)
    End Sub

    Private Function GetDeckString(deck As Deck) As String
        Dim res As String = ""
        For Each card As Card In deck.GetCards()
            res &= card.GetSuit & "_" & card.GetValue & "-"
        Next
        Return res
    End Function

    Private Function GetMyTurnIndex() As Integer
        If mode <> FunctionPool.Mode.ONLINE Then Return -1
        Return Array.IndexOf(dnsModel.GetUsernames, dnsModel.GetUsername)
    End Function

    Private Sub GameLoop()
        'moveThread = New Thread(AddressOf players(turn).GetMove)
        'moveThread.Start()
        Threading.ThreadPool.QueueUserWorkItem(AddressOf players(turn).GetMove)
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

    Public Sub SetClientsPlayers(hands() As Hand, myPlayerIndex As Integer)
        For i As Integer = 0 To players.Length - 1
            If Not i = myPlayerIndex Then
                players(i) = New Player_WEB(dnsModel.GetReadyMoves)
                players(i).SetCanSeeHand(False)
            Else
                players(i) = New Player_HUM()
                players(i).SetCanSeeHand(True)
            End If
            players(i).SetHand(hands(i))
            players(i).GetHand.SortHand()
            players(i).SetCallback(AddressOf ResultCallback)
        Next
    End Sub

    Public Sub SetTurn(turn As Integer)
        Me.turn = turn
    End Sub
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

    Private Function GetNameRef(turn As Integer) As String
        If mode = FunctionPool.Mode.ONLINE Then
            Return dnsModel.GetUsernames(turn)
        Else
            Return "Player " & turn
        End If
        Return "_PLAYER_NOT_FOUND_"
    End Function

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
            finisherStr = "   " & GetNameRef(turn) & " finishes in position " & finishers & "!"
            If finishers = 4 Then finisherStr &= vbCrLf & vbCrLf & "    GAME OVER!"
            gameView.Finisher(finishers, turn)
        End If

        Dim cardStr As String = "PASS"
        If card IsNot Nothing Then cardStr = card.GetCardText
        moveStr = GetNameRef(turn) & " plays " & cardStr

        If newTurn = GetMyTurnIndex() Then moveStr &= " : ITS YOUR MOVE!"
        If turn >= newTurn Then moveStr &= vbCrLf
        gameView.WriteToLog(moveStr)
        If finisherStr <> "" Then gameView.WriteToLog(finisherStr)

        If mode = FunctionPool.Mode.ONLINE Then
            Dim cardWebStr As String = "PASS"
            If card IsNot Nothing Then cardWebStr = card.GetSuit & "_" & card.GetValue
            If Not players(turn).GetIsWeb Then
                If wc.GetIsClient Then
                    wc.SendToServer("PLAYCARD:" & cardWebStr)
                Else
                    wc.SendToClients("PLAYCARD:" & cardWebStr)
                End If
            Else
                If Not wc.GetIsClient Then
                    Dim usernames As String = dnsModel.GetUsername & "," & wc.GetClientUsernames
                    Dim playerUsername As String = usernames.Split(",")(turn)
                    wc.SendToClients("PLAYCARD:" & cardWebStr & "-" & playerUsername)
                End If
            End If
        End If

        turn = newTurn

        If finishers < 4 Then GameLoop()
    End Sub
End Class
