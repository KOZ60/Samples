Imports System.Runtime.InteropServices

Public Class Form1

    Public Sub New()
        InitializeComponent()

        ComboBoxEx1.Items.Add("葛" & ChrW(&HDB40) & ChrW(&HDD00) & "城市")
        ComboBoxEx1.Items.Add("葛飾区")
        'Me.ComboBox1.Items.Add("葛󠄀城市")
        'Dim str As String = "葛󠄀城市"
        'For Each c In str
        '    Debug.Print(AscW(c))
        'Next
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim bmp As New Bitmap(ComboBoxEx1.Width, ComboBoxEx1.Height)
        ComboBoxEx1.DrawToBitmap(bmp, New Rectangle(New Point(0, 0), ComboBoxEx1.Size))
        PictureBox1.Image = bmp
    End Sub
End Class
