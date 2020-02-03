Public Class Menu
    Private fp As New FunctionPool
    Private but_debugGame, but_COMGame As New Button

    Private Sub Menu_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        fp.formSetup(Me)
        fp.objectHandler.addButton(Me, but_debugGame, 25, 25, 50, 150, "New DebugGame", AddressOf load_DebugGame)
        fp.objectHandler.addButton(Me, but_COMGame, 100, 25, 50, 150, "New COMGame", AddressOf load_COMGame)
    End Sub

    Private Sub Load_DebugGame()
        Dim debugGameView As New DebugGameView
        debugGameView.Show()
        Me.Close()
    End Sub

    Private Sub Load_COMGame()
        MsgBox("wip")
    End Sub
End Class