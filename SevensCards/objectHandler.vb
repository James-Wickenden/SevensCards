Public Class ObjectHandler
    Public Sub AddObject(form As Form, obj As Object, top As Integer, left As Integer, height As Integer, width As Integer, text As String)
        obj.top = top
        obj.left = left
        obj.height = height
        obj.width = width
        obj.text = text
        form.Controls.Add(obj)
    End Sub

    Public Sub AddObject(panel As Panel, obj As Object, top As Integer, left As Integer, height As Integer, width As Integer, text As String)
        obj.top = top
        obj.left = left
        obj.height = height
        obj.width = width
        obj.text = text
        panel.Controls.Add(obj)
    End Sub

    Public Sub AddButton(form As Form, obj As Button, top As Integer, left As Integer, height As Integer, width As Integer, text As String, address As Action)
        AddObject(form, obj, top, left, height, width, text)
        obj.BackColor = Color.White
        AddHandler(obj.Click), AddressOf address.Invoke
    End Sub

    Public Sub AddButton(panel As Panel, obj As Button, top As Integer, left As Integer, height As Integer, width As Integer, text As String, address As Action)
        AddObject(panel, obj, top, left, height, width, text)
        obj.BackColor = Color.White
        AddHandler(obj.Click), AddressOf address.Invoke
    End Sub
End Class
