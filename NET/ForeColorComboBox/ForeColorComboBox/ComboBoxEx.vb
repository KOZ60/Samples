Option Strict On

Imports System.Runtime.InteropServices
Imports System.Windows.Forms.VisualStyles
Imports ButtonElement = System.Windows.Forms.VisualStyles.VisualStyleElement.Button.PushButton
Imports DropDownElement = System.Windows.Forms.VisualStyles.VisualStyleElement.ComboBox.DropDownButton

Public Class ComboBoxEx
    Inherits ComboBox

    <ThreadStatic>
    Private Shared _VsRenderer As VisualStyleRenderer
    Private Shared ReadOnly Property VsRenderer As VisualStyleRenderer
        Get
            If _VsRenderer Is Nothing Then
                _VsRenderer = New VisualStyleRenderer(ButtonElement.Default)
            End If
            Return _VsRenderer
        End Get
    End Property

    Private oldState As ComboBoxState = ComboBoxState.Normal

    Public Sub New()
        AddHandler EnabledChanged, AddressOf CheckCurrentState
        AddHandler GotFocus, AddressOf CheckCurrentState
        AddHandler LostFocus, AddressOf CheckCurrentState
        AddHandler DropDown, AddressOf CheckCurrentState
        AddHandler DropDownClosed, AddressOf CheckCurrentState
        AddHandler MouseCaptureChanged, AddressOf CheckCurrentState
        AddHandler MouseEnter, AddressOf CheckCurrentState
        AddHandler MouseLeave, AddressOf CheckCurrentState
        AddHandler MouseDown, AddressOf CheckCurrentState
        AddHandler MouseUp, AddressOf CheckCurrentState
    End Sub

    Private Sub CheckCurrentState(sender As Object, e As EventArgs)
        If UseAnimation AndAlso IsHandleCreated AndAlso oldState <> GetCurrentState() Then
            Invalidate()
        End If
    End Sub

    Private Function GetCurrentState() As ComboBoxState
        If Enabled Then
            If DroppedDown Then
                Return ComboBoxState.Pressed
            ElseIf MouseIsOver Then
                Return ComboBoxState.Hot
            Else
                Return ComboBoxState.Normal
            End If
        Else
            Return ComboBoxState.Disabled
        End If
    End Function

    Protected Overrides Sub OnHandleCreated(e As EventArgs)
        NativeMethods.BufferedPaintInit()
        oldState = GetCurrentState()
        MyBase.OnHandleCreated(e)
    End Sub

    Protected Overrides Sub OnHandleDestroyed(e As EventArgs)
        NativeMethods.BufferedPaintUnInit()
        MyBase.OnHandleDestroyed(e)
    End Sub

    Private ReadOnly Property UseAnimation As Boolean
        Get
            Return Not DesignMode AndAlso
                Application.RenderWithVisualStyles AndAlso
                DropDownStyle = ComboBoxStyle.DropDownList AndAlso
                FlatStyle = FlatStyle.Standard
        End Get
    End Property

    Private ReadOnly Property MouseIsOver As Boolean
        Get
            If Not IsHandleCreated Then Return False
            Dim screenPosition As Point = MousePosition
            If NativeMethods.WindowFromPoint(screenPosition) <> Handle Then Return False
            Dim rect As Rectangle = Bounds
            If Parent IsNot Nothing Then
                rect = Parent.RectangleToScreen(rect)
            End If
            If Not rect.Contains(screenPosition) Then Return False
            Return True
        End Get
    End Property

    Protected Overrides Sub WndProc(ByRef m As Message)
        Select Case m.Msg
            Case NativeMethods.WM_PAINT
                If UseAnimation Then
                    WmPaint(m)
                Else
                    MyBase.WndProc(m)
                End If
            Case NativeMethods.WM_PRINTCLIENT
                If UseAnimation Then
                    WmPrintClient(m)
                Else
                    MyBase.WndProc(m)
                End If
            Case Else
                MyBase.WndProc(m)
        End Select
    End Sub

    Private Sub WmPaint(ByRef m As Message)
        Dim newState As ComboBoxState = GetCurrentState()
        If m.WParam = IntPtr.Zero Then
            Dim ps As New NativeMethods.PAINTSTRUCT()
            Dim hdc As IntPtr = NativeMethods.BeginPaint(m.HWnd, ps)
            Dim hPal As IntPtr = NativeMethods.SetUpPalette(hdc, False, False)
            Try
                If Not NativeMethods.BufferedPaintRenderAnimation(m.HWnd, hdc) Then
                    Dim clip As Rectangle = Rectangle.FromLTRB(ps.rcPaint_left, ps.rcPaint_top,
                                                        ps.rcPaint_right, ps.rcPaint_bottom)
                    If clip.Width > 0 AndAlso clip.Height > 0 Then
                        DrawAnimation(m.HWnd, hdc, clip, newState)
                    End If
                End If
            Finally
                oldState = newState
                NativeMethods.SelectPalette(hdc, hPal, 0)
                NativeMethods.EndPaint(m.HWnd, ps)
            End Try
        Else
            Using g As Graphics = Graphics.FromHdc(m.WParam)
                DrawComboBox(g, ClientRectangle, newState)
            End Using
        End If
    End Sub

    Private Sub DrawAnimation(hwnd As IntPtr, hdc As IntPtr, clip As Rectangle, newState As ComboBoxState)
        Dim animParams As New NativeMethods.BP_ANIMATIONPARAMS
        animParams.cbSize = Marshal.SizeOf(animParams)
        animParams.style = NativeMethods.BP_ANIMATIONSTYLE.BPAS_LINEAR
        animParams.dwDuration = GetDuration(oldState, newState)
        Dim rcTarget As New NativeMethods.RECT(clip)
        Dim hdcFrom As IntPtr
        Dim hdcTo As IntPtr
        Dim hBuffer As IntPtr = NativeMethods.BeginBufferedAnimation(
                                        hwnd, hdc, rcTarget,
                                        NativeMethods.BP_BUFFERFORMAT.BPBF_COMPATIBLEBITMAP,
                                        IntPtr.Zero, animParams,
                                        hdcFrom, hdcTo)
        If hBuffer <> IntPtr.Zero Then
            Try
                If hdcFrom <> IntPtr.Zero Then
                    DrawComboBox(hdcFrom, clip, oldState, False)
                End If
                If hdcTo <> IntPtr.Zero Then
                    DrawComboBox(hdcTo, clip, newState, False)
                End If
            Finally
                NativeMethods.EndBufferedAnimation(hBuffer, True)
            End Try
        Else
            DrawComboBox(hdc, clip, newState, True)
        End If
    End Sub

    Private Function GetDuration(oldState As ComboBoxState, newState As ComboBoxState) As Integer
        Dim oldElement As VisualStyleElement = ToDropDownElement(oldState)
        Dim newElement As VisualStyleElement = ToDropDownElement(newState)
        Dim dwDuration As Integer = 0
        Dim hTheme As IntPtr = NativeMethods.OpenThemeData(Handle, oldElement.ClassName)
        If hTheme <> IntPtr.Zero Then
            Dim hResult As IntPtr = NativeMethods.GetThemeTransitionDuration(
                                            hTheme, oldElement.Part,
                                            oldElement.State, newElement.State,
                                            NativeMethods.TMT_TRANSITIONDURATIONS,
                                            dwDuration)
            If hResult <> IntPtr.Zero Then
                dwDuration = If(MouseIsOver, 200, 800)
            End If
            NativeMethods.CloseThemeData(hTheme)
        Else
            dwDuration = If(MouseIsOver, 200, 800)
        End If
        Return dwDuration
    End Function

    Private Sub WmPrintClient(ByRef m As Message)
        Using g As Graphics = Graphics.FromHdc(m.WParam)
            DrawComboBox(g, ClientRectangle, GetCurrentState())
        End Using
    End Sub

    Private Sub DrawComboBox(hdc As IntPtr, clip As Rectangle, state As ComboBoxState, bufferd As Boolean)
        If bufferd Then
            Dim context As BufferedGraphicsContext = BufferedGraphicsManager.Current
            Using bg As BufferedGraphics = context.Allocate(hdc, ClientRectangle)
                DrawComboBox(bg.Graphics, clip, state)
                bg.Render()
            End Using
        Else
            Using g As Graphics = Graphics.FromHdc(hdc)
                DrawComboBox(g, clip, state)
            End Using
        End If
    End Sub

    Private Sub DrawComboBox(g As Graphics, clip As Rectangle, state As ComboBoxState)
        Dim clientRect As Rectangle = ClientRectangle
        Dim controlRect As Rectangle = clientRect
        controlRect.Inflate(1, 1)
        g.SetClip(clip)
        VsRenderer.SetParameters(ToButtonElement(state))
        If VsRenderer.IsBackgroundPartiallyTransparent() Then
            VsRenderer.DrawParentBackground(g, controlRect, Me)
        End If
        VsRenderer.DrawBackground(g, controlRect)

        Dim dropdownWidth As Integer = SystemInformation.HorizontalScrollBarArrowWidth
        Dim dropdownRect As New Rectangle(controlRect.Right - dropdownWidth, controlRect.Top,
                                          dropdownWidth, controlRect.Height)
        Dim dropdownFace As Rectangle = Rectangle.Inflate(dropdownRect, -2, -2)
        VsRenderer.SetParameters(ToDropDownElement(state))
        VsRenderer.DrawBackground(g, dropdownRect, dropdownFace)

        Dim textFace As Rectangle = clientRect
        textFace.Width -= dropdownWidth
        textFace.Inflate(-3, -3)
        Dim textColor As Color = If(Enabled, ForeColor, SystemColors.GrayText)
        TextRenderer.DrawText(g, Text, Font, textFace, textColor,
                            TextFormatFlags.Left Or TextFormatFlags.VerticalCenter)
        If ContainsFocus AndAlso (Not DroppedDown) AndAlso ShowFocusCues Then
            ControlPaint.DrawFocusRectangle(g, textFace)
        End If
    End Sub

    Private Function ToButtonElement(state As ComboBoxState) As VisualStyleElement
        Select Case state
            Case ComboBoxState.Disabled
                Return ButtonElement.Disabled
            Case ComboBoxState.Hot
                Return ButtonElement.Hot
            Case ComboBoxState.Normal
                Return ButtonElement.Normal
            Case ComboBoxState.Pressed
                Return ButtonElement.Pressed
        End Select
        Return ButtonElement.Normal
    End Function

    Private Function ToDropDownElement(state As ComboBoxState) As VisualStyleElement
        Select Case state
            Case ComboBoxState.Disabled
                Return DropDownElement.Disabled
            Case ComboBoxState.Hot
                Return DropDownElement.Hot
            Case ComboBoxState.Normal
                Return DropDownElement.Normal
            Case ComboBoxState.Pressed
                Return DropDownElement.Pressed
        End Select
        Return DropDownElement.Normal
    End Function

End Class
