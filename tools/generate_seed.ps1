param(
    [string]$ImportDir = (Join-Path $PSScriptRoot "..\\import"),
    [string]$OutFile = (Join-Path $PSScriptRoot "..\db\\seed.sql")
)

function Get-XlsxSharedStrings {
    param([string]$Path)
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $zip = [System.IO.Compression.ZipFile]::OpenRead($Path)
    try {
        $sharedEntry = $zip.GetEntry('xl/sharedStrings.xml')
        if (-not $sharedEntry) { return @() }
        $sr = New-Object System.IO.StreamReader($sharedEntry.Open())
        $xml = [xml]$sr.ReadToEnd()
        $sr.Close()
        $sharedStrings = @()
        foreach ($si in $xml.sst.si) {
            if ($si.t) { $sharedStrings += [string]$si.t }
            else { $sharedStrings += (( $si.r | ForEach-Object { $_.t }) -join '') }
        }
        return $sharedStrings
    }
    finally { $zip.Dispose() }
}

function Get-XlsxRowsRaw {
    param([string]$Path)
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $zip = [System.IO.Compression.ZipFile]::OpenRead($Path)
    try {
        $sharedStrings = Get-XlsxSharedStrings -Path $Path
        $sheetEntry = $zip.GetEntry('xl/worksheets/sheet1.xml')
        if (-not $sheetEntry) { return @() }
        $sr2 = New-Object System.IO.StreamReader($sheetEntry.Open())
        $sheetXml = [xml]$sr2.ReadToEnd()
        $sr2.Close()

        $ns = New-Object System.Xml.XmlNamespaceManager($sheetXml.NameTable)
        $ns.AddNamespace('d', 'http://schemas.openxmlformats.org/spreadsheetml/2006/main')

        $rows = @()
        $rowNodes = $sheetXml.SelectNodes('//d:sheetData/d:row', $ns)
        foreach ($row in $rowNodes) {
            $rowMap = [ordered]@{}
            $cellNodes = $row.SelectNodes('d:c', $ns)
            foreach ($c in $cellNodes) {
                $ref = $c.GetAttribute('r')
                if (-not $ref) { continue }
                $col = ($ref -replace '\d','')
                $cellType = $c.GetAttribute('t')

                $valNode = $c.SelectSingleNode('d:v', $ns)
                $val = if ($valNode) { $valNode.InnerText } else { '' }

                if ($cellType -eq 's') {
                    $idx = [int]$val
                    $val = if ($idx -lt $sharedStrings.Count) { $sharedStrings[$idx] } else { '' }
                } elseif ($cellType -eq 'inlineStr') {
                    $inlineNodes = $c.SelectNodes('d:is//d:t', $ns)
                    if ($inlineNodes) {
                        $val = ($inlineNodes | ForEach-Object { $_.InnerText }) -join ''
                    }
                }

                $rowMap[$col] = [string]$val
            }
            $rows += [pscustomobject]$rowMap
        }
        return $rows
    }
    finally { $zip.Dispose() }
}

function Get-XlsxTable {
    param([string]$Path)
    $rows = Get-XlsxRowsRaw -Path $Path
    if ($rows.Count -eq 0) { return @() }
    $header = $rows[0]
    $cols = $header.PSObject.Properties.Name | Sort-Object
    $data = @()
    for ($i=1; $i -lt $rows.Count; $i++) {
        $obj = [ordered]@{}
        foreach ($col in $cols) {
            $name = $header.$col
            if (-not $name) { $name = $col }
            $obj[$name] = $rows[$i].$col
        }
        $data += [pscustomobject]$obj
    }
    return $data
}

function SqlLiteral([string]$value) {
    if ([string]::IsNullOrWhiteSpace($value)) { return "NULL" }
    $escaped = $value -replace "'", "''"
    return "N'$escaped'"
}

function ToInt([string]$value) {
    if ([string]::IsNullOrWhiteSpace($value)) { return 0 }
    return [int]$value
}

