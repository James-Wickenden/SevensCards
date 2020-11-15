Imports System.Net

Public Class DNSModel
    Private dnsView As DNSView

    Public Sub New(menu As Menu)
        dnsView = New DNSView()
        dnsView.SetDNSModel(Me)
        dnsView.Show()
        menu.Close()

    End Sub

    Public Sub SetViewInModel(dnsView As DNSView)
        Me.dnsView = dnsView
    End Sub

    Public Sub StartServer()

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
    End Sub

    Private Sub ListDNS_Address(hostName As String, ipAddress As String)
        dnsView.WriteToLog(dnsView.serverInfo, "Name: " & hostName & " IP Address: " & ipAddress)
    End Sub
End Class
