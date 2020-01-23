Public Class FunctionPool
    Public PATH As String = AppDomain.CurrentDomain.BaseDirectory
    Public objectHandler As New ObjectHandler

    Public Sub formSetup(form As Form)
        'Me.Icon = My.Resources.icon
        form.WindowState = FormWindowState.Maximized
        form.BackColor = Color.DimGray

    End Sub

End Class
