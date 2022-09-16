Public Class Form1

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Task.Run(Sub()
                     Application.Run(New Form1())
                 End Sub)
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim bmp As New Bitmap(ComboBoxEx1.Width, ComboBoxEx1.Height)
        ComboBoxEx1.DrawToBitmap(bmp, New Rectangle(Point.Empty, bmp.Size))
        If PictureBox1.Image IsNot Nothing Then
            PictureBox1.Image.Dispose()
        End If
        PictureBox1.Image = bmp
    End Sub

End Class
