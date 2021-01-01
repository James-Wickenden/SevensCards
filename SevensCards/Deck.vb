Public Class Deck
    Private cards() As Card

    Public Sub New()
        cards = generateDeck()
    End Sub

    Private Function GenerateDeck() As Card()
        Dim tempCards As New List(Of Card)

        For i As Integer = CardEnums.Suit.DIAMOND To CardEnums.Suit.SPADE
            tempCards.Add(New Card(i, CardEnums.Value.KING, False))
            For j As Integer = CardEnums.Value.QUEEN To CardEnums.Value.EIGHT Step -1
                tempCards.Add(New Card(i, j, False, tempCards.Last))
                If tempCards.Last.GetValue = CardEnums.Value.EIGHT Then tempCards.Last.SetValid(True)
            Next

            tempCards.Add(New Card(i, CardEnums.Value.ACE, False))
            For j As Integer = CardEnums.Value.TWO To CardEnums.Value.SIX Step 1
                tempCards.Add(New Card(i, j, False, tempCards.Last))
                If tempCards.Last.GetValue = CardEnums.Value.SIX Then tempCards.Last.SetValid(True)
            Next

        Next
        Return tempCards.ToArray
    End Function

    Public Sub ShuffleDeck()
        Dim tmp As Card
        Randomize()

        For i As Integer = 0 To cards.Count - 1
            Dim j As Integer = Int(cards.Count * Rnd())
            tmp = cards(i)
            cards(i) = cards(j)
            cards(j) = tmp
        Next
    End Sub

    Public Function GetCard(index As Integer) As Card
        Return cards(index)
    End Function

    Public Function GetCards() As Card()
        Return cards
    End Function
End Class
