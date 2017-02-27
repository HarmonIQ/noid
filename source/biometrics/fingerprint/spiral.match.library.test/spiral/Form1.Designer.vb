<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim ChartArea1 As System.Windows.Forms.DataVisualization.Charting.ChartArea = New System.Windows.Forms.DataVisualization.Charting.ChartArea()
        Dim Legend1 As System.Windows.Forms.DataVisualization.Charting.Legend = New System.Windows.Forms.DataVisualization.Charting.Legend()
        Dim Series1 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Me.FingerPrintGrid = New System.Windows.Forms.DataGridView()
        Me.TriangleGrid = New System.Windows.Forms.DataGridView()
        Me.Chart1 = New System.Windows.Forms.DataVisualization.Charting.Chart()
        CType(Me.FingerPrintGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TriangleGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Chart1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'FingerPrintGrid
        '
        Me.FingerPrintGrid.AllowUserToAddRows = False
        Me.FingerPrintGrid.AllowUserToDeleteRows = False
        Me.FingerPrintGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.FingerPrintGrid.Location = New System.Drawing.Point(12, 12)
        Me.FingerPrintGrid.Name = "FingerPrintGrid"
        Me.FingerPrintGrid.ReadOnly = True
        Me.FingerPrintGrid.RowTemplate.Height = 24
        Me.FingerPrintGrid.Size = New System.Drawing.Size(888, 507)
        Me.FingerPrintGrid.TabIndex = 0
        '
        'TriangleGrid
        '
        Me.TriangleGrid.AllowUserToAddRows = False
        Me.TriangleGrid.AllowUserToDeleteRows = False
        Me.TriangleGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.TriangleGrid.Location = New System.Drawing.Point(2, 543)
        Me.TriangleGrid.Name = "TriangleGrid"
        Me.TriangleGrid.ReadOnly = True
        Me.TriangleGrid.RowTemplate.Height = 24
        Me.TriangleGrid.Size = New System.Drawing.Size(1427, 150)
        Me.TriangleGrid.TabIndex = 1
        '
        'Chart1
        '
        ChartArea1.Name = "ChartArea1"
        Me.Chart1.ChartAreas.Add(ChartArea1)
        Legend1.Enabled = False
        Legend1.Name = "Legend1"
        Me.Chart1.Legends.Add(Legend1)
        Me.Chart1.Location = New System.Drawing.Point(906, 12)
        Me.Chart1.Name = "Chart1"
        Series1.ChartArea = "ChartArea1"
        Series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line
        Series1.Legend = "Legend1"
        Series1.Name = "Series1"
        Series1.YValuesPerPoint = 2
        Me.Chart1.Series.Add(Series1)
        Me.Chart1.Size = New System.Drawing.Size(532, 507)
        Me.Chart1.TabIndex = 2
        Me.Chart1.Text = "Chart1"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1441, 705)
        Me.Controls.Add(Me.Chart1)
        Me.Controls.Add(Me.TriangleGrid)
        Me.Controls.Add(Me.FingerPrintGrid)
        Me.Name = "Form1"
        Me.Text = "Form1"
        CType(Me.FingerPrintGrid, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TriangleGrid, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Chart1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents FingerPrintGrid As DataGridView
    Friend WithEvents TriangleGrid As DataGridView
    Friend WithEvents Chart1 As DataVisualization.Charting.Chart
End Class
