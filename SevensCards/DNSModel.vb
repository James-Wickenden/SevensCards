Imports System.Net

Public Class DNSModel
    Private dnsView As DNSView
    Private wc As WebController
    Private username As String = "test"

    Public Sub New(menu As Menu)
        dnsView = New DNSView()
        dnsView.SetDNSModel(Me)
        dnsView.Show()
        menu.Close()
    End Sub

    Public Sub StartServer()
        wc = New WebController(False, Me)
        Dim started As Boolean = wc.StartServer()
        If started Then dnsView.WriteToLog(dnsView.serverInfo, "Server started successfully. Set host username in client panel!")
    End Sub

    Public Sub BeginGame()
        dnsView.WriteToLog(dnsView.serverInfo, "Beginning game...")
    End Sub

    Public Sub SetUsername(username As String)
        Me.username = username
    End Sub

    Public Sub ClientConnect(sender As Button, ipStr As String)
        If sender.Text = "Disconnect" Then
            ClientDisconnect(sender)
            Exit Sub
        End If

        If username Is Nothing Then
            WriteToLog("Set a username first (only alphanumerics)", True)
            Exit Sub
        Else
            Dim valid As Boolean = System.Text.RegularExpressions.Regex.IsMatch(username, "^[a-zA-Z0-9]*$")
            If Not valid Then
                WriteToLog("Illegal character; only alphanumerics allowed", True)
                Exit Sub
            End If

            If wc IsNot Nothing Then
                If wc.GetIsConnected Then
                    WriteToLog("Already connected to a server", True)
                    Exit Sub
                End If
            End If
        End If

        wc = New WebController(True, Me)
        Dim connected As Boolean = wc.Connect(ipStr)
        If Not connected Then
            dnsView.WriteToLog(dnsView.clientInfo, "Failed to connect.")
            Exit Sub
        End If

        sender.Text = "Disconnect"
        wc.SendToServer("USERNAME:" & username)
    End Sub

    Private Sub ClientDisconnect(sender As Button)
        If Not wc.GetIsConnected Then Exit Sub
        If Not wc.Disconnect() Then Exit Sub

        sender.Text = "Connect"
    End Sub

    Public Sub WriteToLog(msg As String, isClient As Boolean)
        If isClient Then
            dnsView.WriteToLog(dnsView.clientInfo, msg)
        Else
            dnsView.WriteToLog(dnsView.serverInfo, msg)
        End If
    End Sub

    Public Sub ListDNS_Addresses(Optional Verbose As Boolean = True)
        Dim hostName As String = System.Net.Dns.GetHostName()
        Dim addresses As IPAddress() = Dns.GetHostEntry(hostName).AddressList()

        If Verbose Then dnsView.WriteToLog(dnsView.serverInfo, "All Machine IP Addresses: ")
        For Each hostAdr As IPAddress In addresses
            If hostAdr.ToString().Contains("192.168") Then
                ListDNS_Address(hostName, hostAdr.ToString())
                dnsView.WriteToLog(dnsView.serverInfo, "    [Local Network IP]")
            Else
                If Verbose Then
                    ListDNS_Address(hostName, hostAdr.ToString())
                    If hostAdr.AddressFamily.ToString() = "InterNetwork" Then
                        dnsView.WriteToLog(dnsView.serverInfo, "    [Potential host IP]")
                    End If
                End If

            End If
        Next

        dnsView.WriteToLog(dnsView.serverInfo, "Set to run with port 65535")
    End Sub

    Private Sub ListDNS_Address(hostName As String, ipAddress As String)
        dnsView.WriteToLog(dnsView.serverInfo, "Name: " & hostName & " IP Address: " & ipAddress)
    End Sub
End Class
