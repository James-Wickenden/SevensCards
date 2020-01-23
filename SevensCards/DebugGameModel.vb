Public Class DebugGameModel
    Private view As Form
    Private turn As Integer = 1
    Private players(3) As Hand

    Public Sub New(view As Form)
        Me.view = view

        gameSetup()

    End Sub

    Private Sub gameSetup()

    End Sub
End Class
