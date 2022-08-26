Public Class ObjectHandler

    Public Sub AddObject(f As Form, surface As Object, obj As Object,
                         top As Integer, left As Integer, height As Integer, width As Integer,
                         text As String)
        ScaleObject(f, top, left, height, width)
        obj.top = top
        obj.left = left
        obj.height = height
        obj.width = width
        obj.text = text
        surface.Controls.Add(obj)
    End Sub

    Public Sub AddButton(f As Form, surface As Object, obj As Button,
                         top As Integer, left As Integer, height As Integer, width As Integer,
                         text As String, address As Action)
        ScaleObject(f, top, left, height, width)
        AddObject(f, surface, obj, top, left, height, width, text)
        obj.BackColor = Color.White
        AddHandler(obj.Click), AddressOf address.Invoke
    End Sub

    Private Sub ScaleObject(ByVal f As Form, ByRef top As Integer, ByRef left As Integer, ByRef height As Integer, ByRef width As Integer)
        Dim scalingFactorHeight As Double = f.Height / 1056
        Dim scalingFactorWidth As Double = f.Width / 1936

        top *= scalingFactorHeight
        height *= scalingFactorHeight
        left *= scalingFactorWidth
        width *= scalingFactorWidth
    End Sub
End Class
