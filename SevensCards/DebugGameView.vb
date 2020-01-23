Public Class DebugGameView
    Private fp As New FunctionPool
    Private debugGame As DebugGameModel

    Private Sub DebugGameView_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        fp.formSetup(Me)
        debugGame = New DebugGameModel(Me)
    End Sub

    Public Sub drawView(board As Board, players() As Hand)
        For i As Integer = 0 To 3
            For Each card As Card In board.getSuit(i).getHand
                Dim l As New Label
                fp.objectHandler.addObject(Me, l, 20 + (i * 40), 20 + (card.getValue * 40), 30, 30, card.getValue + 1)
            Next
        Next

        For i As Integer = 0 To 3
            For Each card As Card In players(i).getHand
                Dim l As New Label
                fp.objectHandler.addObject(Me, l, 200 + (i * 40), 20 + (players(i).getHand.IndexOf(card) * 40), 30, 30, card.getValue + 1 & card.getSuit)
            Next
        Next
    End Sub

    Public Sub playCard(player As Integer, card As Card)

    End Sub
End Class