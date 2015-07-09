Public Class FrmMain
    'Matrix van 8 breed (0 tot en met 7) en 8 hoog (0 tot en met 7)
    Dim oRooster(7, 7) As Button
    Dim iTime As Long = 0
    Dim iUnturned
    Private Sub FrmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        For iRij As Integer = oRooster.GetLowerBound(0) To oRooster.GetUpperBound(0)
            For iKolom As Integer = oRooster.GetLowerBound(1) To oRooster.GetUpperBound(1)
                oRooster(iRij, iKolom) = New Button
                oRooster(iRij, iKolom).AutoSize = False
                oRooster(iRij, iKolom).Size = New Size(50, 50)
                oRooster(iRij, iKolom).TextAlign = ContentAlignment.MiddleCenter
                oRooster(iRij, iKolom).Font = New Font("Arial", 12, FontStyle.Bold)
                oRooster(iRij, iKolom).Name = "cLbl_" & iRij & "," & iKolom
                'voor herinterpretatie vatbaar
                'x,y locatie in het venster
                Dim iXLocatie, iYLocatie As Integer
                iXLocatie = (iRij * 50) + 10
                'debug.WriteLine(iXLocatie)
                iYLocatie = (iKolom * 50) + 10
                'debug.WriteLine(iYLocatie)
                oRooster(iRij, iKolom).Location = New Point(iYLocatie, iXLocatie)

                'kleur van de tegel
                oRooster(iRij, iKolom).BackColor = System.Drawing.Color.LightGray
                'text op de tegel (initieel leeg)
                oRooster(iRij, iKolom).Text = ""
                Me.Controls.Add(oRooster(iRij, iKolom))

                'Voor ieder klikbaar Label:
                'linker klik
                AddHandler oRooster(iRij, iKolom).Click, AddressOf FncOnLabelClick
            Next
        Next
        Timer1.Start()
        'verstop 10 mijnen
        FncVerstopMijnen()

    End Sub
    Private Sub FncVerstopMijnen()
        Dim rand As Random = New Random
        Dim iRij, iKolom As Integer
        For i As Integer = 0 To 9
            iRij = rand.Next(oRooster.GetLowerBound(0), oRooster.GetUpperBound(0))
            iKolom = rand.Next(oRooster.GetLowerBound(0), oRooster.GetUpperBound(0))
            If (oRooster(iRij, iKolom).Tag = -1) Then i -= 1

            oRooster(iRij, iKolom).Tag = -1
        Next
    End Sub
    Private Sub FncBerekenGetallen()
        For iRij As Integer = oRooster.GetLowerBound(0) To oRooster.GetUpperBound(0)
            For iKolom As Integer = oRooster.GetLowerBound(1) To oRooster.GetUpperBound(1)
                If (oRooster(iRij, iKolom).Tag <> -1) Then
                    oRooster(iRij, iKolom).Tag = FncGetNeighbourValues(iRij, iKolom)
                End If

            Next
        Next
    End Sub
    Private Function FncGetNeighbourValues(i As Integer, j As Integer)
        Dim iSom As Integer
        For iRij As Integer = i - 1 To i + 1
            For iKolom As Integer = j - 1 To j + 1
                If (FncCheckCoord(iRij, iKolom)) Then
                    If(oRooster(iRij,iKolom).Tag == -1) iSom += 1
                    End If
                End If
            Next
        Next
        Return iSom
    End Function
    Private Function FncCheckCoord(i As Integer, j As Integer)
        If (i < 0 Or i > 8) Then Return False
        If (j < 0 Or j > 8) Then Return False
        Return True
    End Function

    Private Sub FncOnLabelClick(ByVal sender As Button, ByVal e As MouseEventArgs)
        If (e.Button = Windows.Forms.MouseButtons.Left) Then
            If (sender.Text = "X") Then Return
            If (sender.Tag = -1) Then
                FncGameOver()
            Else
                sender.Text = sender.Tag
            End If
        ElseIf (e.Button = Windows.Forms.MouseButtons.Right) Then

        Else

        End If


    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        iTime += 1
        lblTime.Text = "Time: " & Int(iTime / 10) & "s"
    End Sub

    Private Sub FncGameOver()

    End Sub
End Class
