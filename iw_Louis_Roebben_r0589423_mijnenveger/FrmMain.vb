﻿Public Class FrmMain
    'debug mode
    Dim bDebug As Boolean = True

    '-- ONGELDIG --Matrix van 8 breed (0 tot en met 7) en 8 hoog (0 tot en met 7)
    'Matrix van 5 breed (o tot em met 4) en 5 hoog (0 tot em met 4)
    Dim oRooster(8, 8) As Button
    'tijd hoelang het spel bezig is
    Dim iTime As Double = 0
    'het aantal mijnen dat er verspopt mogen worden
    Dim iAantalMijnen As Integer = 4 ' 4 mijnen
    'het aan vlaggen dat er mogen gezet worden
    Dim iFlagged As Integer = iAantalMijnen
    'een bool of het spel over is
    Dim blnGamemover As Boolean = False
    'joke
    Dim bJoker As Boolean = True

    Private Sub FrmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'verwijderen van selectie hearder in de datagridview
        Me.DataGridView1.RowHeadersVisible = False
        'het initialiseren van het spelbord
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
                Debug.WriteLine(iXLocatie)
                iYLocatie = (iKolom * 50) + 10
                Debug.WriteLine(iYLocatie)
                oRooster(iRij, iKolom).Location = New Point(iYLocatie, iXLocatie)

                'kleur van de tegel
                oRooster(iRij, iKolom).BackColor = Color.LightGray
                'text op de tegel (initieel leeg)
                oRooster(iRij, iKolom).Text = ""
                Me.Controls.Add(oRooster(iRij, iKolom))

                'Voor ieder klikbaar Label:
                'linker klik
                AddHandler oRooster(iRij, iKolom).MouseDown, AddressOf FncOnLabelClick
            Next
        Next
        'start de spel timer
        Timer1.Start()
        'vraag mijnen 
        FncVraagMijnen()
        'verstop 10 mijnen
        lblMines.Text = "Mines Left: " & iFlagged
        FncVerstopMijnen()
        FncBerekenGetallen()
    End Sub

    Private Sub FncVraagMijnen()
        Dim ivoorstel
        'assert hoogte = breete !
        While (ivoorstel > oRooster.Length / 2 Or ivoorstel < 1)
