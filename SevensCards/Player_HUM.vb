Public Class Player_HUM
    Inherits Player

    Public Sub New()
        canSeeHand = True
    End Sub

    Private Function GetPlayedCard_HUM() As Card
        While Not isMyMove
        End While
        isMyMove = False
        While playedCard Is Nothing
            If skipMove Then
                skipMove = False
                Return Nothing
            End If
        End While

        Return playedCard
    End Function

    Public Overrides Sub GetMove()
        Dim card As Card = GetPlayedCard_HUM()
        playedCard = Nothing
        callback(card)
    End Sub

End Class
