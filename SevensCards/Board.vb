Public Class Board
    Private suits(3) As Hand

    Public Sub New()
        For i As Integer = 0 To 3
            suits(i) = New Hand({})
            suits(i).addCard(New Card(i, CardEnums.Value.SEVEN, Nothing))
        Next
    End Sub

    Public Function getSuit(index As Integer) As Hand
        Return suits(index)
    End Function
End Class
