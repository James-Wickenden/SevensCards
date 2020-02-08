Public Class Menu
    Private fp As New FunctionPool
    Private but_OffGame, but_HUMGame, but_COMGame As New Button

    Private Sub Menu_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        fp.FormSetup(Me, "MENU")
        fp.objectHandler.AddButton(Me, but_OffGame, 25, 25, 50, 150, "Offline Game", AddressOf Load_OffGame)
        fp.objectHandler.AddButton(Me, but_HUMGame, 100, 25, 50, 150, "HUM Game", AddressOf Load_HUMGame)
        fp.objectHandler.AddButton(Me, but_COMGame, 175, 25, 50, 150, "COM Game", AddressOf Load_COMGame)
    End Sub

    Private Sub Load_OffGame()
        Dim gameModel As New GameModel(Me, FunctionPool.Mode.OFFLINE)
    End Sub

    Private Sub Load_HUMGame()
        Dim gameModel As New GameModel(Me, FunctionPool.Mode.HUM)
    End Sub

    Private Sub Load_COMGame()
        Dim gameModel As New GameModel(Me, FunctionPool.Mode.COM)
    End Sub
End Class