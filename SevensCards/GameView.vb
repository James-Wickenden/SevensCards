﻿Public Class GameView
    Private fp As New FunctionPool
    Private gameModel As GameModel
    Const CARDHEIGHT As Integer = 105
    Const CARDWIDTH As Integer = 70
    Private lastClicked As Object
    Private boardPanel, handsPanel, activePlayerPanel As New Panel
    Private but_Skip As New Button
    Private placeLabels(3) As Label

    Private Sub GameView_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        fp.FormSetup(Me, gameModel.GetModeString)
        PanelSetup()
    End Sub

    Private Sub PanelSetup()
        fp.objectHandler.AddObject(Me, boardPanel, 0, 0, ((CARDHEIGHT + 10) * 4) + 30, ((CARDWIDTH + 10) * 13) + 50, "")
        boardPanel.BackColor = Color.DarkGreen
        fp.objectHandler.AddObject(Me, handsPanel, boardPanel.Height, 0, ((CARDHEIGHT + 4) * 10) + 20, boardPanel.Width, "")
        handsPanel.Height = Me.Height - handsPanel.Top
        handsPanel.BackColor = Color.BurlyWood

        fp.objectHandler.AddButton(handsPanel, but_Skip, 20, ((CARDWIDTH + 10) * 12) + 40, 50, CARDWIDTH, "Skip", AddressOf Skip)
    End Sub

    Public Sub SetGameModel(gameModel As GameModel)
        Me.gameModel = gameModel
    End Sub

    Public Sub DrawView(board As Board, players() As Hand, turn As Integer)

        For i As Integer = 0 To 3
            For Each card As Card In board.GetSuit(i).GetHand
                DrawCardOnBoard(card)
            Next
        Next

        For i As Integer = 0 To 3
            For Each card As Card In players(i).GetHand
                DrawCardOnHand(card, players(i), i, turn)
            Next
        Next

        Dim top As Integer = ((CARDHEIGHT + 15) * turn) + 20
        fp.objectHandler.AddObject(handsPanel, activePlayerPanel, top, 0, CARDHEIGHT, 20, "")
        activePlayerPanel.BackColor = Color.Red
    End Sub

    Public Sub DrawCardOnBoard(card As Card)
        Me.Invoke(Sub() fp.objectHandler.AddObject(boardPanel, card.GetView, 20 + (card.GetSuit * (CARDHEIGHT + 10)),
                                                   40 + (card.GetValue * (CARDWIDTH + 10)), CARDHEIGHT, CARDWIDTH, ""))
        If card.GetValue = CardEnums.Value.SEVEN Then Me.Invoke(Sub() card.Flip())
        Me.Invoke(Sub() card.GetView.BorderStyle = BorderStyle.FixedSingle)
    End Sub

    Public Sub DrawCardOnHand(card As Card, player As Hand, i As Integer, turn As Integer)
        Me.Invoke(Sub() fp.objectHandler.AddObject(handsPanel, card.GetView, 20 + (i * (CARDHEIGHT + 15)),
                                                   40 + (player.GetHand.IndexOf(card) * (CARDWIDTH + 10)), CARDHEIGHT, CARDWIDTH, ""))
        Me.Invoke(Sub() fp.objectHandler.AddObject(handsPanel, card.GetValidBar, card.GetView.Top + CARDHEIGHT,
                                   40 + (player.GetHand.IndexOf(card) * (CARDWIDTH + 10)), 5, CARDWIDTH, card.GetValid.ToString))
        If i = turn Then Me.Invoke(Sub() card.Flip())

        AddHandler(card.GetView.MouseClick), AddressOf CardSClicked
        AddHandler(card.GetView.MouseDoubleClick), AddressOf CardDClicked
    End Sub

    Public Sub RemoveCardFromHand(hand As List(Of Card), card As Card)
        Me.Invoke(Sub() card.GetValidBar.Dispose())
        If Not hand.Contains(card) Then Exit Sub
        For i As Integer = hand.IndexOf(card) To hand.Count - 1
            Dim temp As Integer = i
            Me.Invoke(Sub() hand(temp).GetView.Left -= (CARDWIDTH + 10))
            Me.Invoke(Sub() hand(temp).GetValidBar.Left -= (CARDWIDTH + 10))
        Next
    End Sub

    Public Sub ChangePlayer(oldHand As List(Of Card), newHand As List(Of Card), turn As Integer)
        For Each card As Card In oldHand
            card.Flip()
        Next
        For Each card As Card In newHand
            card.Flip()
        Next
        Me.Invoke(Sub() activePlayerPanel.Top += activePlayerPanel.Height + 15)
        If turn = 0 Then Me.Invoke(Sub() activePlayerPanel.Top = 20)
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
        Me.Invoke(Sub() fp.objectHandler.AddObject(handsPanel, placeLabels(place - 1), 20 + (turn * (CARDHEIGHT + 15)), 40, CARDHEIGHT, CARDWIDTH, place))
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