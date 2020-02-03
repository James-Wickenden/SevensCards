Public Class Player_HUM
    Inherits Player
    Dim c As Card = Nothing

    Private Function GetPlayedCard_HUM() As Card
        While playedCard Is Nothing
        End While
        Return playedCard
    End Function

    Public Overrides Sub GetMove()
        Dim card As Card = GetPlayedCard_HUM()
        callback(card)
    End Sub
End Class