function ToDecimal([string]$value) {
    if ([string]::IsNullOrWhiteSpace($value)) { return '0' }
    return ([decimal]::Parse($value, [System.Globalization.CultureInfo]::InvariantCulture)).ToString([System.Globalization.CultureInfo]::InvariantCulture)
}

function ToDateString([string]$value) {
    if ([string]::IsNullOrWhiteSpace($value)) { return '1900-01-01' }

    $num = 0.0
    if ([double]::TryParse($value, [ref]$num)) {
        return [DateTime]::FromOADate($num).ToString('yyyy-MM-dd')
    }

    $ru = [System.Globalization.CultureInfo]::GetCultureInfo('ru-RU')
    $dt = New-Object DateTime
    if ([DateTime]::TryParse($value, $ru, [System.Globalization.DateTimeStyles]::None, [ref]$dt)) {
        return $dt.ToString('yyyy-MM-dd')
    }

    if ($value -match '^(\d{1,2})\.(\d{1,2})\.(\d{4})$') {
        $day = [int]$matches[1]
        $month = [int]$matches[2]
        $year = [int]$matches[3]
        $maxDay = [DateTime]::DaysInMonth($year, $month)
        if ($day -gt $maxDay) { $day = $maxDay }
        return (Get-Date -Year $year -Month $month -Day $day).ToString('yyyy-MM-dd')
    }

    return '1900-01-01'
}

$importPath = Resolve-Path $ImportDir
$files = Get-ChildItem -Path $importPath -Filter *.xlsx

$productsFile = $null
$usersFile = $null
$ordersFile = $null
$pickupFile = $null

foreach ($file in $files) {
    $rows = Get-XlsxRowsRaw -Path $file.FullName
    if ($rows.Count -eq 0) { continue }
    $colCount = ($rows[0].PSObject.Properties.Name).Count
    if ($colCount -eq 11) { $productsFile = $file; continue }
    if ($colCount -eq 4) { $usersFile = $file; continue }
    if ($colCount -eq 12) { $ordersFile = $file; continue }
    if ($colCount -eq 1) { $pickupFile = $file; continue }
}

if (-not $productsFile -or -not $usersFile -or -not $ordersFile -or -not $pickupFile) {
    throw "Not all import files detected. Found: products=$productsFile users=$usersFile orders=$ordersFile pickup=$pickupFile"
}

$products = Get-XlsxTable -Path $productsFile.FullName
$users = Get-XlsxTable -Path $usersFile.FullName
$orders = Get-XlsxTable -Path $ordersFile.FullName
$pickupRaw = Get-XlsxSharedStrings -Path $pickupFile.FullName

# Column names by index (avoid non-ASCII literals)
$prodCols = $products[0].PSObject.Properties.Name
$userCols = $users[0].PSObject.Properties.Name
$orderCols = $orders[0].PSObject.Properties.Name

$roles = $users | ForEach-Object { $_.$($userCols[0]) } | Where-Object { $_ } | Select-Object -Unique
$units = $products | ForEach-Object { $_.$($prodCols[2]) } | Where-Object { $_ } | Select-Object -Unique
$suppliers = $products | ForEach-Object { $_.$($prodCols[4]) } | Where-Object { $_ } | Select-Object -Unique
$manufacturers = $products | ForEach-Object { $_.$($prodCols[5]) } | Where-Object { $_ } | Select-Object -Unique
$categories = $products | ForEach-Object { $_.$($prodCols[6]) } | Where-Object { $_ } | Select-Object -Unique
$statuses = $orders | ForEach-Object { $_.$($orderCols[7]) } | Where-Object { $_ } | Where-Object { $_ -ne 'System.Xml.XmlElement' } | Select-Object -Unique
if ($statuses.Count -eq 0) { $statuses = @('Завершен') }

$sb = New-Object System.Text.StringBuilder
$null = $sb.AppendLine('SET NOCOUNT ON;')
$null = $sb.AppendLine('')
$null = $sb.AppendLine("INSERT INTO dbo.Warehouses (Name) VALUES (N'Основной склад');")
$null = $sb.AppendLine('')

