Public Class GameView
    Private fp As New FunctionPool
    Private gameModel As GameModel
    Const CARDHEIGHT As Integer = 105
    Const CARDWIDTH As Integer = 70
    Private lastClicked As Object
    Private loadPanel, boardPanel, handsPanel, activePlayerPanel, logPanel As New Panel
    Private gameLog As New TextBox
    Private but_Skip As New Button
    Private placeLabels(3) As Label

    Private Sub GameView_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        fp.FormSetup(Me, gameModel.GetModeString)

        loadPanel.BackColor = Color.Black
        fp.objectHandler.AddObject(Me, Me, loadPanel, 0, 0, Me.Height, Me.Width, "")

        PanelSetup()
    End Sub

    Private Sub GameView_Close(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles Me.FormClosing
        gameModel.GameClose()
    End Sub

    Private Sub PanelSetup()
        fp.objectHandler.AddObject(Me, Me, boardPanel, 0, 0, ((CARDHEIGHT + 10) * 4) + 30, ((CARDWIDTH + 10) * 13) + 50, "")
        boardPanel.BackColor = Color.DarkGreen

        fp.objectHandler.AddObject(Me, Me, handsPanel, ((CARDHEIGHT + 10) * 4) + 30, 0, ((CARDHEIGHT + 4) * 10) + 20, ((CARDWIDTH + 10) * 13) + 50, "")
        handsPanel.Height = Me.Height - handsPanel.Top
        handsPanel.BackColor = Color.BurlyWood

        fp.objectHandler.AddObject(Me, Me, logPanel, 0, boardPanel.Width, Me.Height, (Me.Width - boardPanel.Width), "", shouldScale:=False)
        LogSetup()

        fp.objectHandler.AddButton(Me, handsPanel, but_Skip, 20, ((CARDWIDTH + 10) * 12) + 40, 50, CARDWIDTH, "Skip", AddressOf Skip)
    End Sub

    Private Sub LogSetup()
        fp.objectHandler.AddObject(Me, logPanel, gameLog, 0, 0, logPanel.Height - 40, logPanel.Width - 15, ">", shouldScale:=False)

        gameLog.Multiline = True
        gameLog.ReadOnly = True
        gameLog.ScrollBars = ScrollBars.Vertical
        gameLog.BackColor = Color.Black
        gameLog.ForeColor = Color.White
        gameLog.Font = New Font("Courier New", 15)
    End Sub

    Public Sub WriteToLog(str As String)
        Me.Invoke(Sub()
                      gameLog.Text = gameLog.Text.Remove(gameLog.Text.Count - 1)
                      gameLog.Text &= " " & str & vbCrLf & ">"
                      gameLog.SelectionStart = gameLog.Text.Length - 1
                      gameLog.ScrollToCaret()
                  End Sub)
    End Sub

    Public Sub KillSkip()
        but_Skip.Dispose()
    End Sub

    Public Sub SetGameModel(gameModel As GameModel)
        Me.gameModel = gameModel
    End Sub

    Public Sub DrawView(board As Board, players() As Player, turn As Integer, mode As FunctionPool.Mode)

        For i As Integer = 0 To 3
            For Each card As Card In board.GetSuit(i).GetHand
                DrawCardOnBoard(card)
            Next
        Next

        For i As Integer = 0 To 3
            For Each card As Card In players(i).GetHandCards
                DrawCardOnHand(card, players(i), i, turn, mode)
            Next
        Next

        Dim top As Integer = ((CARDHEIGHT + 15) * turn) + 20
        fp.objectHandler.AddObject(Me, handsPanel, activePlayerPanel, top, 0, CARDHEIGHT, 20, "")
        activePlayerPanel.BackColor = Color.Red
        loadPanel.Dispose()
    End Sub

    Public Sub DrawCardOnBoard(card As Card)
        Me.Invoke(Sub() fp.objectHandler.AddObject(Me, boardPanel, card.GetView, 20 + (card.GetSuit * (CARDHEIGHT + 10)),
                                                   40 + (card.GetValue * (CARDWIDTH + 10)), CARDHEIGHT, CARDWIDTH, ""))
        card.SetFaceUp()
        Me.Invoke(Sub() card.GetView.BorderStyle = BorderStyle.FixedSingle)
    End Sub

    Public Sub DrawCardOnHand(card As Card, player As Player, i As Integer, turn As Integer, mode As FunctionPool.Mode)

        Me.Invoke(Sub() fp.objectHandler.AddObject(Me, handsPanel, card.GetView, 20 + (i * (CARDHEIGHT + 15)),
                                                   40 + (player.GetHandCards.IndexOf(card) * (CARDWIDTH + 10)), CARDHEIGHT, CARDWIDTH, ""))
        Me.Invoke(Sub() fp.objectHandler.AddObject(Me, handsPanel, card.GetValidBar, 5 + ((i + 1) * (CARDHEIGHT + 15)),
                                   40 + (player.GetHandCards.IndexOf(card) * (CARDWIDTH + 10)), 5, CARDWIDTH, card.GetValid.ToString))

        If (i = turn And mode = FunctionPool.Mode.HUM) Or (player.GetCanSeeHand And mode <> FunctionPool.Mode.HUM) Then
            Me.Invoke(Sub() card.SetFaceUp())
        End If

        AddHandler(card.GetView.MouseClick), AddressOf CardSClicked
        AddHandler(card.GetView.MouseDoubleClick), AddressOf CardDClicked
    End Sub

    Public Sub RemoveCardFromHand(hand As List(Of Card), card As Card)
        Me.Invoke(Sub() card.GetValidBar.Dispose())
        If Not hand.Contains(card) Then Exit Sub
        For i As Integer = hand.IndexOf(card) To hand.Count - 1
            Dim temp As Integer = i
            Dim scaledCARDWITH As Integer = fp.objectHandler.ScaleDimension(CARDWIDTH + 10, form_width:=Me.Width)
            Me.Invoke(Sub() hand(temp).GetView.Left -= (scaledCARDWITH))
            Me.Invoke(Sub() hand(temp).GetValidBar.Left -= (scaledCARDWITH))
        Next
    End Sub

    Private Function CalculateAPTop(turn As Integer) As Integer
        Return fp.objectHandler.ScaleDimension(((CARDHEIGHT + 15) * turn) + 20, form_height:=Me.Height)
    End Function

    Public Sub ChangePlayer(oldHand As List(Of Card), newHand As List(Of Card), turn As Integer, isOldHandVisible As Boolean, isNewHandVisible As Boolean)
        For Each card As Card In oldHand
            card.CardUpDown(isOldHandVisible)
        Next
        For Each card As Card In newHand
            card.CardUpDown(isNewHandVisible)
        Next
        Me.Invoke(Sub() activePlayerPanel.Top = CalculateAPTop(turn))
    End Sub

    Private Function IsMyCard(sender As Object) As Card
        Dim cards As List(Of Card) = gameModel.GetHand(gameModel.GetTurn)
        For Each card As Card In cards
            If card.GetView.Equals(sender) Then Return card
        Next
        Return Nothing
    End Function

    Public Sub Finisher(ByVal place As Integer, ByVal turn As Integer)
        If place = 4 Then Me.Invoke(Sub() but_Skip.Dispose())
        placeLabels(place - 1) = New Label
        Me.Invoke(Sub() fp.objectHandler.AddObject(Me, handsPanel, placeLabels(place - 1), 20 + (turn * (CARDHEIGHT + 15)), 40, CARDHEIGHT, CARDWIDTH, place))
        placeLabels(place - 1).Font = New Font("Arial", 40)
    End Sub

    Private Sub CardSClicked(sender As Object, e As System.EventArgs)
        If IsMyCard(sender) Is Nothing Then Exit Sub
        If lastClicked IsNot Nothing Then lastClicked.BorderStyle = BorderStyle.FixedSingle
        lastClicked = sender
        sender.BorderStyle = BorderStyle.Fixed3D
    End Sub

    Private Sub CardDClicked(sender As Object, e As System.EventArgs)
        Dim c As Card = IsMyCard(sender)
        If Not IsNothing(c) Then
            If c.GetValid Then gameModel.PlayCard(c)
        End If
    End Sub

    Private Sub Skip()
        gameModel.Skip()
    End Sub
End Class