Option Strict On
Imports System.ComponentModel
Imports System.Windows.Forms

Namespace Global.Microsoft.VisualBasic.Compatibility.VB6

    <ProvideProperty("Index", GetType(CheckBox))>   '拡張プロパティの宣言
    Public Class CheckBoxArray
        Inherits BaseControlArray(Of CheckBox)

        '----------------------------------------------------------------
        ' コントロール固有のイベントをフックを行う
        '----------------------------------------------------------------
        Protected Overrides Sub HookUpEvents(o As CheckBox)
            AddHandler o.CheckedChanged, OnCheckedChanged
            AddHandler o.CheckStateChanged, OnCheckStateChanged
            AddHandler o.AppearanceChanged, OnAppearanceChanged
        End Sub

        '----------------------------------------------------------------
        ' コントロール固有のイベントのフックを解除する
        '----------------------------------------------------------------
        Protected Overrides Sub HookDownEvents(o As CheckBox)
            RemoveHandler o.CheckedChanged, OnCheckedChanged
            RemoveHandler o.CheckStateChanged, OnCheckStateChanged
            RemoveHandler o.AppearanceChanged, OnAppearanceChanged
        End Sub

        '----------------------------------------------------------------
        ' コントロール配列のイベントを呼び出すデリゲートの作成
        '----------------------------------------------------------------
        Private ReadOnly OnCheckedChanged As New EventHandler(Sub(s, e) RaiseEvent CheckedChanged(s, e))
        Private ReadOnly OnCheckStateChanged As New EventHandler(Sub(s, e) RaiseEvent CheckStateChanged(s, e))
        Private ReadOnly OnAppearanceChanged As New EventHandler(Sub(s, e) RaiseEvent AppearanceChanged(s, e))

        '----------------------------------------------------------------
        ' コントロール配列のイベントを定義
        '----------------------------------------------------------------
        Public Event CheckedChanged As EventHandler
        Public Event CheckStateChanged As EventHandler
        Public Event AppearanceChanged As EventHandler

    End Class

End Namespace

