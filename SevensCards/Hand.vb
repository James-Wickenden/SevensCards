Public Class Hand
    Private cards As List(Of Card)

    Public Sub New(hand() As Card)
        cards = hand.ToList
    End Sub

    Public Function GetHand() As List(Of Card)
        Return cards
    End Function

    Public Sub AddCard(card As Card)
        cards.Add(card)
    End Sub

    Public Sub RemoveCard(card As Card)
        cards.Remove(card)
    End Sub

    Public Sub SortHand()
        cards.Sort()
    End Sub

End Class
