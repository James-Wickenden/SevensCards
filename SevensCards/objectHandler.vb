﻿Public Class ObjectHandler
    Public Sub addObject(form As Form, obj As Object, top As Integer, left As Integer, height As Integer, width As Integer, text As String)
        obj.top = top
        obj.left = left
        obj.height = height
        obj.width = width
        obj.text = text
        obj.backColor = Color.White
        form.Controls.Add(obj)
    End Sub

    Public Sub addButton(form As Form, obj As Button, top As Integer, left As Integer, height As Integer, width As Integer, text As String, address As Action)
        addObject(form, obj, top, left, height, width, text)
        AddHandler(obj.Click), AddressOf address.Invoke
    End Sub
End Class