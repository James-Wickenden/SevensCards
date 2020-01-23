Public Class Deck
    Private cards() As Card

    Public Sub New()
        cards = generateDeck()
        If cards.Length = 10 Then MsgBox("?")
    End Sub

    Private Function generateDeck() As Card()
        Dim tempCards As New List(Of Card)
        For i As Integer = 0 To 3
            For j As Integer = 0 To 12
                tempCards.Add(New Card(i, j))
            Next
        Next
        Return tempCards.ToArray
    End Function
End Class
