Public Class Card
    Private suit As CardEnums.Suit
    Private value As CardEnums.Value

    Public Sub New(suit As CardEnums.Suit, value As CardEnums.Value)
        Me.suit = suit
        Me.value = value
    End Sub
End Class
