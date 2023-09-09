Imports System.ComponentModel
Imports System.Runtime.InteropServices

''' <summary>
''' 背景を透過する PictureBox
''' </summary>
Public Class TransparentPictureBox
    Inherits PictureBox

    ' 親の描画タイミングを検知するための NativeWindow
    Private _ParentNativeWindow As ParentNativeWindow

    Public Sub New()
        _ParentNativeWindow = New ParentNativeWindow(Me)
        SetStyle(ControlStyles.ResizeRedraw, True)
    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        MyBase.Dispose(disposing)
        If _ParentNativeWindow IsNot Nothing Then
            _ParentNativeWindow.ReleaseHandle()
            _ParentNativeWindow = Nothing
        End If
    End Sub

    ''' <summary>
    ''' 背景に関するプロパティを使えなくする
    ''' </summary>
    <Obsolete, Browsable(False),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Overrides Property BackgroundImageLayout As ImageLayout
    <Obsolete, Browsable(False),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Overrides Property BackgroundImage As Image
    <Obsolete, Browsable(False),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Overrides Property BackColor As Color

    ' 背景を描画
    Protected Overrides Sub OnPaintBackground(pevent As PaintEventArgs)
        If Parent IsNot Nothing Then
            DrawBackground(pevent.Graphics)
        Else
            MyBase.OnPaintBackground(pevent)
        End If
    End Sub

    Private Sub DrawBackground(g As Graphics)
        ' (1) 親を描画
        DrawParent(g)
        ' (2) 自分と同じ階層のコントロールを描画
        ' Control.Controls プロパティは Z オーダー順に並んでいるはずだが
        ' SetWindowPos 等、API による並び替えがあるとずれてしまうので
        ' EnumChildWindows で列挙する
        ' 後ろから列挙し、自分が出てきたら終了
        Dim ctls = GetSameHierarchyList()
        For i = ctls.Count - 1 To 0 Step -1
            Dim ctrl As Control = ctls(i)
            If ctrl Is Me Then Exit For
            If ctrl.Visible Then
                DrawSameHierarchy(g, ctrl)
            End If
        Next
    End Sub

    Private Sub DrawParent(g As Graphics)
        ' 描画エリアを取得(親コントロールの座標)
        Dim cr = Parent.ClientRectangle
        Dim clip = GetClip(cr)

        If clip.Width > 0 AndAlso clip.Height > 0 Then
            Using bmp As New Bitmap(cr.Width, cr.Height,
                                    Imaging.PixelFormat.Format32bppRgb)
                ' ビットマップに親を描画
                Using bmpG = Graphics.FromImage(bmp)
                    Dim hdc = bmpG.GetHdc()
                    Dim printParam = PRF_CLIENT Or PRF_ERASEBKGND
                    SendMessage(Parent.Handle, WM_PRINTCLIENT,
                                hdc, New IntPtr(printParam))
                    bmpG.ReleaseHdc()
                End Using

                ' ビットマップを切り出して描画
                Dim destRect = MapRectangle(Parent, clip, Me)
                g.DrawImage(bmp, destRect, clip, GraphicsUnit.Pixel)
            End Using
        End If
    End Sub

    Private Function GetClip(r As Rectangle) As Rectangle
        Dim mapRect = MapRectangle(Me, ClientRectangle, Parent)
        Dim clip = Rectangle.Intersect(r, mapRect)
        Return clip
    End Function

    Private Function GetSameHierarchyList() As List(Of Control)
        Dim list As New List(Of Control)
        Dim gch = GCHandle.Alloc(list)
        EnumChildWindows(Parent.Handle,
                         New EnumWindowsProc(AddressOf EnumSameHierarchyProc),
                         GCHandle.ToIntPtr(gch))
        gch.Free()
        Return list
    End Function

    Private Function EnumSameHierarchyProc(hWnd As IntPtr, lParam As IntPtr) As Boolean
        Dim gch = GCHandle.FromIntPtr(lParam)
        Dim list = DirectCast(gch.Target, List(Of Control))
        Dim ctrl = FromHandle(hWnd)
        If ctrl IsNot Nothing Then
            list.Add(ctrl)
        End If
        Return True
    End Function

    Private Sub DrawSameHierarchy(g As Graphics, ctrl As Control)
        ' 描画エリアを取得(親コントロールの座標)
        Dim cr = ctrl.Bounds
        Dim clip = GetClip(cr)

        If clip.Width > 0 AndAlso clip.Height > 0 Then
            Using bmp As New Bitmap(cr.Width, cr.Height,
                                    Imaging.PixelFormat.Format32bppRgb)
                ' ビットマップにコントロールを描画
                ctrl.DrawToBitmap(bmp, New Rectangle(Point.Empty, ctrl.Size))

                ' ビットマップを切り出して描画
                Dim destRect = MapRectangle(Parent, clip, Me)
                Dim srcRect = New Rectangle(
                                clip.Location - CType(cr.Location, Size), clip.Size)
                g.DrawImage(bmp, destRect, srcRect, GraphicsUnit.Pixel)
            End Using
        End If
    End Sub

    Private Function MapRectangle(fromCtrl As Control,
                                  fromRect As Rectangle,
                                  toCtrl As Control) As Rectangle
        Dim rect As New RECT(fromRect)
        MapWindowPoints(fromCtrl.Handle, toCtrl.Handle, rect, 2)
        Return rect.ToRectangle()
    End Function

    Private Const WM_PAINT = &HF,
                  WM_PRINTCLIENT = &H318

    Private Const PRF_CHECKVISIBLE = &H1,
                  PRF_NONCLIENT = &H2,
                  PRF_CLIENT = &H4,
                  PRF_ERASEBKGND = &H8,
                  PRF_CHILDREN = &H10

    <StructLayout(LayoutKind.Sequential)>
    Private Structure RECT
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

    <DllImport("user32")>
    Private Shared Function MapWindowPoints(
                        hwndFrom As IntPtr, hwndTo As IntPtr,
                        ByRef RECT As RECT, cPoints As Integer) As Integer
    End Function

    <DllImport("user32", CharSet:=CharSet.Auto)>
    Private Shared Function SendMessage(
                        hwnd As IntPtr, msg As Integer,
                        wparam As IntPtr, lparam As IntPtr) As IntPtr
    End Function

    Private Delegate Function EnumWindowsProc(hWnd As IntPtr, lParam As IntPtr) As Boolean

    <DllImport("user32.dll", CharSet:=CharSet.Auto)>
    Private Shared Function EnumChildWindows(
                        hWndParent As IntPtr,
                        lpEnumFunc As EnumWindowsProc,
                        lParam As IntPtr) As Boolean
    End Function

    ' 親の描画タイミングを検知するための NativeWindow
    ' Parent.Paint イベントではタイミング的に遅く、
    ' Invalidate を実行するとループに入ってしまう
    Private Class ParentNativeWindow
        Inherits NativeWindow

        ' こういうとき WithEvents は便利。C# だと面倒
        Private WithEvents Owner As TransparentPictureBox
        Private WithEvents Parent As Control

        Public Sub New(owner As TransparentPictureBox)
            Me.Owner = owner
        End Sub

        Private Sub Owner_ParentChanged(sender As Object, e As EventArgs) Handles Owner.ParentChanged
            Parent = Owner.Parent
            If Parent IsNot Nothing AndAlso Parent.IsHandleCreated Then
                AssignHandle(Parent.Handle)
            End If
        End Sub

        Private Sub Owner_LocationChanged(sender As Object, e As EventArgs) Handles Owner.LocationChanged
            Owner.Invalidate()
        End Sub

        Private Sub Owner_Disposed(sender As Object, e As EventArgs) Handles Owner.Disposed
            Owner = Nothing
        End Sub

        Private Sub Parent_Disposed(sender As Object, e As EventArgs) Handles Parent.Disposed
            Parent = Nothing
        End Sub

        Protected Overrides Sub WndProc(ByRef m As Message)
            ' WM_PAINT が来たら Invalidate()
            If m.Msg = WM_PAINT Then
                Owner.Invalidate()
            End If
            MyBase.WndProc(m)
        End Sub

        Private Sub Parent_OnHandleCreated(sender As Object, e As EventArgs) Handles Parent.HandleCreated
            AssignHandle(Parent.Handle)
        End Sub

        Private Sub Parent_OnHandleDestroyed(sender As Object, e As EventArgs) Handles Parent.HandleDestroyed
            ReleaseHandle()
        End Sub

    End Class

End Class
