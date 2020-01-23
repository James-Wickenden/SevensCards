Public Class DebugGameView
    Private fp As New FunctionPool
    Private debugGame As DebugGameModel
    Const CARDHEIGHT As Integer = 105
    Const CARDWIDTH As Integer = 70
    Private lastClicked As Object
    Private boardPanel, handsPanel, activePlayerPanel As New Panel
    Private but_Skip As New Button
    Private placeLabels(3) As Label

    Private Sub DebugGameView_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        fp.formSetup(Me)
        panelSetup()
        debugGame = New DebugGameModel(Me)
    End Sub

    Private Sub panelSetup()
        fp.objectHandler.addObject(Me, boardPanel, 0, 0, ((CARDHEIGHT + 10) * 4) + 30, ((CARDWIDTH + 10) * 13) + 50, "")
        boardPanel.BackColor = Color.DarkGreen
        fp.objectHandler.addObject(Me, handsPanel, boardPanel.Height, 0, ((CARDHEIGHT + 4) * 10) + 20, boardPanel.Width, "")
        handsPanel.Height = Me.Height - handsPanel.Top
        handsPanel.BackColor = Color.BurlyWood

        fp.objectHandler.addButton(handsPanel, but_Skip, 20, ((CARDWIDTH + 10) * 12) + 40, 50, CARDWIDTH, "Skip", AddressOf skip)
    End Sub

    Public Sub drawView(board As Board, players() As Hand, turn As Integer)

        For i As Integer = 0 To 3
            For Each card As Card In board.getSuit(i).getHand
                drawCardOnBoard(card)
            Next
        Next

        For i As Integer = 0 To 3
            For Each card As Card In players(i).getHand
                drawCardOnHand(card, players(i), i, turn)

            Next
        Next

        Dim top As Integer = ((CARDHEIGHT + 15) * turn) + 20
        fp.objectHandler.addObject(handsPanel, activePlayerPanel, top, 0, CARDHEIGHT, 20, "")
        activePlayerPanel.BackColor = Color.Red

    End Sub

    Public Sub drawCardOnBoard(card As Card)
        fp.objectHandler.addObject(boardPanel, card.getView, 20 + (card.getSuit * (CARDHEIGHT + 10)),
                                                   40 + (card.getValue * (CARDWIDTH + 10)), CARDHEIGHT, CARDWIDTH, "")
        card.flip()
        card.getView.BorderStyle = BorderStyle.FixedSingle
    End Sub

    Public Sub drawCardOnHand(card As Card, player As Hand, i As Integer, turn As Integer)
        fp.objectHandler.addObject(handsPanel, card.getView, 20 + (i * (CARDHEIGHT + 15)),
                                                   40 + (player.getHand.IndexOf(card) * (CARDWIDTH + 10)), CARDHEIGHT, CARDWIDTH, "")
        fp.objectHandler.addObject(handsPanel, card.getValidBar, card.getView.Top + CARDHEIGHT,
                                   40 + (player.getHand.IndexOf(card) * (CARDWIDTH + 10)), 5, CARDWIDTH, card.getValid.ToString)
        If i = turn Then card.flip()

        AddHandler(card.getView.MouseClick), AddressOf cardSClicked
        AddHandler(card.getView.MouseDoubleClick), AddressOf cardDClicked
    End Sub

    Public Sub removeCardFromHand(hand As List(Of Card), card As Card)
        card.getValidBar.Dispose()
        For i As Integer = hand.IndexOf(card) To hand.Count - 1
            hand(i).getView.Left -= (CARDWIDTH + 10)
            hand(i).getValidBar.Left -= (CARDWIDTH + 10)
        Next
    End Sub

    Public Sub changePlayer(oldHand As List(Of Card), newHand As List(Of Card), turn As Integer)
        For Each card As Card In oldHand
            card.flip()
        Next
        For Each card As Card In newHand
            card.flip()
        Next
        activePlayerPanel.Top += activePlayerPanel.Height + 15
        If turn = 0 Then activePlayerPanel.Top = 20
    End Sub

    Private Function isMyCard(sender As Object) As Card
        Dim cards As List(Of Card) = debugGame.getHand(debugGame.getTurn)
        For Each card As Card In cards
            If card.getView.Equals(sender) Then Return card
        Next
        Return Nothing
    End Function

    Public Sub finisher(ByVal place As Integer, ByVal turn As Integer)
        If place = 4 Then but_Skip.Dispose()
        placeLabels(place - 1) = New Label
        fp.objectHandler.addObject(handsPanel, placeLabels(place - 1), 20 + (turn * (CARDHEIGHT + 15)), 40, CARDHEIGHT, CARDWIDTH, place)
        placeLabels(place - 1).Font = New Font("Arial", 40)
    End Sub

    Private Sub cardSClicked(sender As Object, e As System.EventArgs)
        If isMyCard(sender) Is Nothing Then Exit Sub
        If lastClicked IsNot Nothing Then lastClicked.BorderStyle = BorderStyle.FixedSingle
        lastClicked = sender
        sender.BorderStyle = BorderStyle.Fixed3D
    End Sub

    Private Sub cardDClicked(sender As Object, e As System.EventArgs)
        Dim c As Card = isMyCard(sender)
        If Not IsNothing(c) Then debugGame.playCard(c)
    End Sub

    Private Sub skip()
        debugGame.skip()
    End Sub
End Class