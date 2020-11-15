Public Class DNSModel
    Private dnsView As DNSView

    Public Sub New(menu As Menu)
        dnsView = New DNSView()
        dnsView.SetDNSModel(Me)
        dnsView.Show()
        menu.Close()

    End Sub


End Class
