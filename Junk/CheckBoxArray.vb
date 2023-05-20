Imports System.ComponentModel
Imports System.Windows.Forms

Namespace Global.Microsoft.VisualBasic.Compatibility.VB6

    <ProvideProperty("Index", GetType(CheckBox))>
    Public Class CheckBoxArray
        Inherits ControlArray(Of CheckBox)

        Protected Overrides Sub HookUpEvents(o As CheckBox)
            AddHandler o.CheckedChanged, OnCheckedChanged
            AddHandler o.CheckStateChanged, OnCheckStateChanged
            AddHandler o.AppearanceChanged, OnAppearanceChanged
        End Sub

        Protected Overrides Sub HookDownEvents(o As CheckBox)
            RemoveHandler o.CheckedChanged, OnCheckedChanged
            RemoveHandler o.CheckStateChanged, OnCheckStateChanged
            RemoveHandler o.AppearanceChanged, OnAppearanceChanged
        End Sub

        Private ReadOnly OnCheckedChanged As New EventHandler(Sub(s, e) RaiseEvent CheckedChanged(s, e))
        Private ReadOnly OnCheckStateChanged As New EventHandler(Sub(s, e) RaiseEvent CheckStateChanged(s, e))
        Private ReadOnly OnAppearanceChanged As New EventHandler(Sub(s, e) RaiseEvent AppearanceChanged(s, e))

        Public Event CheckedChanged As EventHandler
        Public Event CheckStateChanged As EventHandler
        Public Event AppearanceChanged As EventHandler

    End Class

End Namespace

