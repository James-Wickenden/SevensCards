Public Class FunctionPool
    Public PATH As String = AppDomain.CurrentDomain.BaseDirectory
    Public Delegate Sub moveToBeMade(card As Card)
    Public objectHandler As New ObjectHandler

    Public Enum Mode
        OFFLINE
        ONLINE
        HUM
        COM
    End Enum

    Public Sub FormSetup(form As Form, text As String)
        form.Text = "SEVENS /// " & text
        form.Icon = My.Resources.icon
        form.WindowState = FormWindowState.Maximized
        form.BackColor = Color.DimGray
    End Sub

End Class
