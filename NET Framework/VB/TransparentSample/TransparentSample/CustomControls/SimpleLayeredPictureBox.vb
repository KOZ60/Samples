Imports System.ComponentModel

''' <summary>
''' BackColor と同じ色を透過する PictureBox
''' </summary>
''' <remarks>
''' アプリケーションマニフェストの SupportOS を Windows 8 以上にすると
''' 子ウインドウでもレイヤードウインドウが使えるようになります。
''' </remarks>
<DesignerCategory("CODE")>
Public Class SimpleLayeredPictureBox
    Inherits PictureBox

    Private IsLayered As Boolean

    Public Sub New()
        ' BackColor を透過するので Color.Transparent を使用できなくする
        SetStyle(ControlStyles.SupportsTransparentBackColor, False)
    End Sub

    Protected Overrides Sub OnHandleCreated(e As EventArgs)
        MyBase.OnHandleCreated(e)
        ' マニュフェストで support OS を Windows 8 にしていない場合
        ' CreateParams で WS_EX_LAYERED を適用すると異常終了する
        ' ウインドウ作成後 SetWindowLong で適用すると失敗しても異常終了しない
        IsLayered = Not DesignMode AndAlso SetLayeredStyle()
        SetAttributes()
    End Sub

    Private Sub SetAttributes()
        If IsLayered AndAlso IsHandleCreated Then
            SetLayeredWindowAttributes(Handle,
                                       ColorTranslator.ToWin32(BackColor),
                                       0, LWA_COLORKEY)
        End If
    End Sub

    Protected Overrides Sub OnBackColorChanged(e As EventArgs)
        MyBase.OnBackColorChanged(e)
        SetAttributes()
    End Sub

End Class
