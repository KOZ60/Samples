Option Strict On

Imports System.Globalization
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Global.Microsoft.VisualBasic.Compatibility.VB6

    Public Module Support

        Public Function Format(Expression As Object, Optional Style As String = "",
                           Optional DayOfWeek As FirstDayOfWeek = FirstDayOfWeek.Sunday,
                           Optional WeekOfYear As FirstWeekOfYear = FirstWeekOfYear.Jan1) As String
            If (TypeOf Expression Is Long) Then
                Expression = CDec(CLng(Expression))
            ElseIf (TypeOf Expression Is Char) Then
                Expression = Expression.ToString()
            End If
            Dim str As String = Nothing
            Dim ptr As IntPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(GetType(NativeMethods.TagVARIANT)))
            Dim hr As Integer
            Try
                NativeMethods.VariantInit(ptr)
                Try
                    Marshal.GetNativeVariantForObject(Expression, ptr)
                    Dim dwFlags As Integer = If(TypeOf Thread.CurrentThread.CurrentCulture.Calendar Is HijriCalendar,
                                                       NativeMethods.VAR_CALENDAR_HIJRI,
                                                       NativeMethods.VAR_FORMAT_NOSUBSTITUTE)
                    hr = NativeMethods.VarFormat(ptr, Style, DayOfWeek, WeekOfYear, dwFlags, str)
                    If NativeMethods.FAILED(hr) Then
                        Throw Marshal.GetExceptionForHR(hr)
                    End If
                Finally
                    hr = NativeMethods.VariantClear(ptr)
                End Try
            Finally
                Marshal.FreeCoTaskMem(ptr)
            End Try
            If NativeMethods.FAILED(hr) Then
                Throw Marshal.GetExceptionForHR(hr)
            End If
            Return str
        End Function

    End Module

    Friend Class NativeMethods

        Private Const Oleaut32 As String = "oleaut32.dll"

        Public Const VAR_FORMAT_NOSUBSTITUTE As Integer = &H20
        Public Const VAR_CALENDAR_HIJRI As Integer = &H8

        <DllImport(Oleaut32, CharSet:=CharSet.Auto, ExactSpelling:=True)>
        Public Shared Function VarFormat(pvariant As IntPtr,
                                             sFmt As String,
                                             dow As Integer,
                                             woy As Integer,
                                             dwFlags As Integer,
                                             <MarshalAs(UnmanagedType.BStr)> ByRef sb As String) As Integer
        End Function

        <DllImport(Oleaut32, CharSet:=CharSet.Auto, ExactSpelling:=True)>
        Public Shared Function VariantClear(pObject As IntPtr) As Integer
        End Function

        <DllImport(Oleaut32, CharSet:=CharSet.Auto, ExactSpelling:=True)>
        Public Shared Sub VariantInit(pObject As IntPtr)
        End Sub

        Public Structure TagVARIANT
            Public vt As Short
            Public reserved1 As Short
            Public reserved2 As Short
            Public reserved3 As Short
            Public data1 As IntPtr
            Public data2 As IntPtr
        End Structure

        Public Shared Function SUCCEEDED(hr As Integer) As Boolean
            Return hr >= 0
        End Function

        Public Shared Function FAILED(hr As Integer) As Boolean
            Return hr < 0
        End Function

    End Class

End Namespace

