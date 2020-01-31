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
        PanelSetup()
        Dim debugGame As New DebugGameModel(Me)
        'Dim modelThread As New Threading.Thread(Sub() debugGame = New DebugGameModel(Me))
        'modelThread.Start()
    End Sub

    Private Sub PanelSetup()
        fp.objectHandler.AddObject(Me, boardPanel, 0, 0, ((CARDHEIGHT + 10) * 4) + 30, ((CARDWIDTH + 10) * 13) + 50, "")
        boardPanel.BackColor = Color.DarkGreen
        fp.objectHandler.AddObject(Me, handsPanel, boardPanel.Height, 0, ((CARDHEIGHT + 4) * 10) + 20, boardPanel.Width, "")
        handsPanel.Height = Me.Height - handsPanel.Top
        handsPanel.BackColor = Color.BurlyWood

        fp.objectHandler.AddButton(handsPanel, but_Skip, 20, ((CARDWIDTH + 10) * 12) + 40, 50, CARDWIDTH, "Skip", AddressOf skip)
    End Sub


    Public Sub DrawView(board As Board, players() As Hand, turn As Integer)

        For i As Integer = 0 To 3
            For Each card As Card In board.GetSuit(i).GetHand
                drawCardOnBoard(card)
            Next
        Next

        For i As Integer = 0 To 3
            For Each card As Card In players(i).GetHand
                drawCardOnHand(card, players(i), i, turn)

            Next
        Next

        Dim top As Integer = ((CARDHEIGHT + 15) * turn) + 20
        fp.objectHandler.AddObject(handsPanel, activePlayerPanel, top, 0, CARDHEIGHT, 20, "")
        activePlayerPanel.BackColor = Color.Red

    End Sub

    Public Sub DrawCardOnBoard(card As Card)
        fp.objectHandler.AddObject(boardPanel, card.GetView, 20 + (card.GetSuit * (CARDHEIGHT + 10)),
                                                   40 + (card.GetValue * (CARDWIDTH + 10)), CARDHEIGHT, CARDWIDTH, "")
        card.Flip()
        card.GetView.BorderStyle = BorderStyle.FixedSingle
    End Sub

    Public Sub DrawCardOnHand(card As Card, player As Hand, i As Integer, turn As Integer)
        fp.objectHandler.AddObject(handsPanel, card.GetView, 20 + (i * (CARDHEIGHT + 15)),
                                                   40 + (player.GetHand.IndexOf(card) * (CARDWIDTH + 10)), CARDHEIGHT, CARDWIDTH, "")
        fp.objectHandler.AddObject(handsPanel, card.GetValidBar, card.GetView.Top + CARDHEIGHT,
                                   40 + (player.GetHand.IndexOf(card) * (CARDWIDTH + 10)), 5, CARDWIDTH, card.GetValid.ToString)
        If i = turn Then card.Flip()

        AddHandler(card.GetView.MouseClick), AddressOf cardSClicked
        AddHandler(card.GetView.MouseDoubleClick), AddressOf cardDClicked
    End Sub

    Public Sub RemoveCardFromHand(hand As List(Of Card), card As Card)
        card.GetValidBar.Dispose()
        For i As Integer = hand.IndexOf(card) To hand.Count - 1
            hand(i).GetView.Left -= (CARDWIDTH + 10)
            hand(i).GetValidBar.Left -= (CARDWIDTH + 10)
        Next
    End Sub

    Public Sub ChangePlayer(oldHand As List(Of Card), newHand As List(Of Card), turn As Integer)
        For Each card As Card In oldHand
            card.Flip()
        Next
        For Each card As Card In newHand
            card.Flip()
        Next
        activePlayerPanel.Top += activePlayerPanel.Height + 15
        If turn = 0 Then activePlayerPanel.Top = 20
    End Sub

    Private Function IsMyCard(sender As Object) As Card
        Dim cards As List(Of Card) = debugGame.GetHand(debugGame.GetTurn)
        For Each card As Card In cards
            If card.GetView.Equals(sender) Then Return card
        Next
        Return Nothing
    End Function

    Public Sub Finisher(ByVal place As Integer, ByVal turn As Integer)
        If place = 4 Then but_Skip.Dispose()
        placeLabels(place - 1) = New Label
        fp.objectHandler.AddObject(handsPanel, placeLabels(place - 1), 20 + (turn * (CARDHEIGHT + 15)), 40, CARDHEIGHT, CARDWIDTH, place)
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
        If Not IsNothing(c) Then debugGame.PlayCard(c)
    End Sub

    Private Sub Skip()
        debugGame.Skip()
    End Sub
End Class