Imports Microsoft.VisualBasic.ApplicationServices

Public Class DNSView
    Private fp As New FunctionPool
    Private dnsModel As DNSModel
    Public clientInfo, serverInfo, username_txt As TextBox
    Public playerNames(3) As Label
    Public AIdifficulty_sel As New ComboBox

    Private Sub DNSView_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CheckForIllegalCrossThreadCalls = False
        fp.FormSetup(Me, "DNS Manager")
        PanelSetup()
    End Sub

    Private Sub DNSView_Close(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles Me.FormClosing
        If sender.ToString.Contains("Expired_DNSView") Then Exit Sub
        dnsModel.StopServer()
    End Sub

    Public Sub SetDNSModel(dnsModel As DNSModel)
        Me.dnsModel = dnsModel
    End Sub

    Public Sub WriteToLog(log As TextBox, str As String)
        Try
            Me.Invoke(Sub()
                          If log Is Nothing Then Exit Sub
                          log.Text = log.Text.Remove(log.Text.Count - 1)
                          log.Text &= " " & str & vbCrLf & ">"
                          log.SelectionStart = log.Text.Length - 1
                          log.ScrollToCaret()
                      End Sub)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub PanelSetup()
        Dim serverPanel, clientPanel As New Panel
        Dim serverBut As New Button
        fp.objectHandler.AddButton(Me, Me, serverBut, 20, CInt(Me.Width / 2 + 20), 50, 120, "Host Server",
                                   New Action(Sub() ServerPanelSetup(serverBut, serverPanel)), shouldScale:=False)
        ClientPanelSetup(clientPanel)
    End Sub

    Private Sub ServerPanelSetup(sender As Button, serverPanel As Panel)
        sender.Dispose()
        fp.objectHandler.AddObject(Me, Me, serverPanel, 0, Me.Width / 2, Me.Height, Me.Width / 2, "", shouldScale:=False)
        serverPanel.BorderStyle = BorderStyle.Fixed3D
        serverPanel.BackColor = Me.BackColor

        Dim serverLabel As New Label
        fp.objectHandler.AddObject(Me, serverPanel, serverLabel, 20, 20, 40, serverPanel.Width - 40, "SERVER", shouldScale:=False)
        serverLabel.Font = New Font("Arial", 25)

        serverInfo = New TextBox
        fp.objectHandler.AddObject(Me, serverPanel, serverInfo, 80, 20, serverPanel.Height / 3,
                                   serverPanel.Width - 60, " ", shouldScale:=False)
        LogSetup(serverInfo, "Server IP Info")

        StartServerPanelSetup(serverPanel)

        dnsModel.ListDNS_Addresses(False)
    End Sub

    Private Sub ClientPanelSetup(clientPanel As Panel)
        fp.objectHandler.AddObject(Me, Me, clientPanel, 0, 0, Me.Height, Me.Width / 2, "", shouldScale:=False)
        clientPanel.BorderStyle = BorderStyle.Fixed3D
        clientPanel.BackColor = Me.BackColor

        Dim clientLabel As New Label
        fp.objectHandler.AddObject(Me, clientPanel, clientLabel, 20, 20, 40, clientPanel.Width - 40, "CLIENT", shouldScale:=False)
        clientLabel.Font = New Font("Arial", 25)

        clientInfo = New TextBox
        fp.objectHandler.AddObject(Me, clientPanel, clientInfo, 80, 20, clientPanel.Height / 3,
                                   clientPanel.Width - 40, " ", shouldScale:=False)
        LogSetup(clientInfo, "Client IP Info")

        ConnectPanelSetup(clientPanel)
        CurClientsSetup(clientPanel)
    End Sub

    Private Sub ConnectPanelSetup(clientPanel As Panel)
        Dim connectPanel As New Panel
        fp.objectHandler.AddObject(Me, clientPanel, connectPanel, clientPanel.Height / 3 + 100, 20,
                                   clientPanel.Height / 3 - 20, clientPanel.Width / 2 - 40, "", shouldScale:=False)
        connectPanel.BorderStyle = BorderStyle.FixedSingle
        connectPanel.BackColor = Color.Green

        Dim connectIP_lbl As New Label
        fp.objectHandler.AddObject(Me, connectPanel, connectIP_lbl, 20, 20, 30, 120, "IP Address:")
        connectIP_lbl.Font = New Font("Arial", fp.objectHandler.ScaleDimension(15, Me.Width))
        connectIP_lbl.TextAlign = ContentAlignment.MiddleRight

        Dim connectIP_txt As New TextBox
        fp.objectHandler.AddObject(Me, connectPanel, connectIP_txt, 20, 160, 80, 120, "192.168.0.")

        Dim connectButton As New Button
        fp.objectHandler.AddButton(Me, connectPanel, connectButton, 20, 300, 80, 120, "Connect",
                                   New Action(Sub() dnsModel.ClientConnect(connectButton, connectIP_txt.Text)))

        Dim disconnectButton As New Button
        fp.objectHandler.AddButton(Me, connectPanel, disconnectButton, 220, 300, 80, 120, "Disconnect", AddressOf dnsModel.ClientDisconnect)

        Dim username_lbl As New Label
        fp.objectHandler.AddObject(Me, connectPanel, username_lbl, 120, 20, 30, 120, "Username:")
        username_lbl.Font = New Font("Arial", fp.objectHandler.ScaleDimension(15, Me.Width))
        username_lbl.TextAlign = ContentAlignment.MiddleRight

        username_txt = New TextBox
        fp.objectHandler.AddObject(Me, connectPanel, username_txt, 120, 160, 80, 120, "")

        Dim username_but As New Button
        fp.objectHandler.AddButton(Me, connectPanel, username_but, 120, 300, 80, 120, "Set Username",
                                   New Action(Sub() SetUsername(username_txt.Text)))

        GenerateUsername(username_txt)
    End Sub

    Private Sub GenerateUsername(username_txt As TextBox)
        Dim name As String = ""
        Dim generator As New Random
        For i As Integer = 0 To 10
            name &= Microsoft.VisualBasic.Chr(generator.Next(Asc("a"), Asc("z")))
        Next
        username_txt.Text = name
        'username_txt.Text = "test"
        SetUsername(username_txt.Text)
    End Sub

    Private Sub StartServerPanelSetup(serverPanel As Panel)
        Dim hostPanel As New Panel
        fp.objectHandler.AddObject(Me, serverPanel, hostPanel, serverPanel.Height / 3 + 100, 20,
                                   serverPanel.Height / 3 - 20, serverPanel.Width - 60, "", shouldScale:=False)
        hostPanel.BorderStyle = BorderStyle.FixedSingle
        hostPanel.BackColor = Color.Green

        Dim AIdifficulty_lbl As New Label
        fp.objectHandler.AddObject(Me, hostPanel, AIdifficulty_lbl, 20, 20, 30, 120, "AI difficulty:", shouldScale:=False)
        AIdifficulty_lbl.Font = New Font("Arial", fp.objectHandler.ScaleDimension(15, Me.Width))
        AIdifficulty_lbl.TextAlign = ContentAlignment.MiddleRight

        fp.objectHandler.AddObject(Me, hostPanel, AIdifficulty_sel, 20, 160, 30, 120, "", shouldScale:=False)
        AIdifficulty_sel.Items.AddRange({"EASY", "MEDIUM", "HARD"})
        AIdifficulty_sel.SelectedIndex = 1
        AIdifficulty_sel.DropDownStyle = ComboBoxStyle.DropDownList

        Dim startServer_but As New Button
        fp.objectHandler.AddButton(Me, hostPanel, startServer_but, 70, 20, 30, 120, "Start Server", AddressOf dnsModel.StartServer, shouldScale:=False)

        Dim loadGame_but As New Button
        fp.objectHandler.AddButton(Me, hostPanel, loadGame_but, 70, 140, 30, 120, "Begin Game", AddressOf dnsModel.BeginGame, shouldScale:=False)

        Dim stopServer_but As New Button
        fp.objectHandler.AddButton(Me, hostPanel, stopServer_but, 70, 260, 30, 120, "Stop Server", AddressOf dnsModel.StopServer, shouldScale:=False)

        Dim showIPs_verbose_but As New Button
        fp.objectHandler.AddButton(Me, hostPanel, showIPs_verbose_but, 20, 300, 30, 120, "Show local IPs", AddressOf dnsModel.ListDNS_Addresses, shouldScale:=False)
    End Sub

    Private Sub CurClientsSetup(clientPanel As Panel)
        Dim playersPanel As New Panel
        fp.objectHandler.AddObject(Me, clientPanel, playersPanel, clientPanel.Height / 3 + 100, clientPanel.Width / 2,
                                   clientPanel.Height / 3 - 20, clientPanel.Width / 2 - 20, "", shouldScale:=False)

        playersPanel.BorderStyle = BorderStyle.FixedSingle
        playersPanel.BackColor = Color.Green
        Dim titlelab As New Label
        titlelab.Font = New Font("Arial", fp.objectHandler.ScaleDimension(15, Me.Width))
        fp.objectHandler.AddObject(Me, playersPanel, titlelab, 20, 20, 40, 160, "Players:")

        Dim playersIndices(3) As Label
        Dim playerFont As New Font("Arial", fp.objectHandler.ScaleDimension(15, Me.Width))

        For i As Integer = 0 To playersIndices.Length - 1
            playersIndices(i) = New Label
            playersIndices(i).Font = playerFont
            fp.objectHandler.AddObject(Me, playersPanel, playersIndices(i), 80 + i * 50, 20, 30, 40, i)

            playerNames(i) = New Label
            playerNames(i).Font = playerFont
            playerNames(i).BorderStyle = BorderStyle.FixedSingle
            playerNames(i).BackColor = Color.White
            fp.objectHandler.AddObject(Me, playersPanel, playerNames(i), 80 + i * 50, 60, 30, 320, "")
        Next
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

    Private Sub SetUsername(username As String)
        If username = "" Then
            WriteToLog(clientInfo, "Username is empty")
            Exit Sub
        End If
        For Each c As Char In username
            If Not Char.IsLetterOrDigit(c) Then
                WriteToLog(clientInfo, "Username must contain only characters 0..9, a..Z")
                Exit Sub
            End If
        Next
        dnsModel.SetUsername(username)
        WriteToLog(clientInfo, "Set username as " & username)
    End Sub
End Class