# Roles
if ($roles.Count -gt 0) {
    $roleValues = $roles | ForEach-Object { "(" + (SqlLiteral $_) + ")" }
    $null = $sb.AppendLine("INSERT INTO dbo.Roles (Name) VALUES $($roleValues -join ', ');")
    $null = $sb.AppendLine('')
}

# Users
foreach ($user in $users) {
    $roleName = $user.$($userCols[0])
    $fullName = $user.$($userCols[1])
    $login = $user.$($userCols[2])
    $password = $user.$($userCols[3])
    if ([string]::IsNullOrWhiteSpace($roleName) -or [string]::IsNullOrWhiteSpace($login)) { continue }
    $null = $sb.AppendLine("INSERT INTO dbo.Users (FullName, Login, Password, RoleId)")
    $null = $sb.AppendLine("VALUES (" + (SqlLiteral $fullName) + ", " + (SqlLiteral $login) + ", " + (SqlLiteral $password) + ", (SELECT RoleId FROM dbo.Roles WHERE Name = " + (SqlLiteral $roleName) + "));")
}
$null = $sb.AppendLine('')

# Reference tables
if ($units.Count -gt 0) {
    $unitValues = $units | ForEach-Object { "(" + (SqlLiteral $_) + ")" }
    $null = $sb.AppendLine("INSERT INTO dbo.Units (Name) VALUES $($unitValues -join ', ');")
    $null = $sb.AppendLine('')
}
if ($suppliers.Count -gt 0) {
    $supplierValues = $suppliers | ForEach-Object { "(" + (SqlLiteral $_) + ")" }
    $null = $sb.AppendLine("INSERT INTO dbo.Suppliers (Name) VALUES $($supplierValues -join ', ');")
    $null = $sb.AppendLine('')
}
if ($manufacturers.Count -gt 0) {
    $manufacturerValues = $manufacturers | ForEach-Object { "(" + (SqlLiteral $_) + ")" }
    $null = $sb.AppendLine("INSERT INTO dbo.Manufacturers (Name) VALUES $($manufacturerValues -join ', ');")
    $null = $sb.AppendLine('')
}
if ($categories.Count -gt 0) {
    $categoryValues = $categories | ForEach-Object { "(" + (SqlLiteral $_) + ")" }
    $null = $sb.AppendLine("INSERT INTO dbo.Categories (Name) VALUES $($categoryValues -join ', ');")
    $null = $sb.AppendLine('')
}

# Products
foreach ($product in $products) {
    $article = $product.$($prodCols[0])
    if ([string]::IsNullOrWhiteSpace($article)) { continue }
    $name = $product.$($prodCols[1])
    $unitName = $product.$($prodCols[2])
    $price = ToDecimal $product.$($prodCols[3])
    $supplierName = $product.$($prodCols[4])
    $manufacturerName = $product.$($prodCols[5])
    $categoryName = $product.$($prodCols[6])
    $discount = ToInt $product.$($prodCols[7])
    $qty = ToInt $product.$($prodCols[8])
    $description = $product.$($prodCols[9])
    $photo = $product.$($prodCols[10])

    $null = $sb.AppendLine("INSERT INTO dbo.Products (Article, Name, UnitId, Price, SupplierId, ManufacturerId, CategoryId, DiscountPercent, Description, Photo)")
    $null = $sb.AppendLine("VALUES (" + (SqlLiteral $article) + ", " + (SqlLiteral $name) + ", (SELECT UnitId FROM dbo.Units WHERE Name = " + (SqlLiteral $unitName) + "), " + $price + ", (SELECT SupplierId FROM dbo.Suppliers WHERE Name = " + (SqlLiteral $supplierName) + "), (SELECT ManufacturerId FROM dbo.Manufacturers WHERE Name = " + (SqlLiteral $manufacturerName) + "), (SELECT CategoryId FROM dbo.Categories WHERE Name = " + (SqlLiteral $categoryName) + "), " + $discount + ", " + (SqlLiteral $description) + ", " + (SqlLiteral $photo) + ");")

    $null = $sb.AppendLine("INSERT INTO dbo.Stock (WarehouseId, Article, Quantity)")
    $null = $sb.AppendLine("VALUES ((SELECT WarehouseId FROM dbo.Warehouses WHERE Name = N'Основной склад'), " + (SqlLiteral $article) + ", " + $qty + ");")
}
$null = $sb.AppendLine('')

