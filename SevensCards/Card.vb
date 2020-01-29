Public Class Card
    Implements IComparable

    Private suit As CardEnums.Suit
    Private value As CardEnums.Value
    Private view, validBar As New Panel
    Private frontImg, backImg As Image
    Private valid, faceUp As Boolean
    Private adjCard As Card

    Public Sub New(suit As CardEnums.Suit, value As CardEnums.Value, valid As Boolean, Optional adjCard As Card = Nothing)
        Me.suit = suit
        Me.value = value
        Me.adjCard = adjCard
        setValid(valid)

        frontImg = Image.FromFile((New FunctionPool).PATH & "Resources\" & "cards\" & getCardText() & ".jpg")
        backImg = My.Resources.blue_back
        view.BackgroundImage = backImg
        validBar.BackColor = Color.Gray
        view.BackgroundImageLayout = ImageLayout.Stretch
        view.BorderStyle = BorderStyle.FixedSingle
        validBar.BorderStyle = BorderStyle.FixedSingle
    End Sub

    Public Sub setValid(valid As Boolean)
        Me.valid = valid
        If faceUp Then
            If Not valid Then validBar.BackColor = Color.Red
            If valid Then validBar.BackColor = Color.Green
        End If
    End Sub

    Public Function getSuit() As CardEnums.Suit
        Return suit
    End Function
    Public Function getValue() As CardEnums.Value
        Return value
    End Function
    Public Function getValid() As Boolean
        Return valid
    End Function
    Public Function getFaceUp() As Boolean
        Return faceUp
    End Function
    Public Function getView() As Panel
        Return view
    End Function
    Public Function getValidBar() As Panel
        Return validBar
    End Function
    Public Function getadjCard() As Card
        Return adjCard
    End Function
    Public Sub setadjCard(c As Card)
        adjCard = c
    End Sub
    Public Function getCardText() As String
        Dim res As String = ""

        Select Case value
            Case CardEnums.Value.ACE : res = "A"
            Case CardEnums.Value.JACK : res = "J"
            Case CardEnums.Value.QUEEN : res = "Q"
            Case CardEnums.Value.KING : res = "K"
            Case Else : res = (value + 1)
        End Select
        Select Case suit
            Case CardEnums.Suit.DIAMOND : res &= "D"
            Case CardEnums.Suit.HEART : res &= "H"
            Case CardEnums.Suit.CLUB : res &= "C"
            Case CardEnums.Suit.SPADE : res &= "S"
        End Select
        Return res
    End Function

    Public Sub flip()
        If faceUp Then
            faceUp = False
            view.BackgroundImage = backImg
            validBar.BackColor = Color.Gray
        Else
            faceUp = True
            view.BackgroundImage = frontImg
            If valid Then validBar.BackColor = Color.Green
            If Not valid Then validBar.BackColor = Color.Red
        End If

    End Sub

    Private Function compare(x As Card) As Integer
        If x.getSuit = suit And x.getValue = value Then Return 0
        If x.getSuit <> suit Then Return (x.getSuit > suit)
        Return (x.getValue > value)
    End Function

    Private Function IComparable_CompareTo(obj As Object) As Integer Implements IComparable.CompareTo
        If Me Is Nothing Or obj Is Nothing Then
            Return 0
        Else Return compare(obj)
        End If
    End Function
End Class
