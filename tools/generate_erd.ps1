param(
    [string]$OutFile = (Join-Path $PSScriptRoot "..\\db\\ERD.pdf")
)

$lineHeight = 10
$padding = 4
$pageWidth = 595
$pageHeight = 842

$tables = @(
    @{ Name = 'Roles'; X = 20; Y = 810; Width = 165; Fields = @('PK RoleId','Name (UQ)') },
    @{ Name = 'Users'; X = 20; Y = 730; Width = 165; Fields = @('PK UserId','FullName','Login (UQ)','Password','FK RoleId') },

    @{ Name = 'Orders'; X = 210; Y = 810; Width = 165; Fields = @('PK OrderNumber','OrderDate','DeliveryDate','FK PickupPointId','FK CustomerId','PickupCode','FK StatusId') },
    @{ Name = 'OrderItems'; X = 210; Y = 650; Width = 165; Fields = @('PK (OrderNumber, Article)','FK OrderNumber','FK Article','Quantity','UnitPrice','DiscountPercent') },
    @{ Name = 'OrderStatuses'; X = 210; Y = 520; Width = 165; Fields = @('PK StatusId','Name (UQ)') },
    @{ Name = 'PickupPoints'; X = 210; Y = 450; Width = 165; Fields = @('PK PickupPointId','PostalCode','City','Street','House') },

    @{ Name = 'Products'; X = 400; Y = 810; Width = 165; Fields = @('PK Article','Name','FK UnitId','Price','FK SupplierId','FK ManufacturerId','FK CategoryId','DiscountPercent','Description','Photo') },
    @{ Name = 'Categories'; X = 400; Y = 610; Width = 165; Fields = @('PK CategoryId','Name (UQ)') },
    @{ Name = 'Suppliers'; X = 400; Y = 540; Width = 165; Fields = @('PK SupplierId','Name (UQ)') },
    @{ Name = 'Manufacturers'; X = 400; Y = 470; Width = 165; Fields = @('PK ManufacturerId','Name (UQ)') },
    @{ Name = 'Units'; X = 400; Y = 400; Width = 165; Fields = @('PK UnitId','Name (UQ)') },
    @{ Name = 'Warehouses'; X = 400; Y = 330; Width = 165; Fields = @('PK WarehouseId','Name (UQ)') },
    @{ Name = 'Stock'; X = 400; Y = 250; Width = 165; Fields = @('PK (WarehouseId, Article)','FK WarehouseId','FK Article','Quantity') }
)

function EscapePdfText([string]$text) {
    if ($null -eq $text) { return '' }
    return $text.Replace('\\', '\\\\').Replace('(', '\\(').Replace(')', '\\)')
}

# Compute bounds
$lookup = @{}
foreach ($t in $tables) {
    $height = ($t.Fields.Count + 1) * $lineHeight + ($padding * 2)
    $t.Height = $height
    $t.Bottom = $t.Y - $height
    $t.CenterX = $t.X + ($t.Width / 2)
    $t.CenterY = $t.Bottom + ($height / 2)
    $lookup[$t.Name] = $t
}

$relations = @(
    @{ From = 'Users'; To = 'Roles' },
    @{ From = 'Orders'; To = 'Users' },
    @{ From = 'Orders'; To = 'PickupPoints' },
    @{ From = 'Orders'; To = 'OrderStatuses' },
    @{ From = 'OrderItems'; To = 'Orders' },
    @{ From = 'OrderItems'; To = 'Products' },
    @{ From = 'Products'; To = 'Units' },
    @{ From = 'Products'; To = 'Suppliers' },
    @{ From = 'Products'; To = 'Manufacturers' },
    @{ From = 'Products'; To = 'Categories' },
    @{ From = 'Stock'; To = 'Warehouses' },
    @{ From = 'Stock'; To = 'Products' }
)

$sb = New-Object System.Text.StringBuilder
$null = $sb.AppendLine('0.7 w')

# Draw table boxes and text
foreach ($t in $tables) {
    $x = [Math]::Round($t.X, 2)
    $y = [Math]::Round($t.Bottom, 2)
    $w = [Math]::Round($t.Width, 2)
    $h = [Math]::Round($t.Height, 2)
    $null = $sb.AppendLine("$x $y $w $h re S")

    $textX = $t.X + $padding
    $textY = $t.Y - $padding - $lineHeight
    $null = $sb.AppendLine("BT /F1 9 Tf $textX $textY Td (" + (EscapePdfText $t.Name) + ") Tj ET")

    $idx = 0
    foreach ($field in $t.Fields) {
        $textY = $t.Y - $padding - $lineHeight * (2 + $idx)
        $null = $sb.AppendLine("BT /F1 8 Tf $textX $textY Td (" + (EscapePdfText $field) + ") Tj ET")
        $idx++
    }
}

# Draw relationship lines
foreach ($rel in $relations) {
    $from = $lookup[$rel.From]
    $to = $lookup[$rel.To]
    if ($null -eq $from -or $null -eq $to) { continue }
    $x1 = [Math]::Round($from.CenterX, 2)
    $y1 = [Math]::Round($from.CenterY, 2)
    $x2 = [Math]::Round($to.CenterX, 2)
    $y2 = [Math]::Round($to.CenterY, 2)
    $null = $sb.AppendLine("$x1 $y1 m $x2 $y2 l S")
}

$content = $sb.ToString()
$contentBytes = [System.Text.Encoding]::ASCII.GetBytes($content)
$contentLength = $contentBytes.Length

$objects = @()
$objects += "1 0 obj`n<< /Type /Catalog /Pages 2 0 R >>`nendobj`n"
$objects += "2 0 obj`n<< /Type /Pages /Kids [3 0 R] /Count 1 >>`nendobj`n"
$objects += "3 0 obj`n<< /Type /Page /Parent 2 0 R /MediaBox [0 0 $pageWidth $pageHeight] /Resources << /Font << /F1 4 0 R >> >> /Contents 5 0 R >>`nendobj`n"
$objects += "4 0 obj`n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>`nendobj`n"
$objects += "5 0 obj`n<< /Length $contentLength >>`nstream`n$content`nendstream`nendobj`n"

# Build xref
$offsets = @()
$header = "%PDF-1.4`n"
$pos = $header.Length
$offsets += 0

foreach ($obj in $objects) {
    $offsets += $pos
    $pos += $obj.Length
}

$xrefStart = $pos

$sbOut = New-Object System.Text.StringBuilder
$null = $sbOut.Append($header)
foreach ($obj in $objects) { $null = $sbOut.Append($obj) }

$null = $sbOut.Append("xref`n0 " + ($objects.Count + 1) + "`n")
$null = $sbOut.Append("0000000000 65535 f `n")
for ($i = 1; $i -le $objects.Count; $i++) {
    $null = $sbOut.Append(($offsets[$i]).ToString('0000000000') + " 00000 n `n")
}

$null = $sbOut.Append("trailer`n<< /Size " + ($objects.Count + 1) + " /Root 1 0 R >>`nstartxref`n$xrefStart`n%%EOF")

[System.IO.File]::WriteAllText($OutFile, $sbOut.ToString(), [System.Text.Encoding]::ASCII)
Write-Host "ERD PDF written to $OutFile"
