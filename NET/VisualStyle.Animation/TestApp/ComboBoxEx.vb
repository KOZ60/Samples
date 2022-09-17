Option Strict On

Imports System.Runtime.InteropServices
Imports System.Windows.Forms.VisualStyles
Imports VisualStyle.Animation

Public Class ComboBoxEx
    Inherits ComboBox

    <ThreadStatic>
    Private Shared vsRenderer As VisualStyleRenderer

    Private WithEvents Controller As AnimationController

    Public Sub New()
        Controller = New AnimationController(Me)
    End Sub

    Protected Overrides Sub CreateHandle()
        MyBase.CreateHandle()
        Controller.Enabled = Not DesignMode AndAlso
                            Application.RenderWithVisualStyles AndAlso
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

    Private Sub Controller_QueryCurrentState(sender As Object, e As QueryCurrentStateEventArgs) Handles Controller.QueryCurrentState
        If Enabled Then
            If DroppedDown Then
                e.State = AnimationState.Pressed
            Else
                If Controller.MouseIsOver Then
                    e.State = AnimationState.Hot
                Else
                    e.State = AnimationState.Normal
                End If
            End If
        Else
            e.State = AnimationState.Disabled
        End If
    End Sub

    Private Sub Controller_QueryDuration(sender As Object, e As QueryDurationEventArgs) Handles Controller.QueryDuration
        Dim newElement As VisualStyleElement = ToDropDownElement(e.NewState)
        Dim oldElemment As VisualStyleElement = ToDropDownElement(e.OldState)
        e.Duration = ThemeData.GetThemeTransitionDuration(oldElemment, newElement)
    End Sub

    Private Sub Controller_DrawControl(sender As Object, e As DrawControlEventArgs) Handles Controller.DrawControl

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
        Dim textColor As Color = If(Enabled, ForeColor, SystemColors.GrayText)
        TextRenderer.DrawText(e.Graphics, Text, Font, textFace, textColor,
                            TextFormatFlags.Left Or TextFormatFlags.VerticalCenter)

        ' フォーカスを示す四角形を描画

        If ContainsFocus AndAlso (Not DroppedDown) AndAlso ShowFocusCues Then
            ControlPaint.DrawFocusRectangle(e.Graphics, textFace)
        End If
    End Sub

    Private Function ToButtonElement(state As AnimationState) As VisualStyleElement
        Select Case state
            Case AnimationState.Disabled
                Return VisualStyleElement.Button.PushButton.Disabled
            Case AnimationState.Hot
                Return VisualStyleElement.Button.PushButton.Hot
            Case AnimationState.Normal
                Return VisualStyleElement.Button.PushButton.Normal
            Case AnimationState.Pressed
                Return VisualStyleElement.Button.PushButton.Pressed
        End Select
        Return VisualStyleElement.Button.PushButton.Normal
    End Function

    Private Function ToDropDownElement(state As AnimationState) As VisualStyleElement
        Select Case state
            Case AnimationState.Disabled
                Return VisualStyleElement.ComboBox.DropDownButton.Disabled
            Case AnimationState.Hot
                Return VisualStyleElement.ComboBox.DropDownButton.Hot
            Case AnimationState.Normal
                Return VisualStyleElement.ComboBox.DropDownButton.Normal
            Case AnimationState.Pressed
                Return VisualStyleElement.ComboBox.DropDownButton.Pressed
        End Select
        Return VisualStyleElement.ComboBox.DropDownButton.Normal
    End Function

End Class
