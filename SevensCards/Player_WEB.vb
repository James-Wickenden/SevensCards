Imports System.Net

Public Class Player_WEB
    Inherits Player

    Public Sub New()

    End Sub

    Private Function GetPlayedCard_WEB() As Card
        Dim message As String = ""
        Dim cardReceived As Boolean = False
        Do

        Loop Until cardReceived

        For Each card As Card In hand.GetHand
            If card.GetValid Then Return card
        Next
        Return Nothing
    End Function

    Public Overrides Sub GetMove()
        Dim card As Card = GetPlayedCard_WEB()
        callback(card)
    End Sub

End Class
