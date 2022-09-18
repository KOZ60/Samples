Imports System.Runtime.InteropServices

Public Class Form1

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim bmp As New Bitmap(ComboBoxEx1.Width, ComboBoxEx1.Height)
        ComboBoxEx1.DrawToBitmap(bmp, New Rectangle(New Point(0, 0), ComboBoxEx1.Size))
        PictureBox1.Image = bmp
    End Sub
End Class
