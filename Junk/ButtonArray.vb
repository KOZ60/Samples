Option Strict On
Imports System.ComponentModel
Imports System.Windows.Forms

Namespace Global.Microsoft.VisualBasic.Compatibility.VB6

	<ProvideProperty("Index", GetType(Button))>
	Public Class ButtonArray
		Inherits BaseControlArray(Of Button)

		Public Sub New()
			MyBase.New()
		End Sub

		Public Sub New(Container As IContainer)
			MyBase.New(Container)
		End Sub

		Protected Overrides Sub HookUpEvents(o As Button)
		End Sub

		Protected Overrides Sub HookDownEvents(o As Button)
		End Sub

	End Class


End Namespace

