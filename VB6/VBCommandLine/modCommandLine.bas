Attribute VB_Name = "modCommandLine"
Option Explicit

Private Declare Function CommandLineToArgvW Lib "shell32" (ByVal lpCmdLine As Long, pNumArgs As Integer) As Long
Private Declare Function LocalFree Lib "kernel32" (ByVal hMem As Long) As Long
Private Declare Function lstrlenW Lib "kernel32" (ByVal lpString As Long) As Long
Private Declare Function lstrcpyW Lib "kernel32" (ByVal lpString1 As Long, ByVal lpString2 As Long) As Long
Private Declare Sub CopyMemory Lib "kernel32" Alias "RtlMoveMemory" (Destination As Any, Source As Any, ByVal Length As Long)
Private Declare Function SafeArrayAllocDescriptor Lib "oleaut32" (ByVal cDims As Long, ByRef ppsaOut() As Any) As Long

Private Const OFFSET_4      As Currency = 4294967296@
Private Const MAXINT_4      As Currency = 2147483647

Public Function DivCommand(ByVal iCommand As String) As String()
    Dim lpszArgs    As Long
    Dim nArgs       As Integer
    Dim i           As Long
    Dim nRet        As Long
    Dim sArgs()     As String

    sArgs = Split(vbNullString)
    iCommand = Trim(iCommand)
    If Len(iCommand) > 0 Then
        lpszArgs = CommandLineToArgvW(StrPtr(iCommand), nArgs)
        If lpszArgs <> 0 Then
            ReDim sArgs(nArgs - 1)
            For i = 0 To nArgs - 1
                sArgs(i) = ToStringW(GetPointer(lpszArgs, i))
            Next
            nRet = LocalFree(lpszArgs)
        End If
    End If
    DivCommand = sArgs
End Function

Private Function GetPointer(ByVal iAddress As Long, ByVal iIndex As Long) As Long
    Dim lpAddress As Long
    Dim lpPointer As Long

    lpAddress = SLONG(ULONG(iAddress) + iIndex * 4)

    CopyMemory lpPointer, ByVal lpAddress, Len(lpPointer)
    GetPointer = lpPointer

End Function

Private Function SLONG(ByVal Value As Currency) As Long
    If Value < 0 Or Value >= OFFSET_4 Then Error 6
    If Value <= MAXINT_4 Then
        SLONG = Value
    Else
        SLONG = Value - OFFSET_4
    End If
End Function

Private Function ULONG(ByVal Value As Long) As Currency
    If Value < 0 Then
        ULONG = Value + OFFSET_4
    Else
        ULONG = Value
    End If
End Function

Private Function ToStringW(ByVal lpAddr As Long) As String
    Dim nLen As Long
    Dim strBuffer As String
    
    If lpAddr <> 0 Then
        nLen = lstrlenW(lpAddr)
        If nLen > 0 Then
            strBuffer = String(nLen + 1, vbNullChar)
            Call lstrcpyW(StrPtr(strBuffer), lpAddr)
            ToStringW = Left(strBuffer, InStr(strBuffer, vbNullChar) - 1)
        Else
            ToStringW = ""
        End If
    Else
        ToStringW = ""
    End If
End Function

