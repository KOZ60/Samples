Option Strict On

Imports System.Globalization
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace VB6

    Public Module Support

        Public Function Format(Expression As Object, Optional Style As String = "",
                           Optional DayOfWeek As FirstDayOfWeek = FirstDayOfWeek.Sunday,
                           Optional WeekOfYear As FirstWeekOfYear = FirstWeekOfYear.Jan1) As String
            If (TypeOf Expression Is Long) Then
                Expression = New Decimal(Convert.ToInt64(RuntimeHelpers.GetObjectValue(Expression)))
            ElseIf (TypeOf Expression Is Char) Then
                Expression = Expression.ToString()
            End If
            Dim nResult As Integer
            Dim str As String = Nothing
            Dim intPtr As IntPtr = Marshal.AllocCoTaskMem(24)
            Try
                NativeMethods.VariantInit(intPtr)
                Try
                    Marshal.GetNativeVariantForObject(Expression, intPtr)
                    Dim dwFlags = If(TypeOf Thread.CurrentThread.CurrentCulture.Calendar Is HijriCalendar,
                                            NativeMethods.VAR_CALENDAR_HIJRI,
                                            NativeMethods.VAR_FORMAT_NOSUBSTITUTE)
                    nResult = NativeMethods.VarFormat(intPtr, Style, DayOfWeek, WeekOfYear, dwFlags, str)
                Finally
                    NativeMethods.VariantClear(intPtr)
                End Try
            Finally
                Marshal.FreeCoTaskMem(intPtr)
            End Try
            If (nResult < 0) Then
                Throw New ArgumentException()
            End If
            Return str
        End Function

        Friend Class NativeMethods

            Public Const VAR_FORMAT_NOSUBSTITUTE As Integer = &H20
            Public Const VAR_CALENDAR_HIJRI As Integer = &H8

            <DllImport("oleaut32.dll", CharSet:=CharSet.Unicode, ExactSpelling:=True)>
            Public Shared Function VarFormat(pvariant As IntPtr,
                                             sFmt As String,
                                             dow As Integer,
                                             woy As Integer,
                                             dwFlags As Integer,
                                             <MarshalAs(UnmanagedType.BStr)> ByRef sb As String) As Integer
            End Function

            <DllImport("oleaut32.dll", CharSet:=CharSet.Ansi, ExactSpelling:=True)>
            Public Shared Function VariantClear(ByVal pObject As IntPtr) As Integer
            End Function

            <DllImport("oleaut32.dll", CharSet:=CharSet.Ansi, ExactSpelling:=True)>
            Public Shared Sub VariantInit(ByVal pObject As IntPtr)
            End Sub

        End Class
    End Module

End Namespace

