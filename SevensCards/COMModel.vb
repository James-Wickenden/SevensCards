Public Class COMModel
    'The COM should prioritise playing outer cards over inner ones
    'At higher difficulties, the COM should put higher importance on cards with more extreme values 
    '   ie a player with aces in a suit should try to play towards those aces
    '   a player with only mid values in a suit should try and keep those cards spare to impede others
    'At lower difficulties, the COM should play randomly.

    Private difficulty As Integer
    Private hand As List(Of Card)

    Public Sub New(difficulty As Integer)
        Me.difficulty = difficulty

    End Sub

End Class
