Public Class Menu
    Private fp As New FunctionPool
    Private but_debugGame As New Button

    Private Sub Menu_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        fp.formSetup(Me)
        fp.objectHandler.addButton(Me, but_debugGame, 25, 25, 50, 150, "New DebugGame", AddressOf load_DebugGame)
    End Sub

    Private Sub load_DebugGame()
        Dim debugGameView As New DebugGameView
        debugGameView.Show()
        Me.Close()
    End Sub
End Class