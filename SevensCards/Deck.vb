Public Class Deck
    Private cards() As Card

    Public Sub New()
        cards = generateDeck()
    End Sub

    Private Function generateDeck() As Card()
        Dim tempCards As New List(Of Card)
        For i As Integer = 0 To 3
            For j As Integer = 0 To 12
                If j > CardEnums.Value.EIGHT Or j < CardEnums.Value.SIX Then tempCards.Add(New Card(i, j))
                If j = CardEnums.Value.EIGHT Or j = CardEnums.Value.SIX Then tempCards.Add(New Card(i, j, True))
            Next
        Next
        Return tempCards.ToArray
    End Function

    Public Sub shuffleDeck()
        Dim tmp As Card
        Randomize()

        For i As Integer = 0 To cards.Count - 1
            Dim j As Integer = Int(cards.Count * Rnd())
            tmp = cards(i)
            cards(i) = cards(j)
            cards(j) = tmp
        Next
    End Sub

    Public Function getCard(index As Integer) As Card
        Return cards(index)
    End Function
End Class
