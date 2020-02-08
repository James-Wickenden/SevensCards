Public Class Player_COM
    Inherits Player
    'Difficulty 0: The COM plays whatever valid card it comes across first.
    'Difficulty 1: The COM weighs cards towards the extremities more heavily, and plays to get rid of them above more central plays. 
    'Difficulty 2: The COM weighs cards like mode 1, but also weighs how many cards it can play towards one extremity and plays to that first.
    Private difficulty As Integer = 2

    Public Sub SetDifficulty(difficulty As Integer)
        Me.difficulty = difficulty
    End Sub

    Private Function GetValidCards() As Card()
        Dim res As New List(Of Card)
        For Each card As Card In hand.GetHand
            If card.GetValid Then
                res.Add(card)
            End If
        Next
        Return res.ToArray
    End Function

    Private Function PickCard_0() As Card
        Dim validCards() As Card = GetValidCards()
        If validCards.Length = 0 Then Return Nothing

        Randomize()
        Return validCards(Int((validCards.Count) * Rnd()))
    End Function

    Private Function PickCard_1() As Card
        Dim validCards() As Card = GetValidCards()
        If validCards.Length = 0 Then Return Nothing

        Dim bestWeight As Integer = 0
        Dim bestWeights As New List(Of Card)
        Dim weight As Integer
        For i As Integer = 0 To validCards.Length - 1
            weight = Math.Abs(validCards(i).GetValue - CardEnums.Value.SEVEN)
            If weight > bestWeight Then bestWeight = weight
        Next
        For i As Integer = 0 To validCards.Length - 1
            weight = Math.Abs(validCards(i).GetValue - CardEnums.Value.SEVEN)
            If weight = bestWeight Then bestWeights.Add(validCards(i))
        Next

        Return bestWeights(Int((bestWeights.Count) * Rnd()))
    End Function

    Private Function PickCard_2() As Card
        Dim validCards() As Card = GetValidCards()
        If validCards.Length = 0 Then Return Nothing

        Dim gapToNextCard(validCards.Length - 1), weights(validCards.Length - 1) As Integer
        Dim bestWeightIndex, bestWeight As Integer
        For i As Integer = 0 To validCards.Length - 1
            weights(i) = Math.Abs(validCards(i).GetValue - CardEnums.Value.SEVEN) + 2

            gapToNextCard(i) = -1
            For Each card As Card In hand.GetHand
                If (validCards(i).GetSuit = card.GetSuit) And (validCards(i).GetValue <> card.GetValue) Then
                    Dim cardDist As Integer = Math.Abs(validCards(i).GetValue - card.GetValue)
                    Dim validPairing As Boolean = False

                    If (validCards(i).GetValue < CardEnums.Value.SEVEN) And (card.GetValue < CardEnums.Value.SEVEN) Then validPairing = True
                    If (validCards(i).GetValue > CardEnums.Value.SEVEN) And (card.GetValue > CardEnums.Value.SEVEN) Then validPairing = True

                    If validPairing And (cardDist > gapToNextCard(i)) Then gapToNextCard(i) = cardDist
                End If
            Next
            weights(i) *= (gapToNextCard(i) + 2)
            If weights(i) > bestWeight Then
                bestWeightIndex = i
                bestWeight = weights(i)
            End If
        Next

        Return validCards(bestWeightIndex)
    End Function

    Private Function GetPlayedCard_COM() As Card
        Dim card As Card

        Select Case difficulty
            Case 0 : card = PickCard_0()
            Case 1 : card = PickCard_1()
            Case 2 : card = PickCard_2()
            Case Else : card = PickCard_0()
        End Select
        Threading.Thread.Sleep(300)
        Return card
    End Function

    Public Overrides Sub GetMove()
        While Not isMyMove
            If skipMove Then
                skipMove = False
                callback(Nothing)
            End If
        End While
        Dim card As Card = GetPlayedCard_COM()

        Try
            callback(card)
        Catch ex As Exception
        End Try

    End Sub
End Class
