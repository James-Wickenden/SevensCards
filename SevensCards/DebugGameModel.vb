﻿Public Class DebugGameModel
    Private view As DebugGameView
    Private turn As Integer = 1
    Private players(3) As Hand
    Private board As New Board
    Private finishers As Integer = 0

    'TODO:
    '   Make players a class with human and COM inheritors
    '   Implement networking and refactor for decentralised hands

    Public Sub New(view As DebugGameView)
        Me.view = view

        gameSetup()
        Me.view.drawView(board, players, turn)
    End Sub

    Private Sub gameSetup()
        Dim deck As New Deck()
        deck.shuffleDeck()

        For i As Integer = 0 To 3
            Dim playerHand As New List(Of Card)
            For j As Integer = 0 To 11
                playerHand.Add(deck.getCard((12 * i) + j))
            Next
            players(i) = New Hand(playerHand.ToArray)
            players(i).sortHand()
        Next
        Randomize()
        turn = Int((4) * Rnd())
    End Sub

    Private Sub updateValidCards(card As Card)
        If card.getValue <> CardEnums.Value.KING And card.getValue <> CardEnums.Value.ACE Then card.getadjCard.setValid(True)
    End Sub

    Public Function getHand(index As Integer) As List(Of Card)
        Return players(index).getHand
    End Function

    Public Function getTurn() As Integer
        Return turn
    End Function

    Public Sub skip()
        Dim newTurn As Integer = getNextPlayer()
        view.changePlayer(players(turn).getHand, players(newTurn).getHand, newTurn)
        turn = newTurn
    End Sub

    Private Function getNextPlayer() As Integer
        Dim x As Integer = turn
        Do
            x = (x + 1) Mod 4
        Loop Until players(x).getHand.Count > 0
        Return x
    End Function

    Public Sub playCard(card As Card)
        Dim newTurn As Integer = getNextPlayer()
        If Not card.getValid Then Exit Sub
        board.getSuit(card.getSuit).addCard(card)

        view.removeCardFromHand(players(turn).getHand, card)
        view.drawCardOnBoard(card)
        view.changePlayer(players(turn).getHand, players(newTurn).getHand, newTurn)
        players(turn).removeCard(card)

        If players(turn).getHand.Count = 0 Then
            finishers += 1
            view.finisher(finishers, turn)
        End If
        updateValidCards(card)
        turn = newTurn
    End Sub
End Class
