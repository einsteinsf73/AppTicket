$excelFilePath = "U:\T.I\Software Ticket\ControlePatrimonio.xlsx"
$csvFilePath = "U:\T.I\Software Ticket\ControlePatrimonio.csv"

$excel = New-Object -ComObject Excel.Application
$excel.Visible = $false
$excel.DisplayAlerts = $false

$workbook = $excel.Workbooks.Open($excelFilePath)
$workbook.SaveAs($csvFilePath, 6)

$workbook.Close()
$excel.Quit()

[System.Runtime.InteropServices.Marshal]::ReleaseComObject($workbook) | Out-Null
[System.Runtime.InteropServices.Marshal]::ReleaseComObject($excel) | Out-Null
Remove-Variable excel, workbook -ErrorAction SilentlyContinue