Imports System.Net

Public Class DNSModel
    Private dnsView As DNSView
    Private wc As WebController

    Public Sub New(menu As Menu)
        dnsView = New DNSView()
        dnsView.SetDNSModel(Me)
        dnsView.Show()
        menu.Close()
    End Sub

    Public Sub StartServer()
        wc = New WebController(False, Me)
        Dim started As Boolean = wc.StartServer()
        If started Then dnsView.WriteToLog(dnsView.serverInfo, "Server started successfully.")
    End Sub

    Public Sub BeginGame()
        dnsView.WriteToLog(dnsView.serverInfo, "Beginning game...")
    End Sub

    Public Sub ClientConnect(ipStr As String)
        wc = New WebController(True, Me)
        Dim connected As Boolean = wc.Connect(ipStr)
        If Not connected Then dnsView.WriteToLog(dnsView.clientInfo, "Failed to connect.")
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
