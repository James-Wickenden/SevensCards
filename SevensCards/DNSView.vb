Public Class DNSView
    Private fp As New FunctionPool
    Private dnsModel As DNSModel
    Public clientInfo, serverInfo As TextBox

    Private Sub DNSView_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        fp.FormSetup(Me, "DNS Manager")
        PanelSetup()
    End Sub

    Public Sub SetDNSModel(dnsModel As DNSModel)
        Me.dnsModel = dnsModel
        dnsModel.SetViewInModel(Me)
    End Sub

    Public Sub WriteToLog(log As TextBox, str As String)
        log.Text = log.Text.Remove(log.Text.Count - 1)
        log.Text &= " " & str & vbCrLf & ">"
        log.SelectionStart = log.Text.Length - 1
        log.ScrollToCaret()
    End Sub

    Private Sub PanelSetup()
        Dim serverPanel, clientPanel As New Panel
        Dim serverBut As New Button
        fp.objectHandler.AddButton(Me, serverBut, 20, Me.Width / 2 + 20, 50, 120, "Host Server",
                                   New Action(Sub() ServerPanelSetup(serverBut, serverPanel)))
        ClientPanelSetup(clientPanel)
    End Sub

    Private Sub ServerPanelSetup(sender As Button, serverPanel As Panel)
        sender.Dispose()

        fp.objectHandler.AddObject(Me, serverPanel, 0, Me.Width / 2, Me.Height, Me.Width / 2, "")
        serverPanel.BorderStyle = BorderStyle.Fixed3D
        serverPanel.BackColor = Me.BackColor

        Dim serverLabel As New Label
        fp.objectHandler.AddObject(serverPanel, serverLabel, 20, 20, 40, serverPanel.Width - 40, "SERVER")
        serverLabel.Font = New Font("Arial", 25)

        serverInfo = New TextBox
        fp.objectHandler.AddObject(serverPanel, serverInfo, 80, 20, serverPanel.Height / 2,
                                   serverPanel.Width - 60, " ")
        LogSetup(serverInfo, "Server IP Info")

        StartServerPanelSetup(serverPanel)

        dnsModel.ListDNS_Addresses(False)
    End Sub

    Private Sub ClientPanelSetup(clientPanel As Panel)
        fp.objectHandler.AddObject(Me, clientPanel, 0, 0, Me.Height, Me.Width / 2, "")
        clientPanel.BorderStyle = BorderStyle.Fixed3D
        clientPanel.BackColor = Me.BackColor

        Dim clientLabel As New Label
        fp.objectHandler.AddObject(clientPanel, clientLabel, 20, 20, 40, clientPanel.Width - 40, "CLIENT")
        clientLabel.Font = New Font("Arial", 25)

        clientInfo = New TextBox
        fp.objectHandler.AddObject(clientPanel, clientInfo, 80, 20, clientPanel.Height / 2,
                                   clientPanel.Width - 40, " ")
        LogSetup(clientInfo, "Client IP Info")

        ConnectPanelSetup(clientPanel)

    End Sub

    Private Sub ConnectPanelSetup(clientPanel As Panel)
        Dim connectPanel As New Panel
        fp.objectHandler.AddObject(clientPanel, connectPanel, clientPanel.Height / 2 + 100, 20,
                                   clientPanel.Height / 3 - 20, clientPanel.Width - 40, "")
        connectPanel.BorderStyle = BorderStyle.FixedSingle
        connectPanel.BackColor = Color.Green

        Dim connectIP_lbl As New Label
        fp.objectHandler.AddObject(connectPanel, connectIP_lbl, 20, 20, 30, 120, "IP Address:")
        connectIP_lbl.Font = New Font("Arial", 15)
        connectIP_lbl.TextAlign = ContentAlignment.MiddleRight

        Dim connectIP_txt As New TextBox
        fp.objectHandler.AddObject(connectPanel, connectIP_txt, 20, 160, 30, 120, "")

        Dim connectButton As New Button
        fp.objectHandler.AddButton(connectPanel, connectButton, 20, 300, 30, 120, "Connect",
                                   New Action(Sub() ClientConnect(connectIP_txt.Text)))

    End Sub

    Private Sub StartServerPanelSetup(serverPanel As Panel)
        Dim hostPanel As New Panel
        fp.objectHandler.AddObject(serverPanel, hostPanel, serverPanel.Height / 2 + 100, 20,
                                   serverPanel.Height / 3 - 20, serverPanel.Width - 60, "")
        hostPanel.BorderStyle = BorderStyle.FixedSingle
        hostPanel.BackColor = Color.Green

        Dim AIdifficulty_lbl As New Label
        fp.objectHandler.AddObject(hostPanel, AIdifficulty_lbl, 20, 20, 30, 120, "AI difficulty:")
        AIdifficulty_lbl.Font = New Font("Arial", 15)
        AIdifficulty_lbl.TextAlign = ContentAlignment.MiddleRight

        Dim AIdifficulty_sel As New ComboBox
        fp.objectHandler.AddObject(hostPanel, AIdifficulty_sel, 20, 160, 30, 120, "")
        AIdifficulty_sel.Items.AddRange({"Easy", "Medium", "Hard"})

        Dim startServer_but As New Button
        fp.objectHandler.AddButton(hostPanel, startServer_but, 70, 20, 30, 120, "Start Server", AddressOf dnsModel.StartServer)

        Dim showIPs_verbose_but As New Button
        fp.objectHandler.AddButton(hostPanel, showIPs_verbose_but, 20, 300, 30, 120, "Show local IPs", AddressOf dnsModel.ListDNS_Addresses)
    End Sub

    Private Sub LogSetup(Log As TextBox, dispStr As String)
        Log.Multiline = True
        Log.ReadOnly = True
        Log.ScrollBars = ScrollBars.Vertical
        Log.BackColor = Color.Black
        Log.ForeColor = Color.White
        Log.Font = New Font("Courier New", 15)
        WriteToLog(Log, dispStr)
    End Sub

    Private Sub ClientConnect(ipStr As String)
        WriteToLog(clientInfo, ipStr)
    End Sub
End Class