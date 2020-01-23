Public Class DebugGameModel
    Private view As DebugGameView
    Private turn As Integer = 1
    Private players(3) As Hand
    Private board As New Board

    Public Sub New(view As DebugGameView)
        Me.view = view

        gameSetup()
        Me.view.drawView(board, players)
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
        Next
        turn = Rnd() * 4
    End Sub

End Class
