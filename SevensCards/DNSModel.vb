Imports System.Net
Imports System.Net.Sockets
Imports System.Threading
Imports Microsoft.VisualBasic.ApplicationServices

Public Class DNSModel
    Private dnsView As DNSView
    Private wc As WebController
    Private username As String
    Private gameModel As GameModel
    Private readyClients As Integer = 0

    Public Sub New(menu As Menu)
        dnsView = New DNSView()
        dnsView.SetDNSModel(Me)
        dnsView.Show()
        menu.Close()
    End Sub

    Public Sub SetGameModel(gm As GameModel)
        gameModel = gm
    End Sub

    Public Sub SetReadyClients(rc As Integer)
        readyClients = rc
    End Sub
    Public Function GetReadyClients() As Integer
        Return readyClients
    End Function

    Public Sub StartServer()
        wc = New WebController(False, Me)
        Dim started As Boolean = wc.StartServer()
        If started Then dnsView.WriteToLog(dnsView.serverInfo, "Server started successfully. Set host username in client panel!")
        UpdatePlayers({username})
    End Sub

    Private Function ParseUsernames(usernames As String) As String()
        Dim splitUsers As String() = usernames.Split(",")

        Return splitUsers.Take(splitUsers.Length - 1).ToArray
    End Function

    Private Sub UpdatePlayers(usernames() As String)
        For i As Integer = 0 To dnsView.playerNames.Length - 1
            If i <= usernames.Length - 1 Then
                dnsView.playerNames(i).Text = usernames(i)
                dnsView.playerNames(i).ForeColor = Color.Black
            Else
                dnsView.playerNames(i).Text = "COM"
                dnsView.playerNames(i).ForeColor = Color.Red
            End If
        Next
    End Sub

    Public Sub BeginGame()
        If wc Is Nothing Then Exit Sub
        dnsView.WriteToLog(dnsView.clientInfo, "Beginning game...")
        wc.SendToClients("START:")
        Dim tmpGM As New GameModel(dnsView, FunctionPool.Mode.ONLINE, wc, Me)
    End Sub

    Public Sub HandleIncomingMessage(client As TcpClient, rawData As String)
        Select Case rawData.Split(":")(0)
            Case "USERNAME"
                ServerDistributeUpdatedUsernames(client, rawData)
            Case "USERNAMES"
                UpdatePlayers(ParseUsernames(rawData.Split(":")(1)))
            Case "REMOVED"
                If wc.GetIsClient Then Exit Sub
                Dim usernames As String = username & "," & wc.GetClientUsernames
                UpdatePlayers(ParseUsernames(usernames))
                wc.SendToClients("USERNAMES:" & usernames)
            Case "START"
                dnsView.Invoke(Sub()
                                   BeginGame()
                               End Sub)
            Case "GAMEINFO"
                ParseSetupBoard(rawData.Split(":")(1))
            Case "PLAYCARD"
                'MsgBox(rawData & " " & client.Client.RemoteEndPoint.ToString)
                If gameModel IsNot Nothing Then gameModel.ReceiveOnlineMove(rawData.Split(":")(1))
            Case "READYCLIENT"
                readyClients += 1
        End Select
    End Sub

    Private Sub ParseSetupBoard(gameInfo As String)
        Dim infos() As String = gameInfo.Split(" ")
        Dim int_turn As Integer = CInt(infos(0))
        If gameModel IsNot Nothing Then
            gameModel.SetTurn(int_turn)
        End If

        Dim deck() As String = infos(1).Split("-")
        Dim refDeck As New Deck
        Dim resHands As New List(Of Hand)

        For i As Integer = 0 To deck.Length - 2
            Dim cardInfo() As String = deck(i).Split("_")
            Dim c As Card = Nothing
            For Each refCard As Card In refDeck.GetCards
                If refCard.GetSuit = cardInfo(0) And refCard.GetValue = cardInfo(1) Then
                    c = refCard
                    c.SetFaceDown()
                End If
            Next
            If i Mod 12 = 0 Then resHands.Add(New Hand({}))
            If c IsNot Nothing Then resHands.Last.AddCard(c)
        Next

        Dim myPlayerIndex As Integer = 0
        For i As Integer = 0 To infos(2).Split(",").Length - 1
            If username = infos(2).Split(",")(i) Then
                myPlayerIndex = i
                For Each card As Card In resHands(i).GetHand
                    card.SetFaceUp()
                Next
            End If
        Next

        gameModel.SetClientsPlayers(resHands.ToArray, myPlayerIndex)
        wc.SetClientGameReady(True)
    End Sub

    Private Sub ServerDistributeUpdatedUsernames(client As TcpClient, rawData As String)
        wc.UpdateClientUsername(client, rawData.Split(":")(1))
        Dim usernames As String = username & "," & wc.GetClientUsernames
        If usernames = "" Then Exit Sub
        UpdatePlayers(ParseUsernames(usernames))
        wc.SendToClients("USERNAMES:" & usernames)
    End Sub

    Public Function GetUsername() As String
        Return username
    End Function

    Public Sub SetUsername(username As String)
        Me.username = username
        If wc Is Nothing Then Exit Sub

        If wc.GetIsClient() Then
            wc.SendToServer("USERNAME:" & username)
        Else
            Dim usernames As String = username & "," & wc.GetClientUsernames
            wc.SendToClients("USERNAMES:" & usernames)
            UpdatePlayers(ParseUsernames(usernames))
        End If
    End Sub

    Public Sub ClientConnect(sender As Button, ipStr As String)

        If username Is Nothing Then
            WriteToLog("Set a username first (only alphanumerics)", True)
            Exit Sub
        Else
            Dim valid As Boolean = System.Text.RegularExpressions.Regex.IsMatch(username, "^[a-zA-Z0-9]*$")
            If Not valid Then
                WriteToLog("Illegal character; only alphanumerics allowed", True)
                Exit Sub
            End If

            If wc IsNot Nothing Then
                If wc.GetIsConnected Then
                    WriteToLog("Already connected to a server", True)
                    Exit Sub
                End If
            End If
        End If
        If wc IsNot Nothing Then
            If Not wc.GetIsClient Then
                dnsView.WriteToLog(dnsView.clientInfo, "This instance is already hosting.")
                Exit Sub
            End If
        End If

        wc = New WebController(True, Me)
        Dim connected As Boolean = wc.Connect(ipStr)
        If Not connected Then
            dnsView.WriteToLog(dnsView.clientInfo, "Failed to connect.")
            Exit Sub
        End If

        wc.SendToServer("USERNAME:" & username)
    End Sub

    Public Sub WriteToLog(msg As String, isClient As Boolean)
        If isClient Then
            dnsView.WriteToLog(dnsView.clientInfo, msg)
        Else
            dnsView.WriteToLog(dnsView.serverInfo, msg)
        End If
    End Sub

    Public Sub ListDNS_Addresses(Optional Verbose As Boolean = True)
        Dim hostName As String = System.Net.Dns.GetHostName()
        Dim addresses As IPAddress() = Dns.GetHostEntry(hostName).AddressList()

        If Verbose Then dnsView.WriteToLog(dnsView.serverInfo, "All Machine IP Addresses: ")
        For Each hostAdr As IPAddress In addresses
            If hostAdr.ToString().Contains("192.168") Then
                ListDNS_Address(hostName, hostAdr.ToString())
                dnsView.WriteToLog(dnsView.serverInfo, "    [Local Network IP]")
            Else
                If Verbose Then
                    ListDNS_Address(hostName, hostAdr.ToString())
                    If hostAdr.AddressFamily.ToString() = "InterNetwork" Then
                        dnsView.WriteToLog(dnsView.serverInfo, "    [Potential host IP]")
                    End If
                End If

            End If
        Next

        dnsView.WriteToLog(dnsView.serverInfo, "Set to run with port 65535")
    End Sub

    Private Sub ListDNS_Address(hostName As String, ipAddress As String)
        dnsView.WriteToLog(dnsView.serverInfo, "Name: " & hostName & " IP Address: " & ipAddress)
    End Sub
End Class
