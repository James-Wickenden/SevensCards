Public Class ObjectHandler

    Public Sub AddObject(surface As Object, obj As Object, top As Integer, left As Integer, height As Integer, width As Integer, text As String)
        obj.top = top
        obj.left = left
        obj.height = height
        obj.width = width
        obj.text = text
        surface.Controls.Add(obj)
    End Sub

    Public Sub AddButton(surface As Object, obj As Button, top As Integer, left As Integer, height As Integer, width As Integer, text As String, address As Action)
        AddObject(surface, obj, top, left, height, width, text)
        obj.BackColor = Color.White
        AddHandler(obj.Click), AddressOf address.Invoke
    End Sub

End Class
