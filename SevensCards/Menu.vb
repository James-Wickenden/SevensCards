Public Class Menu
    Private fp As New FunctionPool
    Private buttonGameOptions(3) As Button
    Private buttonLabels() As String = {"Offline Game", "HUM Game", "COM Game", "Web Game"}
    Private buttonAddresses() As Action = {AddressOf Load_OffGame, AddressOf Load_HUMGame, AddressOf Load_COMGame, AddressOf Load_WebGame}
    Private txt_AI_difficulty As New ComboBox
    Private lbl_AI_difficulty As New Label

    Private Sub Menu_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        fp.FormSetup(Me, "MENU")

        Dim tlp_menu As TableLayoutPanel = fp.objectHandler.AddTableLayoutPanel(Me, "tlp_menuLayout", 5, 2)

        For i As Integer = 0 To 3
            buttonGameOptions(i) = New Button
            With buttonGameOptions(i)
                .TextAlign = ContentAlignment.MiddleCenter
                .Text = buttonLabels(i)
                .BackColor = Color.White
                .Dock = DockStyle.Fill
                .AutoSize = True
                .AutoSizeMode = AutoSizeMode.GrowAndShrink
            End With

            AddHandler(buttonGameOptions(i).Click), AddressOf buttonAddresses(i).Invoke
            tlp_menu.Controls.Add(buttonGameOptions(i), 0, i)
        Next

        txt_AI_difficulty.Items.AddRange({"EASY", "MEDIUM", "HARD"})
        txt_AI_difficulty.DropDownStyle = ComboBoxStyle.DropDownList
        txt_AI_difficulty.SelectedIndex = 1
        tlp_menu.Controls.Add(txt_AI_difficulty, 1, 4)

        lbl_AI_difficulty.Font = New Font("", 15)
        lbl_AI_difficulty.Text = "AI Difficulty:"
        lbl_AI_difficulty.AutoSize = True
        tlp_menu.Controls.Add(lbl_AI_difficulty, 0, 4)
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