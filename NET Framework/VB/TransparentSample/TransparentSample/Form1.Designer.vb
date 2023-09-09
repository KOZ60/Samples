<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows フォーム デザイナーで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナーで必要です。
    'Windows フォーム デザイナーを使用して変更できます。  
    'コード エディターを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.LayeredPictureBox1 = New TransparentSample.LayeredPictureBox()
        Me.SimpleLayeredPictureBox1 = New TransparentSample.SimpleLayeredPictureBox()
        Me.TransparentablePictureBox2 = New TransparentSample.TransparentablePictureBox()
        Me.TransparentablePictureBox1 = New TransparentSample.TransparentablePictureBox()
        CType(Me.LayeredPictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SimpleLayeredPictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TransparentablePictureBox2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TransparentablePictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'LayeredPictureBox1
        '
        Me.LayeredPictureBox1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LayeredPictureBox1.BackColor = System.Drawing.Color.Gray
        Me.LayeredPictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LayeredPictureBox1.Image = Global.TransparentSample.My.Resources.Resources.megane_hikaru_woman
        Me.LayeredPictureBox1.Location = New System.Drawing.Point(479, 167)
        Me.LayeredPictureBox1.Name = "LayeredPictureBox1"
        Me.LayeredPictureBox1.Opacity = 0.6R
        Me.LayeredPictureBox1.Size = New System.Drawing.Size(214, 204)
        Me.LayeredPictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.LayeredPictureBox1.TabIndex = 3
        Me.LayeredPictureBox1.TabStop = False
        Me.LayeredPictureBox1.TransparencyKey = System.Drawing.Color.Gray
        '
        'SimpleLayeredPictureBox1
        '
        Me.SimpleLayeredPictureBox1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SimpleLayeredPictureBox1.BackColor = System.Drawing.Color.Gray
        Me.SimpleLayeredPictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.SimpleLayeredPictureBox1.Image = Global.TransparentSample.My.Resources.Resources.megane_hikaru_woman
        Me.SimpleLayeredPictureBox1.Location = New System.Drawing.Point(450, 32)
        Me.SimpleLayeredPictureBox1.Name = "SimpleLayeredPictureBox1"
        Me.SimpleLayeredPictureBox1.Size = New System.Drawing.Size(214, 204)
        Me.SimpleLayeredPictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.SimpleLayeredPictureBox1.TabIndex = 2
        Me.SimpleLayeredPictureBox1.TabStop = False
        '
        'TransparentablePictureBox2
        '
        Me.TransparentablePictureBox2.BackColor = System.Drawing.Color.Transparent
        Me.TransparentablePictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TransparentablePictureBox2.Image = Global.TransparentSample.My.Resources.Resources.megane_hikaru_woman
        Me.TransparentablePictureBox2.Location = New System.Drawing.Point(39, 167)
        Me.TransparentablePictureBox2.Name = "TransparentablePictureBox2"
        Me.TransparentablePictureBox2.Size = New System.Drawing.Size(214, 204)
        Me.TransparentablePictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.TransparentablePictureBox2.TabIndex = 1
        Me.TransparentablePictureBox2.TabStop = False
        '
        'TransparentablePictureBox1
        '
        Me.TransparentablePictureBox1.BackColor = System.Drawing.Color.Transparent
        Me.TransparentablePictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TransparentablePictureBox1.Image = Global.TransparentSample.My.Resources.Resources.megane_hikaru_woman
        Me.TransparentablePictureBox1.Location = New System.Drawing.Point(12, 32)
        Me.TransparentablePictureBox1.Name = "TransparentablePictureBox1"
        Me.TransparentablePictureBox1.Size = New System.Drawing.Size(214, 204)
        Me.TransparentablePictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.TransparentablePictureBox1.TabIndex = 0
        Me.TransparentablePictureBox1.TabStop = False
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImage = Global.TransparentSample.My.Resources.Resources.cat
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.ClientSize = New System.Drawing.Size(705, 406)
        Me.Controls.Add(Me.LayeredPictureBox1)
        Me.Controls.Add(Me.SimpleLayeredPictureBox1)
        Me.Controls.Add(Me.TransparentablePictureBox2)
        Me.Controls.Add(Me.TransparentablePictureBox1)
        Me.DoubleBuffered = True
        Me.Name = "Form1"
        Me.Text = "Form1"
        CType(Me.LayeredPictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SimpleLayeredPictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TransparentablePictureBox2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TransparentablePictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents TransparentablePictureBox1 As TransparentablePictureBox
    Friend WithEvents TransparentablePictureBox2 As TransparentablePictureBox
    Friend WithEvents SimpleLayeredPictureBox1 As SimpleLayeredPictureBox
    Friend WithEvents LayeredPictureBox1 As LayeredPictureBox
End Class
