Public Class DebugGameView
    Private fp As New FunctionPool
    Private debugGame As DebugGameModel

    Private Sub DebugGameView_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        fp.formSetup(Me)
        debugGame = New DebugGameModel(Me)
    End Sub

End Class