Attribute VB_Name = "modVBCommandLine"
Option Explicit

Sub Main()
    Dim v As Variant
    For Each v In DivCommand("""C:\program files"" c:\program files")
        Debug.Print v
    Next
    For Each v In DivCommand("")
        Debug.Print v
    Next
End Sub
