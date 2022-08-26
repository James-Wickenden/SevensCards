Public Class ObjectHandler

    Public Sub AddObject(f As Form, surface As Object, obj As Object,
                         top As Integer, left As Integer, height As Integer, width As Integer,
                         text As String, Optional shouldScale As Boolean = True)
        If shouldScale Then ScaleObject(f, top, left, height, width)
        obj.top = top
        obj.left = left
        obj.height = height
        obj.width = width
        obj.text = text
        surface.Controls.Add(obj)
    End Sub

    Public Sub AddButton(f As Form, surface As Object, obj As Button,
                         top As Integer, left As Integer, height As Integer, width As Integer,
                         text As String, address As Action, Optional shouldScale As Boolean = True)
        If shouldScale Then ScaleObject(f, top, left, height, width)
        AddObject(f, surface, obj, top, left, height, width, text)
        obj.BackColor = Color.White
        AddHandler(obj.Click), AddressOf address.Invoke
    End Sub

    Public Function ScaleDimension(val As Integer, Optional form_width As Integer = -1, Optional form_height As Integer = -1) As Integer
        Dim scalingFactor As Double
        If form_height <> -1 Then scalingFactor = form_height / 1056
        If form_width <> -1 Then scalingFactor = form_width / 1936

        Return CInt(val * scalingFactor)
    End Function

    Private Sub ScaleObject(ByVal f As Form, ByRef top As Integer, ByRef left As Integer, ByRef height As Integer, ByRef width As Integer)
        Dim scalingFactorHeight As Double = f.Height / 1056
        Dim scalingFactorWidth As Double = f.Width / 1936

        top *= scalingFactorHeight
        height *= scalingFactorHeight
        left *= scalingFactorWidth
        width *= scalingFactorWidth
    End Sub

    Public Function AddTableLayoutPanel(parentObject As Object, name As String, rows As Integer, cols As Integer) As TableLayoutPanel
        Dim tlp As New TableLayoutPanel
        With tlp
            .Name = name
            .Margin = New System.Windows.Forms.Padding(0, 0, 0, 0)

            .ColumnCount = cols
            .RowCount = rows
            Dim colWidthProportion As Double = 100.0F / cols
            Dim rowWidthProportion As Double = 100.0F / rows
            For i As Integer = 0 To cols
                .ColumnStyles.Add(New ColumnStyle(SizeType.Percent, colWidthProportion))
            Next
            For i As Integer = 0 To rows
                .RowStyles.Add(New ColumnStyle(SizeType.Percent, rowWidthProportion))
            Next

            .Dock = DockStyle.Fill
            .AutoSizeMode = Windows.Forms.AutoSizeMode.GrowAndShrink
            .AutoSize = True
        End With
        parentObject.Controls.Add(tlp)

        Return tlp
    End Function
End Class
