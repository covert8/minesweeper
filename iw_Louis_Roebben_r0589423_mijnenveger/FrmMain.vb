Public Class FrmMain
    'debug mode
    Dim bDebug As Boolean = True


    'Matrix van 8 breed (0 tot en met 7) en 8 hoog (0 tot en met 7)
    Dim oRooster(7, 7) As Button
    Dim iTime As Double = 0
    Dim iAantalMijnen As Integer = 10
    Dim iFlagged As Integer = iAantalMijnen
    Dim blnGamemover As Boolean = False

    Private Sub FrmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        For iRij As Integer = oRooster.GetLowerBound(0) To oRooster.GetUpperBound(0)
            For iKolom As Integer = oRooster.GetLowerBound(1) To oRooster.GetUpperBound(1)
                oRooster(iRij, iKolom) = New Button
                oRooster(iRij, iKolom).AutoSize = False
                oRooster(iRij, iKolom).Size = New Size(50, 50)
                oRooster(iRij, iKolom).TextAlign = ContentAlignment.MiddleCenter
                oRooster(iRij, iKolom).Font = New Font("Arial", 12, FontStyle.Bold)
                oRooster(iRij, iKolom).Name = "cLbl_" & iRij & "_" & iKolom
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
                AddHandler oRooster(iRij, iKolom).MouseDown, AddressOf FncOnLabelClick
            Next
        Next
        Timer1.Start()
        'verstop 10 mijnen
        lblMines.Text = "Mines Left: " & iFlagged
        FncVerstopMijnen()
        FncBerekenGetallen()
    End Sub
    Private Sub FncVerstopMijnen()
        Dim rand As Random = New Random
        Dim iRij, iKolom As Integer
        'creer #mijnen
        For i As Integer = 0 To iAantalMijnen - 1
            iRij = rand.Next(oRooster.GetLowerBound(0), oRooster.GetUpperBound(0))
            iKolom = rand.Next(oRooster.GetLowerBound(0), oRooster.GetUpperBound(0))
            'als gekozen plaats al een mijn heeft verhoog het aantal te verstoppen mijnen met 1
            If (oRooster(iRij, iKolom).Tag = -1) Then i -= 1
            'als bovenstaande fout is verstop mijn
            If (bDebug) Then oRooster(iRij, iKolom).Text = "H"
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
                    If (oRooster(iRij, iKolom).Tag = -1) Then
                        iSom += 1
                    End If
                End If
            Next
        Next
        Return iSom
    End Function
    Private Function FncCheckCoord(i As Integer, j As Integer)
        If (i < 0 Or i > 7) Then
            Return False
        ElseIf (j < 0 Or j > 7) Then
            Return False
        Else
            Return True
        End If
    End Function

    Private Sub FncOnLabelClick(ByVal sender As Button, ByVal e As MouseEventArgs)
        'game over
        If (blnGamemover) Then Return

        If (e.Button = Windows.Forms.MouseButtons.Left) Then
            If (sender.Text = "X?") Then Return
            If (sender.Tag = -1) Then
                sender.Text = "X"
                FncGameOver(False)
            Else
                sender.Text = sender.Tag
                If (sender.Tag = 0) Then
                    Dim strSplit() As String = sender.Name.Split("_")
                    FncRevealNeighbours(strSplit(1), strSplit(2))
                End If

            End If
        ElseIf (e.Button = Windows.Forms.MouseButtons.Right) Then
            If (sender.Text = "" And iFlagged > 0) Then
                sender.Text = "X?"
                iFlagged -= 1
                lblMines.Text = "Mines Left :" & iFlagged
            ElseIf (sender.Text = "X?") Then
                iFlagged += 1
                sender.Text = ""
            Else
                MessageBox.Show("Too many flags")
            End If
            lblMines.Text = "Mines Left :" & iFlagged
        Else
            MessageBox.Show("invalid button")
        End If

        'check if won
        FncCheckWinState()

    End Sub
    Private Sub FncCheckWinState()
        Dim iUnturned As Integer = oRooster.Length
        For Each bt As Button In oRooster
            If (bt.Text <> bt.Tag.ToString) Then iUnturned -= 1
        Next
        If (iUnturned = oRooster.Length - iAantalMijnen) Then FncGameOver(True)
    End Sub
    Private Sub FncRevealNeighbours(i As Integer, j As Integer)
        'als aanliggende vakken 0 zijn -> draai ze dan om 
        Dim iSom As Integer
        For iRij As Integer = i - 1 To i + 1
            For iKolom As Integer = j - 1 To j + 1
                If (FncCheckCoord(iRij, iKolom)) Then
                    'als het geselecteerde vak 0 is en nog niet omgedraait -> draait deze om en doe hetzelfde voor dit vak
                    If (oRooster(iRij, iKolom).Tag = 0 And oRooster(iRij, iKolom).Text <> "0") Then
                        oRooster(iRij, iKolom).Text = 0
                        FncRevealNeighbours(iRij, iKolom)
                        'als dit niet zo is draai deze om
                    ElseIf (oRooster(iRij, iKolom).Tag > 0) Then
                        oRooster(iRij, iKolom).Text = oRooster(iRij, iKolom).Tag
                    End If
                End If
            Next
        Next
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        iTime += 0.1
        lblTime.Text = "Time: " & iTime & "s"
    End Sub

    Private Sub FncGameOver(bWon As Boolean)
        blnGamemover = True
        Timer1.Stop()
        MessageBox.Show("The game has ended")
        If (bWon = False) Then Return
        FncCheckAndReplace(iTime)
        DataGridView1.Sort(DataGridView1.Columns(1), System.ComponentModel.ListSortDirection.Ascending)
    End Sub

    Private Sub FncCheckAndReplace(iScore As Double)
        FncToHighScore(InputBox("please give your name", "Highscore", ""), iScore)
        If (DataSet1.tblHighScore.Rows.Count < 4) Then Return
        Dim currentHoogste As DataSet1.tblHighScoreRow = DataSet1.tblHighScore.Rows(0)
        currentHoogste.fldScore = Double.MaxValue
        For Each dr As DataSet1.tblHighScoreRow In DataSet1.tblHighScore.Rows
            If (dr.fldScore < currentHoogste.fldScore) Then currentHoogste = dr
        Next
        currentHoogste.Delete()

    End Sub

    Private Sub FncToHighScore(strName As String, iScore As Integer)


        Dim newHighScoreRow As DataRow = DataSet1.Tables("tblHighScore").NewRow()

        newHighScoreRow("fldName") = strName
        newHighScoreRow("fldScore") = iScore

        DataSet1.Tables("tblHighScore").Rows.Add(newHighScoreRow)
    End Sub

    Private Sub btnRestart_Click(sender As Object, e As EventArgs) Handles btnRestart.Click
        blnGamemover = False
        iTime = 0
        Timer1.Start()
        FncCleanVeld()
        'verstop 10 mijnen
        iFlagged = iAantalMijnen
        lblMines.Text = "Mines Left: " & iFlagged
        FncVerstopMijnen()
        FncBerekenGetallen()
        'order dataset
    End Sub

    Private Sub FncCleanVeld()
        For i As Integer = oRooster.GetLowerBound(0) To oRooster.GetUpperBound(0)
            For j As Integer = oRooster.GetLowerBound(1) To oRooster.GetUpperBound(0)
                oRooster(i, j).Tag = vbEmpty
                oRooster(i, j).Text = String.Empty
            Next
        Next
    End Sub

    Private Sub btnResetHighScore_Click(sender As Object, e As EventArgs) Handles btnResetHighScore.Click
        DataSet1.tblHighScore.Rows.Clear()
        MessageBox.Show("Highscore table cleared")
    End Sub
End Class
