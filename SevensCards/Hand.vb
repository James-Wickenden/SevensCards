Public Class Hand
    Private cards As List(Of Card)

    Public Sub New(hand() As Card)
        cards = hand.ToList
    End Sub

    Public Function getHand() As List(Of Card)
        Return cards
    End Function

    Public Sub addCard(card As Card)
        cards.Add(card)
    End Sub

    Public Sub removeCard(card As Card)
        cards.Remove(card)
    End Sub
End Class
