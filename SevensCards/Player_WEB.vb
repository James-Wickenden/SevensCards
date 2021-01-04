Imports System.Net

Public Class Player_WEB
    Inherits Player

    Private webMoveCard As Card
    Private readyToPlay As Boolean = False

    Public Sub New()
        isWeb = True
    End Sub

    Private Function GetPlayedCard_WEB() As Card
        webMoveCard = Nothing
        While Not readyToPlay
            Threading.Thread.Sleep(300)
        End While

        If webMoveCard Is Nothing Then Return Nothing

        For Each card As Card In hand.GetHand
            If card.GetValue = webMoveCard.GetValue And card.GetSuit = webMoveCard.GetSuit Then
                If card.GetValid Then Return card
            End If
        Next
        If webMoveCard.GetValid Then Return webMoveCard
        Return Nothing
    End Function

    Public Sub RecieveWebMove(rawData As String)
        If Not rawData = "SKIP" Then
            Dim tmpCard As New Card(rawData.Split("_")(0), rawData.Split("_")(1), False)
            webMoveCard = tmpCard
        End If
        readyToPlay = True
    End Sub

    Public Overrides Sub GetMove()
        Dim card As Card = GetPlayedCard_WEB()
        callback(card)
    End Sub

End Class
