Public Class Menu
    Private fp As New FunctionPool
    Private but_OffGame, but_HUMGame, but_COMGame, but_WebGame As New Button
    Private txt_AI_difficulty As New ComboBox
    Private lbl_AI_difficulty As New Label

    Private Sub Menu_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        fp.FormSetup(Me, "MENU")
        'fp.objectHandler.AddButton(Me, Me, but_OffGame, 25, 25, 50, 150, "Offline Game", AddressOf Load_OffGame)
        'fp.objectHandler.AddButton(Me, Me, but_HUMGame, 100, 25, 50, 150, "HUM Game", AddressOf Load_HUMGame)
        'fp.objectHandler.AddButton(Me, Me, but_COMGame, 175, 25, 50, 150, "COM Game", AddressOf Load_COMGame)
        'fp.objectHandler.AddButton(Me, Me, but_WebGame, 250, 25, 50, 150, "Web Game", AddressOf Load_WebGame)

        Dim tlp_menu As New TableLayoutPanel
        With tlp_menu
            .Name = "tlp_menuLayout"
            .Margin = New System.Windows.Forms.Padding(20, 20, 20, 20)
            .ColumnCount = 2
            .RowCount = 5
            .Dock = DockStyle.Fill
            .AutoSizeMode = Windows.Forms.AutoSizeMode.GrowAndShrink
            .AutoSize = True
        End With
        Me.Controls.Add(tlp_menu)
        Dim buts(3) As Button
        Dim lbls() As String = {"Offline Game", "HUM Game", "COM Game", "Web Game"}
        Dim addrs() As Action = {AddressOf Load_OffGame, AddressOf Load_HUMGame, AddressOf Load_COMGame, AddressOf Load_WebGame}

        For i As Integer = 0 To 3
            buts(i) = New Button
            With buts(i)
                .TextAlign = ContentAlignment.MiddleCenter
                .Text = lbls(i)
                .Dock = DockStyle.Fill
                .AutoSize = True
                .AutoSizeMode = AutoSizeMode.GrowAndShrink
            End With
            AddHandler(buts(i).Click), AddressOf addrs(i).Invoke
            tlp_menu.Controls.Add(buts(i), 0, i)
        Next

        txt_AI_difficulty.Items.AddRange({"EASY", "MEDIUM", "HARD"})
        txt_AI_difficulty.DropDownStyle = ComboBoxStyle.DropDownList
        lbl_AI_difficulty.Font = New Font("", 15)
        fp.objectHandler.AddObject(Me, Me, txt_AI_difficulty, 325, 175, 50, 150, "")
        fp.objectHandler.AddObject(Me, Me, lbl_AI_difficulty, 325, 25, 50, 150, "AI Difficulty: ")
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