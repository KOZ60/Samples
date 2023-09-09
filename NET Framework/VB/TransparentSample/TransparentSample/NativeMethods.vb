Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

Friend Module NativeMethods

    Private Class ExternDll
        Public Const User32 = "user32"
    End Class

    Public Const WS_EX_LAYERED As Integer = &H80000
    Public Const GWL_EXSTYLE As Integer = -20
    Public Const LWA_ALPHA As Integer = &H2
    Public Const LWA_COLORKEY As Integer = &H1

    ' コントロールの座標を他のコントロールの座標に変換する
    ' Screen 座標を経由して変換すると高 DPI 環境下で若干ずれが発生するとの報告があったので
    ' MapWindowPoints を呼び出して直接変換する
    <Extension>
    Public Function MapRectangle(
                        fromCtrl As Control, fromRect As Rectangle,
                        toCtrl As Control) As Rectangle
        Dim rect As New RECT(fromRect)
        MapWindowPoints(fromCtrl.Handle, toCtrl.Handle, rect, 2)
        Return rect.ToRectangle()
    End Function

    <Extension>
    Public Function DrawToBitmap(ctrl As Control) As Bitmap
        'Argb 系を使うとノイズが入ることがあるので Format24bppRgb を使う
        Dim bmp As New Bitmap(ctrl.Width, ctrl.Height,
                                Imaging.PixelFormat.Format24bppRgb)
        ctrl.DrawToBitmap(bmp, New Rectangle(Point.Empty, New Size(ctrl.Width, ctrl.Height)))
        Return bmp
    End Function

    <Extension>
    Public Function SetLayeredStyle(ctrl As Control) As Boolean
        Dim exStyle = GetWindowLong(ctrl.Handle, GWL_EXSTYLE).ToInt64()
        exStyle = exStyle Or WS_EX_LAYERED
        SetWindowLong(ctrl.Handle, GWL_EXSTYLE, New IntPtr(exStyle))
        exStyle = GetWindowLong(ctrl.Handle, GWL_EXSTYLE).ToInt64()
        Return (exStyle And WS_EX_LAYERED) = WS_EX_LAYERED
    End Function

    <StructLayout(LayoutKind.Sequential)>
    Public Structure RECT
        Public Left, Top, Right, Bottom As Integer
        Public Sub New(r As Rectangle)
            Left = r.Left
            Top = r.Top
            Bottom = r.Bottom
            Right = r.Right
        End Sub
        Public Function ToRectangle() As Rectangle
            Return Rectangle.FromLTRB(Left, Top, Right, Bottom)
        End Function
    End Structure

    <DllImport(ExternDll.User32)>
    Private Function MapWindowPoints(
                        hwndFrom As IntPtr, hwndTo As IntPtr,
                        ByRef RECT As RECT, cPoints As Integer) As Integer
    End Function

    <DllImport("user32.dll")>
    Public Function SetLayeredWindowAttributes(
                hWnd As IntPtr, crKey As Integer,
                bAlpha As Byte, dwFlags As Integer) As Boolean
    End Function

    Public Function GetWindowLong(hWnd As IntPtr, nIndex As Integer) As IntPtr
        If Environment.Is64BitProcess Then
            Return GetWindowLongPtr(hWnd, nIndex)
        Else
            Return GetWindowLong32(hWnd, nIndex)
        End If
    End Function

    <DllImport("user32.dll", SetLastError:=True, EntryPoint:="GetWindowLong")>
    Private Function GetWindowLong32(hWnd As IntPtr, nIndex As Integer) As IntPtr
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Private Function GetWindowLongPtr(hWnd As IntPtr, nIndex As Integer) As IntPtr
    End Function

    Public Function SetWindowLong(hWnd As IntPtr, nIndex As Integer, dwNewLong As IntPtr) As IntPtr
        If Environment.Is64BitProcess Then
            Return SetWindowLongPtr(hWnd, nIndex, dwNewLong)
        Else
            Return SetWindowLong32(hWnd, nIndex, dwNewLong)
        End If
    End Function

    <DllImport("user32.dll", SetLastError:=True, EntryPoint:="SetWindowLong")>
    Private Function SetWindowLong32(hWnd As IntPtr, nIndex As Integer, dwNewLong As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Private Function SetWindowLongPtr(hWnd As IntPtr, nIndex As Integer, dwNewLong As IntPtr) As IntPtr
    End Function

End Module
