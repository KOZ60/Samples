Imports System.Runtime.InteropServices

Friend Class NativeMethods

    Private Sub New()
    End Sub

    Private Class ExternDll
        Public Const User32 As String = "user32.dll"
        Public Const Gdi32 As String = "gdi32.dll"
        Public Const Uxtheme As String = "uxtheme.dll"
    End Class

    Public Const WM_PAINT As Integer = &HF
    Public Const WM_PRINTCLIENT As Integer = &H318

    <StructLayout(LayoutKind.Sequential)>
    Public Structure RECT
        Public left, top, right, bottom As Integer

        Public Sub New(r As Rectangle)
            left = r.Left
            top = r.Top
            right = r.Right
            bottom = r.Bottom
        End Sub
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Public Structure PAINTSTRUCT
        Public hdc As IntPtr
        Public fErase As Boolean
        Public rcPaint_left As Integer
        Public rcPaint_top As Integer
        Public rcPaint_right As Integer
        Public rcPaint_bottom As Integer
        Public fRestore As Boolean
        Public fIncUpdate As Boolean
        Public reserved1 As Integer
        Public reserved2 As Integer
        Public reserved3 As Integer
        Public reserved4 As Integer
        Public reserved5 As Integer
        Public reserved6 As Integer
        Public reserved7 As Integer
        Public reserved8 As Integer
    End Structure

    <DllImport(ExternDll.User32)>
    Public Shared Function BeginPaint(hWnd As IntPtr, ByRef lpPaint As PAINTSTRUCT) As IntPtr
    End Function

    <DllImport(ExternDll.User32)>
    Public Shared Function EndPaint(hWnd As IntPtr, ByRef lpPaint As PAINTSTRUCT) As Boolean
    End Function

    <DllImport(ExternDll.Gdi32)>
    Public Shared Function SelectPalette(hdc As IntPtr, hpal As IntPtr, bForceBackground As Integer) As IntPtr
    End Function

    <DllImport(ExternDll.Gdi32)>
    Public Shared Function RealizePalette(hDC As IntPtr) As Integer
    End Function

    Public Shared Function SetUpPalette(dc As IntPtr, force As Boolean, realize As Boolean) As IntPtr
        Dim halftonePalette As IntPtr = Graphics.GetHalftonePalette()
        Dim result As IntPtr = SelectPalette(dc, halftonePalette, (If(force, 0, 1)))
        If result <> IntPtr.Zero AndAlso realize Then
            RealizePalette(dc)
        End If
        Return result
    End Function

    <DllImport(ExternDll.User32)>
    Public Shared Function WindowFromPoint(p As Point) As IntPtr
    End Function

    Public Const TMT_TRANSITIONDURATIONS As Integer = 6000

    <Flags>
    Public Enum BP_ANIMATIONSTYLE
        BPAS_NONE = 0
        BPAS_LINEAR = 1
        BPAS_CUBIC = 2
        BPAS_SINE = 3
    End Enum

    Public Enum BP_BUFFERFORMAT
        BPBF_COMPATIBLEBITMAP
        BPBF_DIB
        BPBF_TOPDOWNDIB
        BPBF_TOPDOWNMONODIB
    End Enum

    <StructLayout(LayoutKind.Sequential)>
    Public Structure BP_ANIMATIONPARAMS
        Public cbSize As Integer
        Public dwFlags As Integer
        Public style As BP_ANIMATIONSTYLE
        Public dwDuration As Integer
    End Structure

    <DllImport(ExternDll.Uxtheme)>
    Public Shared Function OpenThemeData(
                hwnd As IntPtr,
                <MarshalAs(UnmanagedType.LPWStr)> pszClassList As String) As IntPtr
    End Function

    <DllImport(ExternDll.Uxtheme)>
    Public Shared Function CloseThemeData(hTheme As IntPtr) As Integer
    End Function

    <DllImport(ExternDll.Uxtheme)>
    Public Shared Function GetThemeTransitionDuration(
                hTheme As IntPtr, iPartId As Integer,
                iStateIdFrom As Integer, iStateIdTo As Integer, iPropId As Integer,
                <Out> ByRef pdwDuration As Integer) As IntPtr
    End Function

    <DllImport(ExternDll.Uxtheme)>
    Public Shared Function BufferedPaintInit() As IntPtr
    End Function

    <DllImport(ExternDll.Uxtheme)>
    Public Shared Function BufferedPaintUnInit() As IntPtr
    End Function

    <DllImport(ExternDll.Uxtheme)>
    Public Shared Function BeginBufferedAnimation(
                hwnd As IntPtr, hdcTarget As IntPtr, ByRef rcTarget As RECT,
                dwFormat As BP_BUFFERFORMAT,
                pPaintParams As IntPtr, ByRef pAnimationParams As BP_ANIMATIONPARAMS,
                <Out> ByRef phdcFrom As IntPtr, <Out> ByRef phdcTo As IntPtr) As IntPtr
    End Function

    <DllImport(ExternDll.Uxtheme)>
    Public Shared Function EndBufferedAnimation(
                hbpAnimation As IntPtr,
                fUpdateTarget As Boolean) As Integer
    End Function

    <DllImport(ExternDll.Uxtheme)>
    Public Shared Function BufferedPaintRenderAnimation(
                hwnd As IntPtr,
                hdcTarget As IntPtr) As Boolean
    End Function

End Class
