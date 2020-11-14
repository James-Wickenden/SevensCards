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

        ServerPanelSetup()
        ClientPanelSetup()

        'fp.objectHandler.AddObject(Me, handsPanel, boardPanel.Height, 0, ((CARDHEIGHT + 4) * 10) + 20, boardPanel.Width, "")
        'handsPanel.Height = Me.Height - handsPanel.Top
        'handsPanel.BackColor = Color.BurlyWood

        'fp.objectHandler.AddObject(Me, logPanel, 0, boardPanel.Width, Me.Height, (Me.Width - boardPanel.Width), "")
        'LogSetup()

        'fp.objectHandler.AddButton(handsPanel, but_Skip, 20, ((CARDWIDTH + 10) * 12) + 40, 50, CARDWIDTH, "Skip", AddressOf Skip)
    End Sub

    Private Sub ServerPanelSetup()
        serverPanel.BorderStyle = BorderStyle.Fixed3D
        serverPanel.BackColor = Me.BackColor

        Dim serverLabel As New Label
        fp.objectHandler.AddObject(serverPanel, serverLabel, 20, 20, 40, serverPanel.Width - 40, "SERVER")
        serverLabel.Font = New Font("Arial", 25)

        Dim serverInfo As New TextBox
        fp.objectHandler.AddObject(serverPanel, serverInfo, 80, 20, clientPanel.Height / 2, serverPanel.Width - 40, "Server IP Info")
        LogSetup(serverInfo)
    End Sub

    Private Sub ClientPanelSetup()
        clientPanel.BorderStyle = BorderStyle.Fixed3D
        clientPanel.BackColor = Me.BackColor

        Dim clientLabel As New Label
        fp.objectHandler.AddObject(clientPanel, clientLabel, 20, 20, 40, clientPanel.Width - 40, "CLIENT")
        clientLabel.Font = New Font("Arial", 25)

        Dim clientInfo As New TextBox
        fp.objectHandler.AddObject(clientPanel, clientInfo, 80, 20, clientPanel.Height / 2, clientPanel.Width - 40, "Client IP Info")
        LogSetup(clientInfo)
    End Sub

    Private Sub LogSetup(Log As TextBox)
        Log.Multiline = True
        Log.ReadOnly = True
        Log.ScrollBars = ScrollBars.Vertical
        Log.BackColor = Color.Black
        Log.ForeColor = Color.White
        Log.Font = New Font("Courier New", 15)
    End Sub
End Class