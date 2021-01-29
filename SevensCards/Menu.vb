Public Class Menu
    Private fp As New FunctionPool
    Private but_OffGame, but_HUMGame, but_COMGame, but_WebGame As New Button
    Private txt_AI_difficulty As New ComboBox
    Private lbl_AI_difficulty As New Label

    Private Sub Menu_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        fp.FormSetup(Me, "MENU")
        fp.objectHandler.AddButton(Me, but_OffGame, 25, 25, 50, 150, "Offline Game", AddressOf Load_OffGame)
        fp.objectHandler.AddButton(Me, but_HUMGame, 100, 25, 50, 150, "HUM Game", AddressOf Load_HUMGame)
        fp.objectHandler.AddButton(Me, but_COMGame, 175, 25, 50, 150, "COM Game", AddressOf Load_COMGame)
        fp.objectHandler.AddButton(Me, but_WebGame, 250, 25, 50, 150, "Web Game", AddressOf Load_WebGame)

        txt_AI_difficulty.Items.AddRange({"EASY", "MEDIUM", "HARD"})
        txt_AI_difficulty.DropDownStyle = ComboBoxStyle.DropDownList
        lbl_AI_difficulty.Font = New Font("", 15)
        fp.objectHandler.AddObject(Me, txt_AI_difficulty, 325, 175, 50, 150, "")
        fp.objectHandler.AddObject(Me, lbl_AI_difficulty, 325, 25, 50, 150, "AI Difficulty: ")
        txt_AI_difficulty.SelectedIndex = 1
    End Sub

    Private Sub Load_OffGame()
        Dim gameModel As New GameModel(Me, FunctionPool.Mode.OFFLINE, difficulty:=txt_AI_difficulty.SelectedIndex)
    End Sub

    Private Sub Load_HUMGame()
        Dim gameModel As New GameModel(Me, FunctionPool.Mode.HUM)
    End Sub

    Private Sub Load_COMGame()
        Dim gameModel As New GameModel(Me, FunctionPool.Mode.COM, difficulty:=txt_AI_difficulty.SelectedIndex)
    End Sub

    Private Sub Load_WebGame()
        Dim dnsModel As New DNSModel(Me)
    End Sub
End Class