Public Class DNSView
    Private fp As New FunctionPool
    Private serverPanel, clientPanel As New Panel

    Private Sub DNSView_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        fp.FormSetup(Me, "DNS Manager")
        PanelSetup()
    End Sub

    Private Sub PanelSetup()
        fp.objectHandler.AddObject(Me, serverPanel, 0, 0, Me.Height, Me.Width / 2, "Server")
        fp.objectHandler.AddObject(Me, clientPanel, 0, Me.Width / 2, Me.Height, Me.Width / 2, "Client")

        serverPanel.BackColor = Color.Red
        clientPanel.BackColor = Color.Green
        'fp.objectHandler.AddObject(Me, handsPanel, boardPanel.Height, 0, ((CARDHEIGHT + 4) * 10) + 20, boardPanel.Width, "")
        'handsPanel.Height = Me.Height - handsPanel.Top
        'handsPanel.BackColor = Color.BurlyWood

        'fp.objectHandler.AddObject(Me, logPanel, 0, boardPanel.Width, Me.Height, (Me.Width - boardPanel.Width), "")
        'LogSetup()

        'fp.objectHandler.AddButton(handsPanel, but_Skip, 20, ((CARDWIDTH + 10) * 12) + 40, 50, CARDWIDTH, "Skip", AddressOf Skip)
    End Sub
End Class