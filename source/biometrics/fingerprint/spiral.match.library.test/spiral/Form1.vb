Public Class Form1

    Private FingerPrintDS As System.Data.DataSet = New System.Data.DataSet
    Private FingerPrintDT As System.Data.DataTable
    Private TriangleDT As System.Data.DataTable

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim FingerPrintPrimaryKey As System.Data.DataColumn
        Dim FingerPrintData = {
                                    {152, 124, 10, 0}, {182, 117, 235, 0}, {282, 97, 215, 0}, {67, 16, 239, 0}, {216, 263, 0, 0}, {227, 179, 242, 0}, {226, 95, 210, 0}, {201, 230, 2, 0},
                                    {200, 77, 72, 0}, {202, 124, 222, 1}, {154, 117, 9, 0}, {213, 60, 66, 1}, {170, 140, 8, 1}, {220, 48, 132, 0}, {78, 130, 37, 0}, {273, 37, 91, 0},
                                    {140, 61, 13, 0}, {136, 235, 24, 0}, {251, 50, 86, 0}, {207, 330, 6, 0}, {207, 89, 194, 0}, {78, 66, 148, 0}, {240, 22, 130, 0}, {128, 152, 34, 0},
                                    {285, 49, 221, 0}, {254, 244, 121, 0}, {174, 71, 54, 1}, {78, 47, 23, 0}, {100, 251, 161, 0}, {218, 59, 57, 1}, {139, 251, 28, 0}, {283, 105, 94, 0},
                                    {127, 120, 32, 0}, {77, 18, 8, 0}, {85, 291, 32, 0}, {176, 123, 137, 0}, {93, 106, 35, 0}, {161, 88, 240, 0}, {57, 200, 35, 0}, {233, 137, 226, 0},
                                    {147, 114, 162, 0}, {213, 202, 249, 1}, {141, 54, 145, 0}, {145, 95, 157, 0}, {73, 71, 22, 0}, {135, 207, 27, 0}
                                }
        Dim SingularPointX As Integer = 150
        Dim SingularPointY As Integer = 150

        FingerPrintDS.DataSetName = "FingerPrint"
        FingerPrintDT = FingerPrintDS.Tables.Add("PatientPrint")

        FingerPrintPrimaryKey = FingerPrintDT.Columns.Add()
        FingerPrintDT.PrimaryKey = New DataColumn() {FingerPrintPrimaryKey}

        With FingerPrintPrimaryKey
            .ColumnName = "FingerPrintSID"
            .DataType = GetType(Integer)
            .Unique = True
            .AutoIncrement = True
            .AutoIncrementSeed = 1
            .AutoIncrementStep = 1
            .AllowDBNull = False
        End With

        With FingerPrintDT.Columns.Add()
            .ColumnName = "X"
            .DataType = GetType(Integer)
            .AllowDBNull = True
        End With

        With FingerPrintDT.Columns.Add()
            .ColumnName = "Y"
            .DataType = GetType(Integer)
            .AllowDBNull = True
        End With

        With FingerPrintDT.Columns.Add()
            .ColumnName = "Angle"
            .DataType = GetType(Integer)
            .AllowDBNull = True
        End With

        With FingerPrintDT.Columns.Add()
            .ColumnName = "Minutia_Type"
            .DataType = GetType(Integer)
            .AllowDBNull = True
        End With

        With FingerPrintDT.Columns.Add()
            .ColumnName = "Distance"
            .DataType = GetType(Decimal)
            .AllowDBNull = True
        End With

        Dim FingerPrintRow As System.Data.DataRow
        Dim counter As Integer = 45
        Dim Distance As Decimal
        Dim X As Integer = 0
        Dim Y As Integer = 0

        For count = 0 To counter

            FingerPrintRow = FingerPrintDT.NewRow
            X = FingerPrintData.GetValue(count, 1)
            Y = FingerPrintData.GetValue(count, 2)
            Distance = 0.0

            FingerPrintRow(1) = FingerPrintData.GetValue(count, 0)
            FingerPrintRow(2) = X
            FingerPrintRow(3) = Y
            FingerPrintRow(4) = FingerPrintData.GetValue(count, 3)

            'Pythagorean theorem for distance between two points
            Distance = Math.Sqrt(Math.Pow((X - SingularPointX), 2.0) + Math.Pow((Y - SingularPointY), 2.0))
            FingerPrintRow(5) = Distance

            FingerPrintDT.Rows.Add(FingerPrintRow)

        Next

        FingerPrintGrid.DataSource = FingerPrintDS.Tables(0)
        FingerPrintGrid.Sort(FingerPrintGrid.Columns(5), System.ComponentModel.ListSortDirection.Ascending)

        Dim DistanceArray As Decimal() = New Decimal(FingerPrintDT.Rows.Count - 1) {}
        Dim arraycount As Integer = 0

        For Each row As System.Data.DataRow In FingerPrintDT.Rows
            DistanceArray(arraycount) = row.ItemArray(5)
            arraycount = arraycount + 1
        Next

        Array.Sort(DistanceArray)

        CreateSpiral(DistanceArray, 10.0)

    End Sub

    Function CreateSpiral(SortedDistance As Decimal(), UserKey As Decimal)

        Dim SizeOfArray As Integer = 0
        SizeOfArray = SortedDistance.Length + 1

        Dim KeyedDistance As Decimal() = New Decimal(SizeOfArray) {}
        Dim KeyedCounter As Integer = 0

        Dim SumOfThetas As Decimal = 0

        KeyedDistance(KeyedCounter) = UserKey
        KeyedCounter = KeyedCounter + 1

        CreateTriangleTable()

        Dim TriangleTableRow As System.Data.DataRow

        For Each Distance As Decimal In SortedDistance

            TriangleTableRow = TriangleDT.NewRow

            TriangleTableRow(1) = KeyedDistance(KeyedCounter - 1)

            KeyedDistance(KeyedCounter) = Distance + UserKey

            'This branching looks superfluous
            If KeyedCounter = 1 Then
                TriangleTableRow(2) = KeyedDistance(KeyedCounter)
            Else
                TriangleTableRow(2) = KeyedDistance(KeyedCounter)
            End If

            'This next distance is not needed for final calculations just good when drawing triangles
            TriangleTableRow(3) = Math.Sqrt(Math.Pow(TriangleTableRow(1), 2.0) + Math.Pow(TriangleTableRow(2), 2.0))

            TriangleTableRow(4) = Math.Atan2(TriangleTableRow(1), TriangleTableRow(2)) * (180 / Math.PI)

            SumOfThetas = SumOfThetas + TriangleTableRow(4)
            TriangleTableRow(5) = SumOfThetas

            'This is the end point coordinates of the Line
            TriangleTableRow(6) = Math.Sin(((Math.PI / 180) * TriangleTableRow(5)))
            TriangleTableRow(7) = Math.Cos(((Math.PI / 180) * TriangleTableRow(5)))

            TriangleDT.Rows.Add(TriangleTableRow)

            Dim dp As System.Windows.Forms.DataVisualization.Charting.DataPoint = New System.Windows.Forms.DataVisualization.Charting.DataPoint()
            dp.SetValueXY(TriangleTableRow(8), TriangleTableRow(9))
            Chart1.Series(0).Points.Add(dp)

            KeyedCounter = KeyedCounter + 1

        Next

        'Dim I As Integer = 0
        'Dim J As Integer = 0

        'Dim Leg As Decimal() = New Decimal(SizeOfArray) {}

        'Dim X As Decimal() = New Decimal(SizeOfArray) {}
        'Dim Y As Decimal() = New Decimal(SizeOfArray) {}
        'Dim Theta As Decimal() = New Decimal(SizeOfArray) {}

        'TriangleTableRow(3) = -Math.Atan2(TriangleTableRow(1), TriangleTableRow(2)) * (180 / Math.PI)
        'TriangleTableRow(3) = Math.Atan(TriangleTableRow(2) / TriangleTableRow(1)) * (180 / Math.PI)
        'TriangleTableRow(4) = Math.Atan(TriangleTableRow(1) / TriangleTableRow(2)) * (180 / Math.PI)

        'TriangleTableRow(3) = Math.Atan2(TriangleTableRow(2), TriangleTableRow(1)) * (180 / Math.PI)
        'TriangleTableRow(4) = Math.Atan2(TriangleTableRow(1), TriangleTableRow(2)) * (180 / Math.PI)

        'TriangleTableRow(6) = Math.Sin(((Math.PI / 180) * TriangleTableRow(3)))
        'TriangleTableRow(7) = Math.Cos(((Math.PI / 180) * TriangleTableRow(3)))

        'TriangleTableRow(8) = Math.Sqrt(Math.Pow(TriangleTableRow(1), 2.0) + Math.Pow(TriangleTableRow(2), 2.0))

        'Leg(0) = UserKey
        'Theta(0) = 0.0

        'For I = 1 To (SizeOfArray - 1)

        '    If I = 1 Then

        '        Leg(I) = Math.Sqrt(Math.Pow(KeyedDistance(I), 2.0) + Math.Pow(KeyedDistance(0), 2.0))
        '        'X(0) = KeyedDistance(I)
        '        'Y(0) = Leg(I)
        '        'Theta(I) = Math.Atan()

        '    End If

        '    If I > 1 Then

        '        Leg(I) = Math.Sqrt(Math.Pow(KeyedDistance(I + 1), 2.0) + Math.Pow(KeyedDistance(I), 2.0))
        '        Theta(I) = -Math.Atan(Leg(I - 1) / KeyedDistance(I - 1))
        '        Theta(I) = Theta(I) + Theta(I - 1)

        '        '        Theta(I) = Leg(I) / KeyedDistance(I)
        '        '        X(I) = KeyedDistance(I)
        '        '        Y(1) = Leg(I)

        '        '        For J = 1 To 4

        '        '        Next

        '    End If

        'Next

        TriangleGrid.DataSource = FingerPrintDS.Tables(1)

        Return 1

    End Function

    Sub CreateTriangleTable()

        Dim TrianglePrimaryKey As System.Data.DataColumn

        TriangleDT = FingerPrintDS.Tables.Add("Triangle")

        TrianglePrimaryKey = TriangleDT.Columns.Add()
        TriangleDT.PrimaryKey = New DataColumn() {TrianglePrimaryKey}

        With TrianglePrimaryKey
            .ColumnName = "TriangleSID"
            .DataType = GetType(Integer)
            .Unique = True
            .AutoIncrement = True
            .AutoIncrementSeed = 1
            .AutoIncrementStep = 1
            .AllowDBNull = False
        End With

        With TriangleDT.Columns.Add()
            .ColumnName = "Leg_1"
            .DataType = GetType(Decimal)
            .AllowDBNull = True
        End With

        With TriangleDT.Columns.Add()
            .ColumnName = "Leg_2"
            .DataType = GetType(Decimal)
            .AllowDBNull = True
        End With

        With TriangleDT.Columns.Add()
            .ColumnName = "Leg_3"
            .DataType = GetType(Decimal)
            .AllowDBNull = True
        End With

        With TriangleDT.Columns.Add()
            .ColumnName = "Theta_2"
            .DataType = GetType(Decimal)
            .AllowDBNull = True
        End With

        With TriangleDT.Columns.Add()
            .ColumnName = "Sum_Of_Theta_2"
            .DataType = GetType(Decimal)
            .AllowDBNull = True
        End With

        With TriangleDT.Columns.Add()
            .ColumnName = "Sin"
            .DataType = GetType(Decimal)
            .AllowDBNull = True
        End With

        With TriangleDT.Columns.Add()
            .ColumnName = "Cos"
            .DataType = GetType(Decimal)
            .AllowDBNull = True
        End With

        With TriangleDT.Columns.Add()
            .ColumnName = "Leg_2_Cos_x"
            .DataType = GetType(Decimal)
            .AllowDBNull = True
            .Expression = "Cos * Leg_2"
        End With

        With TriangleDT.Columns.Add()
            .ColumnName = "Leg_2_Sin_y"
            .DataType = GetType(Decimal)
            .AllowDBNull = True
            .Expression = "Sin * Leg_2"
        End With

        'With TriangleDT.Columns.Add()
        '    .ColumnName = "Theta_1"
        '    .DataType = GetType(Decimal)
        '    .AllowDBNull = True
        'End With

        'With TriangleDT.Columns.Add()
        '    .ColumnName = "Theta_3"
        '    .DataType = GetType(Decimal)
        '    .AllowDBNull = True
        '    .DefaultValue = 90.0
        'End With

    End Sub

End Class
