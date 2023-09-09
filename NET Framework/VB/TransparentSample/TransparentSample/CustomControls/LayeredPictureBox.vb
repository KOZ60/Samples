Imports System.ComponentModel

''' <summary>
''' レイヤードな PictureBox
''' Opacity プロパティと TransparencyKey プロパティを持ちます。
''' </summary>
''' <remarks>
''' アプリケーションマニフェストの SupportOS を Windows 8 以上にすると
''' 子ウインドウでもレイヤードウインドウが使えるようになります。
''' </remarks>
<DesignerCategory("CODE")>
Public Class LayeredPictureBox
    Inherits PictureBox

    Private IsLayered As Boolean
    Private _Opacity As Double = 1.0
    Private _TransparencyKey As Color = Color.Empty
    Private Const LayeredCatecory As String = "透過"

    Protected Overrides Sub OnHandleCreated(e As EventArgs)
        MyBase.OnHandleCreated(e)
        ' マニュフェストで support OS を Windows 8 にしていない場合
        ' CreateParams で WS_EX_LAYERED を適用すると異常終了する
        ' ウインドウ作成後 SetWindowLong で適用すると失敗しても異常終了しない
        IsLayered = Not DesignMode AndAlso SetLayeredStyle()
        SetAttributes()
    End Sub

    ''' <summary>
    ''' LayeredPictureBox の不透明度を取得または設定します。
    ''' </summary>
    ''' <returns>LayeredPictureBox の不透明度。 既定値は 1.00 です。</returns>
    <TypeConverter(GetType(OpacityConverter))>
    <DefaultValue(1.0)>
    <Category(LayeredCatecory)>
    Public Property Opacity As Double
        Get
            Return _Opacity
        End Get
        Set(value As Double)
            If value > 1.0R Then
                value = 1.0
            ElseIf value < 0 Then
                value = 0
            End If
            If Opacity <> value Then
                _Opacity = value
                SetAttributes()
            End If
        End Set
    End Property

    Private ReadOnly Property OpacityAsByte As Byte
        Get
            Return CByte(Opacity * 255)
        End Get
    End Property

    ''' <summary>
    ''' LayeredPictureBox の透明な領域を表す色を取得または設定します。
    ''' </summary>
    ''' <returns>LayeredPictureBox 上で透明色として表示される色を表す Color。</returns>
    <Category(LayeredCatecory)>
    Public Property TransparencyKey As Color
        Get
            Return _TransparencyKey
        End Get
        Set(value As Color)
            If TransparencyKey <> value Then
                _TransparencyKey = value
                SetAttributes()
            End If
        End Set
    End Property

    Friend Sub ResetTransparencyKey()
        TransparencyKey = Color.Empty
    End Sub

    Friend Function ShouldSerializeTransparencyKey() As Boolean
        Return Not TransparencyKey.IsEmpty
    End Function

    Private Sub SetAttributes()
        If IsLayered AndAlso IsHandleCreated Then
            Dim dwFlags = 0
            Dim crKey = 0
            Dim bAlpha As Byte = 0
            If OpacityAsByte < 255 Then
                dwFlags = dwFlags Or LWA_ALPHA
                bAlpha = OpacityAsByte
            End If
            If Not TransparencyKey.IsEmpty Then
                dwFlags = dwFlags Or LWA_COLORKEY
                crKey = ColorTranslator.ToWin32(TransparencyKey)
            End If
            SetLayeredWindowAttributes(Handle, crKey, bAlpha, dwFlags)
        End If
    End Sub

End Class