# Pickup points
foreach ($raw in $pickupRaw) {
    if ([string]::IsNullOrWhiteSpace($raw)) { continue }
    $parts = $raw -split '\s*,\s*'
    $postal = if ($parts.Count -ge 1) { $parts[0] } else { '' }
    $city = if ($parts.Count -ge 2) { $parts[1] } else { '' }
    $street = if ($parts.Count -ge 3) { $parts[2] } else { '' }
    $house = if ($parts.Count -ge 4) { $parts[3] } else { '' }
    $null = $sb.AppendLine("INSERT INTO dbo.PickupPoints (PostalCode, City, Street, House) VALUES (" + (SqlLiteral $postal) + ", " + (SqlLiteral $city) + ", " + (SqlLiteral $street) + ", " + (SqlLiteral $house) + ");")
}
$null = $sb.AppendLine('')

# Order statuses
if ($statuses.Count -gt 0) {
    $statusValues = $statuses | ForEach-Object { "(" + (SqlLiteral $_) + ")" }
    $null = $sb.AppendLine("INSERT INTO dbo.OrderStatuses (Name) VALUES $($statusValues -join ', ');")
    $null = $sb.AppendLine('')
}

# Orders
foreach ($order in $orders) {
    $orderNumber = ToInt $order.$($orderCols[0])
    if ($orderNumber -le 0) { continue }
    $orderDate = ToDateString $order.$($orderCols[2])
    $deliveryDate = ToDateString $order.$($orderCols[3])
    $pickupPointId = ToInt $order.$($orderCols[4])
    $customerName = $order.$($orderCols[5])
    $pickupCode = $order.$($orderCols[6])
    $statusName = $order.$($orderCols[7])
    if ([string]::IsNullOrWhiteSpace($statusName) -or $statusName -eq 'System.Xml.XmlElement') { $statusName = 'Завершен' }

    $null = $sb.AppendLine("INSERT INTO dbo.Orders (OrderNumber, OrderDate, DeliveryDate, PickupPointId, CustomerId, PickupCode, StatusId)")
    $null = $sb.AppendLine("VALUES (" + $orderNumber + ", '" + $orderDate + "', '" + $deliveryDate + "', " + $pickupPointId + ", (SELECT UserId FROM dbo.Users WHERE FullName = " + (SqlLiteral $customerName) + "), " + (SqlLiteral $pickupCode) + ", (SELECT StatusId FROM dbo.OrderStatuses WHERE Name = " + (SqlLiteral $statusName) + "));")
}
$null = $sb.AppendLine('')

# Order items
foreach ($order in $orders) {
    $orderNumber = ToInt $order.$($orderCols[0])
    if ($orderNumber -le 0) { continue }
    $rawItems = $order.$($orderCols[1])
    if ([string]::IsNullOrWhiteSpace($rawItems)) { continue }
    $parts = $rawItems -split ',' | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne '' }
    for ($i = 0; $i -lt $parts.Count; $i += 2) {
        if ($i + 1 -ge $parts.Count) { break }
        $article = $parts[$i]
        $qty = ToInt $parts[$i + 1]
        $null = $sb.AppendLine("INSERT INTO dbo.OrderItems (OrderNumber, Article, Quantity, UnitPrice, DiscountPercent)")
        $null = $sb.AppendLine("SELECT " + $orderNumber + ", p.Article, " + $qty + ", p.Price, p.DiscountPercent FROM dbo.Products p WHERE p.Article = " + (SqlLiteral $article) + ";")
    }
}

$null = $sb.AppendLine('')

Set-Content -Path $OutFile -Value $sb.ToString() -Encoding UTF8
Write-Host "Seed file written to $OutFile"




