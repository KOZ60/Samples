Imports System.ComponentModel
Imports System.Reflection
Imports System.Windows.Forms

Namespace Global.Microsoft.VisualBasic.Compatibility.VB6

	Public MustInherit Class ControlArray(Of T As Control)
		Inherits Component
		Implements ISupportInitialize
		Implements IExtenderProvider
		Implements IEnumerable(Of T)

		Protected ReadOnly indices As Dictionary(Of T, Integer)
		Protected ReadOnly controls As Dictionary(Of Integer, T)
		Protected components As IContainer
		Private ReadOnly properties As PropertyDescriptorCollection =
								TypeDescriptor.GetProperties(GetType(T))
		Private _Form As Form
		Private _FormType As Type
		Private _ToolTipScaned As Boolean
		Private _ToolTip As ToolTip
		Private Const _BindingFlags As BindingFlags = BindingFlags.Public Or
							BindingFlags.NonPublic Or BindingFlags.Instance

		Public Sub New()
			MyBase.New()
			indices = New Dictionary(Of T, Integer)()
			controls = New Dictionary(Of Integer, T)()
		End Sub

		Public Sub New(Container As IContainer)
			MyClass.New()
			Container.Add(Me)
			components = Container
		End Sub

		Default Public ReadOnly Property Item(Index As Integer) As T
			Get
				Return controls(Index)
			End Get
		End Property

		Protected Overridable Function CanExtend(extendee As Object) As Boolean _
							Implements IExtenderProvider.CanExtend
			If extendee IsNot Nothing Then
				Return extendee.GetType().Equals(GetType(T))
			Else
				Return False
			End If
		End Function

		Public Function GetIndex(o As Object) As Integer
			Dim target As T = TryCast(o, T)
			Dim index As Integer
			If target IsNot Nothing Then
				If Not indices.TryGetValue(target, index) Then
					index = -1
				End If
			Else
				index = -1
			End If
			Return index
		End Function

		Public Sub SetIndex(o As Object, Index As Integer)
			If Not CanExtend(o) Then
				Throw New ArgumentException("型が違います。")
			End If
			If controls.ContainsKey(Index) Then
				Throw New ArgumentException("同じインデックスが存在しています。")
			End If
			ResetIndex(o)
			Dim target As T = DirectCast(o, T)
			indices(target) = Index
			controls(Index) = target
			HookUpEventsOfControl(target)
			HookUpEvents(target)
		End Sub

		Protected MustOverride Sub HookUpEvents(o As T)
		Protected MustOverride Sub HookDownEvents(o As T)

		Public Sub ResetIndex(o As Object)
			Dim target As T = TryCast(o, T)
			If target IsNot Nothing Then
				Dim index As Integer
				If indices.TryGetValue(target, index) Then
					indices.Remove(target)
					controls.Remove(index)
					HookDownEventsOfControl(target)
					HookDownEvents(target)
				End If
			End If
		End Sub

		Public Function ShouldSerializeIndex(o As Object) As Boolean
			Dim target As T = TryCast(o, T)
			If target IsNot Nothing Then
				Return indices.ContainsKey(target)
			Else
				Return False
			End If
		End Function

		Public ReadOnly Property Count As Integer
			Get
				Return controls.Count
			End Get
		End Property

		Public ReadOnly Property LBound As Integer
			Get
				If controls.Count = 0 Then
					Return 0
				End If
				Dim minValue As Integer = Integer.MaxValue
				For Each kp In indices
					If kp.Value < minValue Then
						minValue = kp.Value
					End If
				Next
				Return minValue
			End Get
		End Property

		Public ReadOnly Property UBound As Integer
			Get
				If controls.Count = 0 Then
					Return -1
				End If
				Dim maxValue As Integer = Integer.MinValue
				For Each kp In indices
					If kp.Value > maxValue Then
						maxValue = kp.Value
					End If
				Next
				Return maxValue
			End Get
		End Property

		Private Iterator Function GetEnumeratorOf() As IEnumerator(Of T) _
										Implements IEnumerable(Of T).GetEnumerator
			For Each kp In controls
				Yield kp.Value
			Next
		End Function

		Private Iterator Function GetEnumerator() As IEnumerator _
										Implements IEnumerable.GetEnumerator
			For Each kp In controls
				Yield kp.Value
			Next
		End Function

		Private Sub BeginInit() Implements ISupportInitialize.BeginInit
		End Sub

		Private Sub EndInit() Implements ISupportInitialize.EndInit
		End Sub

		Public Function Load(Index As Integer) As T
			If Index < 0 OrElse Count = 0 OrElse controls.ContainsKey(Index) Then
				Throw New IndexOutOfRangeException()
			End If
			Dim result As T = CloneControl()
			SetIndex(result, Index)
			Return result
		End Function

		Public Sub Unload(Index As Integer)
			Dim ctl As T = Nothing
			If Index < 0 OrElse Not controls.TryGetValue(Index, ctl) Then
				Throw New IndexOutOfRangeException()
			End If
			ResetIndex(ctl)
			ctl.Parent.Controls.Remove(ctl)
		End Sub

		Private Function CloneControl() As T
			Dim lowest As T = controls(LBound)
			Dim ctl As T = DirectCast(Activator.CreateInstance(GetType(T)), T)
			For Each p As PropertyDescriptor In properties
				If IsSerialized(lowest, p) Then
					p.SetValue(ctl, p.GetValue(lowest))
				End If
			Next
			Dim rdo As RadioButton = TryCast(ctl, RadioButton)
			If rdo IsNot Nothing Then
				rdo.Checked = False
			End If
			'VB6 から移植したフォームは ToolTip1 を持っているので設定された caption もコピー
			If ToolTip1 IsNot Nothing Then
				Dim caption As String = ToolTip1.GetToolTip(lowest)
				If Not String.IsNullOrEmpty(caption) Then
					ToolTip1.SetToolTip(ctl, caption)
				End If
			End If
			lowest.Parent.Controls.Add(ctl)
			Return ctl
		End Function

		Private Function IsSerialized(ctl As T, p As PropertyDescriptor) As Boolean
			If p.IsReadOnly Then
				Return False
			End If
			If p.SerializationVisibility <> DesignerSerializationVisibility.Visible Then
				Return False
			End If
			If Not p.ShouldSerializeValue(ctl) Then
				Return False
			End If
			Select Case p.Name
				Case "Visible", "TabIndex", "Index", "MdiList"
					Return False
			End Select
			Return True
		End Function

		Private ReadOnly Property ToolTip1() As ToolTip
			Get
				If _ToolTipScaned Then
					Return _ToolTip
				End If
				Dim ctl As T = controls(LBound)
				If _Form Is Nothing Then
					_Form = ctl.FindForm()
					_FormType = _Form.GetType()
				End If
				Dim pi As PropertyInfo = _FormType.GetProperty("ToolTip1", _BindingFlags)
				If pi IsNot Nothing Then
					_ToolTip = TryCast(pi.GetValue(_Form, Nothing), ToolTip)
				End If
				If _ToolTip Is Nothing Then
					Dim fi As FieldInfo = _FormType.GetField("ToolTip1", _BindingFlags)
					If fi IsNot Nothing Then
						_ToolTip = TryCast(fi.GetValue(_Form), ToolTip)
					End If
				End If
				_ToolTipScaned = True
				Return _ToolTip
			End Get
		End Property

		Protected Overrides Sub Dispose(disposing As Boolean)
			components = Nothing
			MyBase.Dispose(disposing)
		End Sub

		Private Sub HookUpEventsOfControl(o As Control)
			AddHandler o.AutoSizeChanged, OnAutoSizeChanged
			AddHandler o.BackColorChanged, OnBackColorChanged
			AddHandler o.BackgroundImageChanged, OnBackgroundImageChanged
			AddHandler o.BackgroundImageLayoutChanged, OnBackgroundImageLayoutChanged
			AddHandler o.BindingContextChanged, OnBindingContextChanged
			AddHandler o.CausesValidationChanged, OnCausesValidationChanged
			AddHandler o.ChangeUICues, OnChangeUICues
			AddHandler o.Click, OnClick
			AddHandler o.ClientSizeChanged, OnClientSizeChanged
			AddHandler o.ContextMenuChanged, OnContextMenuChanged
			AddHandler o.ContextMenuStripChanged, OnContextMenuStripChanged
			AddHandler o.ControlAdded, OnControlAdded
			AddHandler o.ControlRemoved, OnControlRemoved
			AddHandler o.CursorChanged, OnCursorChanged
			AddHandler o.Disposed, OnDisposed
			AddHandler o.DockChanged, OnDockChanged
			AddHandler o.DoubleClick, OnDoubleClick
			AddHandler o.DragDrop, OnDragDrop
			AddHandler o.DragEnter, OnDragEnter
			AddHandler o.DragLeave, OnDragLeave
			AddHandler o.DragOver, OnDragOver
			AddHandler o.EnabledChanged, OnEnabledChanged
			AddHandler o.Enter, OnEnter
			AddHandler o.FontChanged, OnFontChanged
			AddHandler o.ForeColorChanged, OnForeColorChanged
			AddHandler o.GiveFeedback, OnGiveFeedback
			AddHandler o.GotFocus, OnGotFocus
			AddHandler o.HandleCreated, OnHandleCreated
			AddHandler o.HandleDestroyed, OnHandleDestroyed
			AddHandler o.HelpRequested, OnHelpRequested
			AddHandler o.ImeModeChanged, OnImeModeChanged
			AddHandler o.Invalidated, OnInvalidated
			AddHandler o.KeyDown, OnKeyDown
			AddHandler o.KeyPress, OnKeyPress
			AddHandler o.KeyUp, OnKeyUp
			AddHandler o.Layout, OnLayout
			AddHandler o.Leave, OnLeave
			AddHandler o.LocationChanged, OnLocationChanged
			AddHandler o.LostFocus, OnLostFocus
			AddHandler o.MarginChanged, OnMarginChanged
			AddHandler o.MouseCaptureChanged, OnMouseCaptureChanged
			AddHandler o.MouseClick, OnMouseClick
			AddHandler o.MouseDoubleClick, OnMouseDoubleClick
			AddHandler o.MouseDown, OnMouseDown
			AddHandler o.MouseEnter, OnMouseEnter
			AddHandler o.MouseHover, OnMouseHover
			AddHandler o.MouseLeave, OnMouseLeave
			AddHandler o.MouseMove, OnMouseMove
			AddHandler o.MouseUp, OnMouseUp
			AddHandler o.MouseWheel, OnMouseWheel
			AddHandler o.Move, OnMove
			AddHandler o.PaddingChanged, OnPaddingChanged
			AddHandler o.Paint, OnPaint
			AddHandler o.ParentChanged, OnParentChanged
			AddHandler o.PreviewKeyDown, OnPreviewKeyDown
			AddHandler o.QueryAccessibilityHelp, OnQueryAccessibilityHelp
			AddHandler o.QueryContinueDrag, OnQueryContinueDrag
			AddHandler o.RegionChanged, OnRegionChanged
			AddHandler o.Resize, OnResize
			AddHandler o.RightToLeftChanged, OnRightToLeftChanged
			AddHandler o.SizeChanged, OnSizeChanged
			AddHandler o.StyleChanged, OnStyleChanged
			AddHandler o.SystemColorsChanged, OnSystemColorsChanged
			AddHandler o.TabIndexChanged, OnTabIndexChanged
			AddHandler o.TabStopChanged, OnTabStopChanged
			AddHandler o.TextChanged, OnTextChanged
			AddHandler o.Validated, OnValidated
			AddHandler o.Validating, OnValidating
			AddHandler o.VisibleChanged, OnVisibleChanged
		End Sub

		Private Sub HookDownEventsOfControl(o As Control)
			RemoveHandler o.AutoSizeChanged, OnAutoSizeChanged
			RemoveHandler o.BackColorChanged, OnBackColorChanged
			RemoveHandler o.BackgroundImageChanged, OnBackgroundImageChanged
			RemoveHandler o.BackgroundImageLayoutChanged, OnBackgroundImageLayoutChanged
			RemoveHandler o.BindingContextChanged, OnBindingContextChanged
			RemoveHandler o.CausesValidationChanged, OnCausesValidationChanged
			RemoveHandler o.ChangeUICues, OnChangeUICues
			RemoveHandler o.Click, OnClick
			RemoveHandler o.ClientSizeChanged, OnClientSizeChanged
			RemoveHandler o.ContextMenuChanged, OnContextMenuChanged
			RemoveHandler o.ContextMenuStripChanged, OnContextMenuStripChanged
			RemoveHandler o.ControlAdded, OnControlAdded
			RemoveHandler o.ControlRemoved, OnControlRemoved
			RemoveHandler o.CursorChanged, OnCursorChanged
			RemoveHandler o.Disposed, OnDisposed
			RemoveHandler o.DockChanged, OnDockChanged
			RemoveHandler o.DoubleClick, OnDoubleClick
			RemoveHandler o.DragDrop, OnDragDrop
			RemoveHandler o.DragEnter, OnDragEnter
			RemoveHandler o.DragLeave, OnDragLeave
			RemoveHandler o.DragOver, OnDragOver
			RemoveHandler o.EnabledChanged, OnEnabledChanged
			RemoveHandler o.Enter, OnEnter
			RemoveHandler o.FontChanged, OnFontChanged
			RemoveHandler o.ForeColorChanged, OnForeColorChanged
			RemoveHandler o.GiveFeedback, OnGiveFeedback
			RemoveHandler o.GotFocus, OnGotFocus
			RemoveHandler o.HandleCreated, OnHandleCreated
			RemoveHandler o.HandleDestroyed, OnHandleDestroyed
			RemoveHandler o.HelpRequested, OnHelpRequested
			RemoveHandler o.ImeModeChanged, OnImeModeChanged
			RemoveHandler o.Invalidated, OnInvalidated
			RemoveHandler o.KeyDown, OnKeyDown
			RemoveHandler o.KeyPress, OnKeyPress
			RemoveHandler o.KeyUp, OnKeyUp
			RemoveHandler o.Layout, OnLayout
			RemoveHandler o.Leave, OnLeave
			RemoveHandler o.LocationChanged, OnLocationChanged
			RemoveHandler o.LostFocus, OnLostFocus
			RemoveHandler o.MarginChanged, OnMarginChanged
			RemoveHandler o.MouseCaptureChanged, OnMouseCaptureChanged
			RemoveHandler o.MouseClick, OnMouseClick
			RemoveHandler o.MouseDoubleClick, OnMouseDoubleClick
			RemoveHandler o.MouseDown, OnMouseDown
			RemoveHandler o.MouseEnter, OnMouseEnter
			RemoveHandler o.MouseHover, OnMouseHover
			RemoveHandler o.MouseLeave, OnMouseLeave
			RemoveHandler o.MouseMove, OnMouseMove
			RemoveHandler o.MouseUp, OnMouseUp
			RemoveHandler o.MouseWheel, OnMouseWheel
			RemoveHandler o.Move, OnMove
			RemoveHandler o.PaddingChanged, OnPaddingChanged
			RemoveHandler o.Paint, OnPaint
			RemoveHandler o.ParentChanged, OnParentChanged
			RemoveHandler o.PreviewKeyDown, OnPreviewKeyDown
			RemoveHandler o.QueryAccessibilityHelp, OnQueryAccessibilityHelp
			RemoveHandler o.QueryContinueDrag, OnQueryContinueDrag
			RemoveHandler o.RegionChanged, OnRegionChanged
			RemoveHandler o.Resize, OnResize
			RemoveHandler o.RightToLeftChanged, OnRightToLeftChanged
			RemoveHandler o.SizeChanged, OnSizeChanged
			RemoveHandler o.StyleChanged, OnStyleChanged
			RemoveHandler o.SystemColorsChanged, OnSystemColorsChanged
			RemoveHandler o.TabIndexChanged, OnTabIndexChanged
			RemoveHandler o.TabStopChanged, OnTabStopChanged
			RemoveHandler o.TextChanged, OnTextChanged
			RemoveHandler o.Validated, OnValidated
			RemoveHandler o.Validating, OnValidating
			RemoveHandler o.VisibleChanged, OnVisibleChanged
		End Sub

		Private ReadOnly OnAutoSizeChanged As New EventHandler(Sub(s, e) RaiseEvent AutoSizeChanged(s, e))
		Private ReadOnly OnBackColorChanged As New EventHandler(Sub(s, e) RaiseEvent BackColorChanged(s, e))
		Private ReadOnly OnBackgroundImageChanged As New EventHandler(Sub(s, e) RaiseEvent BackgroundImageChanged(s, e))
		Private ReadOnly OnBackgroundImageLayoutChanged As New EventHandler(Sub(s, e) RaiseEvent BackgroundImageLayoutChanged(s, e))
		Private ReadOnly OnBindingContextChanged As New EventHandler(Sub(s, e) RaiseEvent BindingContextChanged(s, e))
		Private ReadOnly OnCausesValidationChanged As New EventHandler(Sub(s, e) RaiseEvent CausesValidationChanged(s, e))
		Private ReadOnly OnChangeUICues As New UICuesEventHandler(Sub(s, e) RaiseEvent ChangeUICues(s, e))
		Private ReadOnly OnClick As New EventHandler(Sub(s, e) RaiseEvent Click(s, e))
		Private ReadOnly OnClientSizeChanged As New EventHandler(Sub(s, e) RaiseEvent ClientSizeChanged(s, e))
		Private ReadOnly OnContextMenuChanged As New EventHandler(Sub(s, e) RaiseEvent ContextMenuChanged(s, e))
		Private ReadOnly OnContextMenuStripChanged As New EventHandler(Sub(s, e) RaiseEvent ContextMenuStripChanged(s, e))
		Private ReadOnly OnControlAdded As New ControlEventHandler(Sub(s, e) RaiseEvent ControlAdded(s, e))
		Private ReadOnly OnControlRemoved As New ControlEventHandler(Sub(s, e) RaiseEvent ControlRemoved(s, e))
		Private ReadOnly OnCursorChanged As New EventHandler(Sub(s, e) RaiseEvent CursorChanged(s, e))
		Private ReadOnly OnDisposed As New EventHandler(Sub(s, e) RaiseEvent DisposedEvent(s, e))
		Private ReadOnly OnDockChanged As New EventHandler(Sub(s, e) RaiseEvent DockChanged(s, e))
		Private ReadOnly OnDoubleClick As New EventHandler(Sub(s, e) RaiseEvent DoubleClick(s, e))
		Private ReadOnly OnDragDrop As New DragEventHandler(Sub(s, e) RaiseEvent DragDrop(s, e))
		Private ReadOnly OnDragEnter As New DragEventHandler(Sub(s, e) RaiseEvent DragEnter(s, e))
		Private ReadOnly OnDragLeave As New EventHandler(Sub(s, e) RaiseEvent DragLeave(s, e))
		Private ReadOnly OnDragOver As New DragEventHandler(Sub(s, e) RaiseEvent DragOver(s, e))
		Private ReadOnly OnEnabledChanged As New EventHandler(Sub(s, e) RaiseEvent EnabledChanged(s, e))
		Private ReadOnly OnEnter As New EventHandler(Sub(s, e) RaiseEvent Enter(s, e))
		Private ReadOnly OnFontChanged As New EventHandler(Sub(s, e) RaiseEvent FontChanged(s, e))
		Private ReadOnly OnForeColorChanged As New EventHandler(Sub(s, e) RaiseEvent ForeColorChanged(s, e))
		Private ReadOnly OnGiveFeedback As New GiveFeedbackEventHandler(Sub(s, e) RaiseEvent GiveFeedback(s, e))
		Private ReadOnly OnGotFocus As New EventHandler(Sub(s, e) RaiseEvent GotFocus(s, e))
		Private ReadOnly OnHandleCreated As New EventHandler(Sub(s, e) RaiseEvent HandleCreated(s, e))
		Private ReadOnly OnHandleDestroyed As New EventHandler(Sub(s, e) RaiseEvent HandleDestroyed(s, e))
		Private ReadOnly OnHelpRequested As New HelpEventHandler(Sub(s, e) RaiseEvent HelpRequested(s, e))
		Private ReadOnly OnImeModeChanged As New EventHandler(Sub(s, e) RaiseEvent ImeModeChanged(s, e))
		Private ReadOnly OnInvalidated As New InvalidateEventHandler(Sub(s, e) RaiseEvent Invalidated(s, e))
		Private ReadOnly OnKeyDown As New KeyEventHandler(Sub(s, e) RaiseEvent KeyDown(s, e))
		Private ReadOnly OnKeyPress As New KeyPressEventHandler(Sub(s, e) RaiseEvent KeyPress(s, e))
		Private ReadOnly OnKeyUp As New KeyEventHandler(Sub(s, e) RaiseEvent KeyUp(s, e))
		Private ReadOnly OnLayout As New LayoutEventHandler(Sub(s, e) RaiseEvent Layout(s, e))
		Private ReadOnly OnLeave As New EventHandler(Sub(s, e) RaiseEvent Leave(s, e))
		Private ReadOnly OnLocationChanged As New EventHandler(Sub(s, e) RaiseEvent LocationChanged(s, e))
		Private ReadOnly OnLostFocus As New EventHandler(Sub(s, e) RaiseEvent LostFocus(s, e))
		Private ReadOnly OnMarginChanged As New EventHandler(Sub(s, e) RaiseEvent MarginChanged(s, e))
		Private ReadOnly OnMouseCaptureChanged As New EventHandler(Sub(s, e) RaiseEvent MouseCaptureChanged(s, e))
		Private ReadOnly OnMouseClick As New MouseEventHandler(Sub(s, e) RaiseEvent MouseClick(s, e))
		Private ReadOnly OnMouseDoubleClick As New MouseEventHandler(Sub(s, e) RaiseEvent MouseDoubleClick(s, e))
		Private ReadOnly OnMouseDown As New MouseEventHandler(Sub(s, e) RaiseEvent MouseDown(s, e))
		Private ReadOnly OnMouseEnter As New EventHandler(Sub(s, e) RaiseEvent MouseEnter(s, e))
		Private ReadOnly OnMouseHover As New EventHandler(Sub(s, e) RaiseEvent MouseHover(s, e))
		Private ReadOnly OnMouseLeave As New EventHandler(Sub(s, e) RaiseEvent MouseLeave(s, e))
		Private ReadOnly OnMouseMove As New MouseEventHandler(Sub(s, e) RaiseEvent MouseMove(s, e))
		Private ReadOnly OnMouseUp As New MouseEventHandler(Sub(s, e) RaiseEvent MouseUp(s, e))
		Private ReadOnly OnMouseWheel As New MouseEventHandler(Sub(s, e) RaiseEvent MouseWheel(s, e))
		Private ReadOnly OnMove As New EventHandler(Sub(s, e) RaiseEvent Move(s, e))
		Private ReadOnly OnPaddingChanged As New EventHandler(Sub(s, e) RaiseEvent PaddingChanged(s, e))
		Private ReadOnly OnPaint As New PaintEventHandler(Sub(s, e) RaiseEvent Paint(s, e))
		Private ReadOnly OnParentChanged As New EventHandler(Sub(s, e) RaiseEvent ParentChanged(s, e))
		Private ReadOnly OnPreviewKeyDown As New PreviewKeyDownEventHandler(Sub(s, e) RaiseEvent PreviewKeyDown(s, e))
		Private ReadOnly OnQueryAccessibilityHelp As New QueryAccessibilityHelpEventHandler(Sub(s, e) RaiseEvent QueryAccessibilityHelp(s, e))
		Private ReadOnly OnQueryContinueDrag As New QueryContinueDragEventHandler(Sub(s, e) RaiseEvent QueryContinueDrag(s, e))
		Private ReadOnly OnRegionChanged As New EventHandler(Sub(s, e) RaiseEvent RegionChanged(s, e))
		Private ReadOnly OnResize As New EventHandler(Sub(s, e) RaiseEvent Resize(s, e))
		Private ReadOnly OnRightToLeftChanged As New EventHandler(Sub(s, e) RaiseEvent RightToLeftChanged(s, e))
		Private ReadOnly OnSizeChanged As New EventHandler(Sub(s, e) RaiseEvent SizeChanged(s, e))
		Private ReadOnly OnStyleChanged As New EventHandler(Sub(s, e) RaiseEvent StyleChanged(s, e))
		Private ReadOnly OnSystemColorsChanged As New EventHandler(Sub(s, e) RaiseEvent SystemColorsChanged(s, e))
		Private ReadOnly OnTabIndexChanged As New EventHandler(Sub(s, e) RaiseEvent TabIndexChanged(s, e))
		Private ReadOnly OnTabStopChanged As New EventHandler(Sub(s, e) RaiseEvent TabStopChanged(s, e))
		Private ReadOnly OnTextChanged As New EventHandler(Sub(s, e) RaiseEvent TextChanged(s, e))
		Private ReadOnly OnValidated As New EventHandler(Sub(s, e) RaiseEvent Validated(s, e))
		Private ReadOnly OnValidating As New CancelEventHandler(Sub(s, e) RaiseEvent Validating(s, e))
		Private ReadOnly OnVisibleChanged As New EventHandler(Sub(s, e) RaiseEvent VisibleChanged(s, e))

		Public Event AutoSizeChanged As EventHandler
		Public Event BackColorChanged As EventHandler
		Public Event BackgroundImageChanged As EventHandler
		Public Event BackgroundImageLayoutChanged As EventHandler
		Public Event BindingContextChanged As EventHandler
		Public Event CausesValidationChanged As EventHandler
		Public Event ChangeUICues As UICuesEventHandler
		Public Event Click As EventHandler
		Public Event ClientSizeChanged As EventHandler
		Public Event ContextMenuChanged As EventHandler
		Public Event ContextMenuStripChanged As EventHandler
		Public Event ControlAdded As ControlEventHandler
		Public Event ControlRemoved As ControlEventHandler
		Public Event CursorChanged As EventHandler
		Public Event DisposedEvent As EventHandler
		Public Event DockChanged As EventHandler
		Public Event DoubleClick As EventHandler
		Public Event DragDrop As DragEventHandler
		Public Event DragEnter As DragEventHandler
		Public Event DragLeave As EventHandler
		Public Event DragOver As DragEventHandler
		Public Event EnabledChanged As EventHandler
		Public Event Enter As EventHandler
		Public Event FontChanged As EventHandler
		Public Event ForeColorChanged As EventHandler
		Public Event GiveFeedback As GiveFeedbackEventHandler
		Public Event GotFocus As EventHandler
		Public Event HandleCreated As EventHandler
		Public Event HandleDestroyed As EventHandler
		Public Event HelpRequested As HelpEventHandler
		Public Event ImeModeChanged As EventHandler
		Public Event Invalidated As InvalidateEventHandler
		Public Event KeyDown As KeyEventHandler
		Public Event KeyPress As KeyPressEventHandler
		Public Event KeyUp As KeyEventHandler
		Public Event Layout As LayoutEventHandler
		Public Event Leave As EventHandler
		Public Event LocationChanged As EventHandler
		Public Event LostFocus As EventHandler
		Public Event MarginChanged As EventHandler
		Public Event MouseCaptureChanged As EventHandler
		Public Event MouseClick As MouseEventHandler
		Public Event MouseDoubleClick As MouseEventHandler
		Public Event MouseDown As MouseEventHandler
		Public Event MouseEnter As EventHandler
		Public Event MouseHover As EventHandler
		Public Event MouseLeave As EventHandler
		Public Event MouseMove As MouseEventHandler
		Public Event MouseUp As MouseEventHandler
		Public Event MouseWheel As MouseEventHandler
		Public Event Move As EventHandler
		Public Event PaddingChanged As EventHandler
		Public Event Paint As PaintEventHandler
		Public Event ParentChanged As EventHandler
		Public Event PreviewKeyDown As PreviewKeyDownEventHandler
		Public Event QueryAccessibilityHelp As QueryAccessibilityHelpEventHandler
		Public Event QueryContinueDrag As QueryContinueDragEventHandler
		Public Event RegionChanged As EventHandler
		Public Event Resize As EventHandler
		Public Event RightToLeftChanged As EventHandler
		Public Event SizeChanged As EventHandler
		Public Event StyleChanged As EventHandler
		Public Event SystemColorsChanged As EventHandler
		Public Event TabIndexChanged As EventHandler
		Public Event TabStopChanged As EventHandler
		Public Event TextChanged As EventHandler
		Public Event Validated As EventHandler
		Public Event Validating As CancelEventHandler
		Public Event VisibleChanged As EventHandler

	End Class
End Namespace