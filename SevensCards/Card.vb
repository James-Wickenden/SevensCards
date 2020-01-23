Public Class Card
    Private suit As CardEnums.Suit
    Private value As CardEnums.Value

    Public Sub New(suit As CardEnums.Suit, value As CardEnums.Value)
        Me.suit = suit
        Me.value = value
    End Sub

    Public Function getSuit() As CardEnums.Suit
        Return suit
    End Function
    Public Function getValue() As CardEnums.Value
        Return value
    End Function
End Class
