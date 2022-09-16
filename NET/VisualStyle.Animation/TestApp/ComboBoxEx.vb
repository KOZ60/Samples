Imports System.Windows.Forms.VisualStyles
Imports VisualStyle.Animation

Public Class ComboBoxEx
    Inherits ComboBox

    <ThreadStatic>
    Private Shared vsRenderer As VisualStyleRenderer

    Private WithEvents Controller As AnimationController(Of PushButtonState)

    Public Sub New()
        Controller = New AnimationController(Of PushButtonState)(Me)
    End Sub

    Protected Overrides Sub CreateHandle()
        MyBase.CreateHandle()
        Controller.Enabled = Application.RenderWithVisualStyles AndAlso
                            DropDownStyle = ComboBoxStyle.DropDownList AndAlso
                            FlatStyle = FlatStyle.Standard
    End Sub

    Protected Overrides Sub OnDropDown(e As EventArgs)
        MyBase.OnDropDown(e)
        Controller.CheckCurrentState()
    End Sub

    Protected Overrides Sub OnDropDownClosed(e As EventArgs)
        MyBase.OnDropDownClosed(e)
        Controller.CheckCurrentState()
    End Sub

    Private Sub Controller_QueryCurrentState(sender As Object, e As QueryCurrentStateEventArgs(Of PushButtonState)) Handles Controller.QueryCurrentState
        If Enabled Then
            If DroppedDown Then
                e.State = PushButtonState.Pressed
            Else
                Dim pt As Point = PointToClient(Control.MousePosition)
                If ClientRectangle.Contains(pt) Then
                    e.State = PushButtonState.Hot
                Else
                    e.State = PushButtonState.Normal
                End If
            End If
        Else
            e.State = PushButtonState.Disabled
        End If
    End Sub

    Private Sub Controller_QueryDuration(sender As Object, e As QueryDurationEventArgs(Of PushButtonState)) Handles Controller.QueryDuration
        Dim newElement As VisualStyleElement = ToDropDownElement(e.NewState)
        Dim oldElemment As VisualStyleElement = ToDropDownElement(e.OldState)
        e.Duration = ThemeData.GetThemeTransitionDuration(oldElemment, newElement)
    End Sub

    Private Sub Controller_DrawControl(sender As Object, e As DrawControlEventArgs(Of PushButtonState)) Handles Controller.DrawControl

        ' VisualStyle の PushButton をコントロールの全面に描画

        Dim buttonElement As VisualStyleElement = ToButtonElement(e.State)
        If vsRenderer Is Nothing Then
            vsRenderer = New VisualStyleRenderer(buttonElement)
        End If

        Dim controlRect As Rectangle = ClientRectangle
        controlRect.Inflate(1, 1)
        vsRenderer.SetParameters(buttonElement)
        If vsRenderer.IsBackgroundPartiallyTransparent() Then
            vsRenderer.DrawParentBackground(e.Graphics, controlRect, Me)
        End If
        vsRenderer.DrawBackground(e.Graphics, controlRect)

        ' VisualStyle の DropDownButton をコントロールの右側に描画(RightToLeft は考慮しない)

        Dim dropdownWidth As Integer = SystemInformation.HorizontalScrollBarArrowWidth
        Dim dropdownRect As New Rectangle(controlRect.Right - dropdownWidth, controlRect.Top, dropdownWidth, controlRect.Height)
        Dim dropdownFace As Rectangle = Rectangle.Inflate(dropdownRect, -2, -2)
        Dim dropdownElement As VisualStyleElement = ToDropDownElement(e.State)
        vsRenderer.SetParameters(dropdownElement)
        vsRenderer.DrawBackground(e.Graphics, dropdownRect, dropdownFace)

        ' テキストを描画

        Dim textFace As Rectangle = ClientRectangle
        textFace.Width -= dropdownWidth
        textFace.Inflate(-3, -3)
        TextRenderer.DrawText(e.Graphics, Text, Font, textFace, ForeColor,
                            TextFormatFlags.Left Or TextFormatFlags.VerticalCenter)

        ' フォーカスを示す四角形を描画

        If ContainsFocus AndAlso (Not DroppedDown) AndAlso ShowFocusCues Then
            ControlPaint.DrawFocusRectangle(e.Graphics, textFace)
        End If
    End Sub

    Private Function ToButtonElement(state As PushButtonState) As VisualStyleElement
        Select Case state
            Case PushButtonState.Disabled
                Return VisualStyleElement.Button.PushButton.Disabled
            Case PushButtonState.Hot
                Return VisualStyleElement.Button.PushButton.Hot
            Case PushButtonState.Normal
                Return VisualStyleElement.Button.PushButton.Normal
            Case PushButtonState.Pressed
                Return VisualStyleElement.Button.PushButton.Pressed
        End Select
        Return VisualStyleElement.Button.PushButton.Normal
    End Function

    Private Function ToDropDownElement(state As PushButtonState) As VisualStyleElement
        Select Case state
            Case PushButtonState.Disabled
                Return VisualStyleElement.ComboBox.DropDownButton.Disabled
            Case PushButtonState.Hot
                Return VisualStyleElement.ComboBox.DropDownButton.Hot
            Case PushButtonState.Normal
                Return VisualStyleElement.ComboBox.DropDownButton.Normal
            Case PushButtonState.Pressed
                Return VisualStyleElement.ComboBox.DropDownButton.Pressed
        End Select
        Return VisualStyleElement.ComboBox.DropDownButton.Normal
    End Function

End Class
