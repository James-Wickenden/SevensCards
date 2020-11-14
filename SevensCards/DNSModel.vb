Public Class DNSModel


    Public Sub New(menu As Menu)
        Dim DNSView = New DNSView()
        DNSView.Show()
        menu.Close()


    End Sub


End Class