opnieuw:    ivoorstel = InputBox("please give the number of mines", "mines:", "5")
            If Not IsNumeric(ivoorstel) Then
                GoTo opnieuw
            End If
        End While
        iAantalMijnen = ivoorstel
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
            'als debug aan staat -> verklap de verstopte mijnen met H
            If (bDebug) Then oRooster(iRij, iKolom).Text = "H"
            'als bovenstaande fout is verstop mijn
            oRooster(iRij, iKolom).Tag = -1
        Next
    End Sub
    Private Sub FncBerekenGetallen()
        'loop over het rooster
        For iRij As Integer = oRooster.GetLowerBound(0) To oRooster.GetUpperBound(0)
            For iKolom As Integer = oRooster.GetLowerBound(1) To oRooster.GetUpperBound(1)
                'als er op de locatie geen bom is verstopt -> bereken dan de som van de buuren die dat wel zijn
                If (oRooster(iRij, iKolom).Tag <> -1) Then
                    oRooster(iRij, iKolom).Tag = FncGetNeighbourValues(iRij, iKolom)
                End If

            Next
        Next
    End Sub
    Private Function FncGetNeighbourValues(i As Integer, j As Integer)
        Dim iSom As Integer
        'gegeven een start positie (i,j) -> x en y
        For iRij As Integer = i - 1 To i + 1
            For iKolom As Integer = j - 1 To j + 1
                'als de overgeitereerde coordinaat een geldige is -> kijk na op deze een mijn is -> als dat zo is som++
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
        '' de 
        If (i < 0 Or i > oRooster.GetUpperBound(0)) Then
            Return False
        ElseIf (j < 0 Or j > oRooster.GetUpperBound(1)) Then
            Return False
        Else
            Return True
        End If
    End Function

    Private Sub FncOnLabelClick(ByVal sender As Button, ByVal e As MouseEventArgs)
        'als gameover dan kan er niet meer geklikt worden
        If (blnGamemover) Then Return
        'als linkermuisknop klik
        If (e.Button = Windows.Forms.MouseButtons.Left) Then
            'als er een flag op staat -> mag er niet op geklikt worden
            If (sender.Text = "X?" Or sender.BackColor = Color.Red) Then Return
            'als er een mijn onder de knop ligt -> draai deze om en game over (verloren)
            If (sender.Tag = -1) Then
                sender.Text = "X"
                'als heeft nog joker
                If (bJoker) Then
                    bJoker = False
                    Application.DoEvents()
                    System.Threading.Thread.Sleep(3000)
                    MessageBox.Show("Joker used")
                    sender.BackColor = Color.Red
                Else
                    'anders game over
                    FncGameOver(False)
                End If
            Else
                'als er geen mijn onder de knop ligt -> draai de waarde om 
                sender.Text = sender.Tag
                'als der waarde van de knop 0 is dropageer de omdraaing
                If (sender.Tag = 0) Then
                    'bereken de locatie van de knop gebaseerd op de naam
                    Dim strSplit() As String = sender.Name.Split("_")
                    FncRevealNeighbours(strSplit(1), strSplit(2))
                End If
            End If
            'als er een rechterklik is
        ElseIf (e.Button = Windows.Forms.MouseButtons.Right) Then
            'als er nog niets op de knop staat en nog vlaggen overzijn -> zet dan een vlag op de knop
            If (sender.Text = "" And iFlagged > 0) Then                 ' en verminder het aantal te zetten vlaggen 
                sender.Text = "X?"
                iFlagged -= 1
                lblMines.Text = "Mines Left :" & iFlagged
                'als er al een vlag op de knop staat -> verwijder deze en aantal te zetten vlaggen++
            ElseIf (sender.Text = "X?") Then
                iFlagged += 1
                sender.Text = ""
            ElseIf (iFlagged <= 0) Then
                MessageBox.Show("No more flags to place")
            End If
            lblMines.Text = "Mines Left :" & iFlagged
        Else
            MessageBox.Show("invalid operation")
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
                    'check of dat er een flag op de knop staat -> als dat zo is asserten we dat deze eraf gaat gehaald worden
                    If (oRooster(iRij, iKolom).Text = "X?") Then 'dus voegen we 1 bij #vlageen te zetten en updaten we het label
                        iFlagged += 1
                        lblMines.Text = "Mines Left :" & iFlagged
                    End If
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
        lblTime.Text = "Time: " & Int(iTime) & "s"
    End Sub

    Private Sub FncGameOver(bWon As Boolean)
        'als game over -> stop timer
        blnGamemover = True
        Timer1.Stop()
        If (bWon = False) Then
            MessageBox.Show("Game Over, You Lost")
            Return
        End If
        FncCheckAndReplace(iTime)
        DataGridView1.Sort(DataGridView1.Columns(1), System.ComponentModel.ListSortDirection.Ascending)
    End Sub

    Private Sub FncCheckAndReplace(iScore As Double)
        'voeg de nieuwe score toe aan het hightscore systeem
        FncToHighScore(InputBox("please give your name", "Highscore", ""), iScore)
        'als er 3 of minder highscores zijn return
        If (DataSet1.tblHighScore.Rows.Count < 4) Then Return
        'als dit niet zo is ga dan na welke de leterlijk hoogste score heeft
        Dim currentHoogste As DataSet1.tblHighScoreRow = DataSet1.tblHighScore.Rows(0)
        currentHoogste.fldScore = Double.MaxValue
        For Each dr As DataSet1.tblHighScoreRow In DataSet1.tblHighScore.Rows
            If (dr.fldScore < currentHoogste.fldScore) Then currentHoogste = dr
        Next
        'verwijder de highscore met de leterlijk hoogste score
        currentHoogste.Delete()

    End Sub

    Private Sub FncToHighScore(strName As String, iScore As Integer)
        'basic
        Dim newHighScoreRow As DataRow = DataSet1.Tables("tblHighScore").NewRow()

        newHighScoreRow("fldName") = strName
        newHighScoreRow("fldScore") = iScore

        DataSet1.Tables("tblHighScore").Rows.Add(newHighScoreRow)
    End Sub

    Private Sub btnRestart_Click(sender As Object, e As EventArgs) Handles btnRestart.Click
        'voor voor mijnen
        FncVraagMijnen()

        'restart de game
        blnGamemover = False
        iTime = 0
        Timer1.Start()
        FncCleanVeld()
        'verstop 10 mijnen
        iFlagged = iAantalMijnen
        lblMines.Text = "Mines Left: " & iFlagged
        FncVerstopMijnen()
        FncBerekenGetallen()
    End Sub

    Private Sub FncCleanVeld()
        'basic
        For i As Integer = oRooster.GetLowerBound(0) To oRooster.GetUpperBound(0)
            For j As Integer = oRooster.GetLowerBound(1) To oRooster.GetUpperBound(0)
                oRooster(i, j).Tag = vbEmpty
                oRooster(i, j).Text = String.Empty
                oRooster(i, j).BackColor = Color.Gray
            Next
        Next
    End Sub

    Private Sub btnResetHighScore_Click(sender As Object, e As EventArgs) Handles btnResetHighScore.Click
        'basic
        DataSet1.tblHighScore.Rows.Clear()
        MessageBox.Show("Highscore table cleared")
    End Sub
End Class
