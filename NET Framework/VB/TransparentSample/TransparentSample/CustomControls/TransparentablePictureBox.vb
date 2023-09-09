Imports System.ComponentModel
''' <summary>
''' BackColor に Color.Transparent を指定したとき背景を透過する PictureBox
''' </summary>
<DesignerCategory("CODE")>
Public Class TransparentablePictureBox
    Inherits PictureBox

    Protected Overrides Sub OnPaintBackground(pevent As PaintEventArgs)
        MyBase.OnPaintBackground(pevent)
        If BackColor = Color.Transparent Then
            DrawSameHierarchies(pevent.Graphics)
        End If
    End Sub

    ' 自分と同じ階層のコントロールを列挙して描画
    Private Sub DrawSameHierarchies(g As Graphics)
        Dim ctls = Parent.Controls
        ' Controls は Z オーダー順に並んでいるので後ろから列挙
        For i = ctls.Count - 1 To 0 Step -1
            Dim ctrl As Control = ctls(i)
            If ctrl Is Me Then Exit For ' 自分が出てきたら終了
            If ctrl.Visible Then
                DrawSameHierarchy(g, ctrl)
            End If
        Next
    End Sub

    Private Sub DrawSameHierarchy(g As Graphics, ctrl As Control)
        ' 描画対象と自身が交差する領域を取得
        Dim mapRect = ctrl.Parent.MapRectangle(ctrl.Bounds, Me)
        Dim clip = Rectangle.Intersect(mapRect, ClientRectangle)

        If clip.Width > 0 AndAlso clip.Height > 0 Then
            ' ビットマップにコントロールを描画
            Using bmp = ctrl.DrawToBitmap()
                ' 切り出し位置を計算
                ' 描画領域を親の座標に変換しコントロールの Location を差し引いたものが
                ' 切り出し位置となる
                Dim cutRect = MapRectangle(clip, ctrl.Parent)
                cutRect = New Rectangle(
                                cutRect.Location - CType(ctrl.Location, Size), clip.Size)

                ' 切り出して描画
                g.DrawImage(bmp, clip, cutRect, GraphicsUnit.Pixel)
            End Using
        End If
    End Sub

End Class
