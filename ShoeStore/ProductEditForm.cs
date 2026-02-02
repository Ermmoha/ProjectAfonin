using System.Data;
using System.Drawing;
using Microsoft.Data.SqlClient;

namespace ShoeStore;

public class ProductEditForm : Form
{
    private readonly bool _isEdit;
    private readonly string? _article;

    private TextBox articleTextBox = null!;
    private TextBox nameTextBox = null!;
    private ComboBox unitComboBox = null!;
    private ComboBox supplierComboBox = null!;
    private ComboBox manufacturerComboBox = null!;
    private ComboBox categoryComboBox = null!;
    private NumericUpDown priceNumeric = null!;
    private NumericUpDown discountNumeric = null!;
    private NumericUpDown stockNumeric = null!;
    private TextBox descriptionTextBox = null!;
    private TextBox photoTextBox = null!;
    private Button saveButton = null!;
    private Button cancelButton = null!;

    public ProductEditForm(string? article = null)
    {
        _article = article;
        _isEdit = !string.IsNullOrWhiteSpace(article);
        InitializeComponent();
        UiTheme.Apply(this);
        UiTheme.StyleAccent(saveButton);
        UiTheme.StyleAccent(cancelButton);
        LoadCombos();

        if (_isEdit)
        {
            Text = "Редактирование товара";
            articleTextBox.ReadOnly = true;
            LoadProduct(article!);
        }
        else
        {
            Text = "Добавление товара";
        }
    }

    private void InitializeComponent()
    {
        ClientSize = new Size(680, 520);
        StartPosition = FormStartPosition.CenterParent;

        var labelWidth = 140;
        var inputWidth = 440;
        var left = 20;
        var top = 20;
        var rowHeight = 28;
        var gap = 8;

        Controls.Add(MakeLabel("Артикул", left, top));
        articleTextBox = MakeTextBox(left + labelWidth, top, inputWidth);
        Controls.Add(articleTextBox);
        top += rowHeight + gap;

        Controls.Add(MakeLabel("Наименование", left, top));
        nameTextBox = MakeTextBox(left + labelWidth, top, inputWidth);
        Controls.Add(nameTextBox);
        top += rowHeight + gap;

        Controls.Add(MakeLabel("Ед. изм.", left, top));
        unitComboBox = MakeComboBox(left + labelWidth, top, inputWidth);
        Controls.Add(unitComboBox);
        top += rowHeight + gap;

        Controls.Add(MakeLabel("Поставщик", left, top));
        supplierComboBox = MakeComboBox(left + labelWidth, top, inputWidth);
        Controls.Add(supplierComboBox);
        top += rowHeight + gap;

        Controls.Add(MakeLabel("Производитель", left, top));
        manufacturerComboBox = MakeComboBox(left + labelWidth, top, inputWidth);
        Controls.Add(manufacturerComboBox);
        top += rowHeight + gap;

        Controls.Add(MakeLabel("Категория", left, top));
        categoryComboBox = MakeComboBox(left + labelWidth, top, inputWidth);
        Controls.Add(categoryComboBox);
        top += rowHeight + gap;

        Controls.Add(MakeLabel("Цена", left, top));
        priceNumeric = new NumericUpDown { Location = new Point(left + labelWidth, top), Width = inputWidth, Maximum = 1000000, DecimalPlaces = 2 };
        Controls.Add(priceNumeric);
        top += rowHeight + gap;

        Controls.Add(MakeLabel("Скидка %", left, top));
        discountNumeric = new NumericUpDown { Location = new Point(left + labelWidth, top), Width = inputWidth, Maximum = 100 };
        Controls.Add(discountNumeric);
        top += rowHeight + gap;

        Controls.Add(MakeLabel("Остаток", left, top));
        stockNumeric = new NumericUpDown { Location = new Point(left + labelWidth, top), Width = inputWidth, Maximum = 100000 };
        Controls.Add(stockNumeric);
        top += rowHeight + gap;

        Controls.Add(MakeLabel("Описание", left, top));
        descriptionTextBox = new TextBox { Location = new Point(left + labelWidth, top), Width = inputWidth, Height = 60, Multiline = true, ScrollBars = ScrollBars.Vertical };
        Controls.Add(descriptionTextBox);
        top += 60 + gap;

        Controls.Add(MakeLabel("Фото", left, top));
        photoTextBox = MakeTextBox(left + labelWidth, top, inputWidth);
        Controls.Add(photoTextBox);
        top += rowHeight + gap;

        saveButton = new Button { Text = "Сохранить", Location = new Point(left + labelWidth, top + 10), Size = new Size(160, 34) };
        cancelButton = new Button { Text = "Отмена", Location = new Point(left + labelWidth + 180, top + 10), Size = new Size(160, 34) };
        saveButton.Click += OnSave;
        cancelButton.Click += (_, _) => DialogResult = DialogResult.Cancel;
        Controls.Add(saveButton);
        Controls.Add(cancelButton);
    }

    private static Label MakeLabel(string text, int x, int y)
    {
        return new Label { Text = text, Location = new Point(x, y + 4), Width = 140 };
    }

    private static TextBox MakeTextBox(int x, int y, int width)
    {
        return new TextBox { Location = new Point(x, y), Width = width };
    }

    private static ComboBox MakeComboBox(int x, int y, int width)
    {
        return new ComboBox { Location = new Point(x, y), Width = width, DropDownStyle = ComboBoxStyle.DropDownList };
    }

    private void LoadCombos()
    {
        FillCombo(unitComboBox, "SELECT UnitId AS Id, Name FROM dbo.Units ORDER BY Name");
        FillCombo(supplierComboBox, "SELECT SupplierId AS Id, Name FROM dbo.Suppliers ORDER BY Name");
        FillCombo(manufacturerComboBox, "SELECT ManufacturerId AS Id, Name FROM dbo.Manufacturers ORDER BY Name");
        FillCombo(categoryComboBox, "SELECT CategoryId AS Id, Name FROM dbo.Categories ORDER BY Name");
    }

    private static void FillCombo(ComboBox combo, string sql)
    {
        var table = Db.GetDataTable(sql);
        combo.DataSource = table;
        combo.DisplayMember = "Name";
        combo.ValueMember = "Id";
        if (table.Rows.Count > 0) combo.SelectedIndex = 0;
    }

    private void LoadProduct(string article)
    {
        var sql = @"
SELECT p.Article, p.Name, p.UnitId, p.Price, p.SupplierId, p.ManufacturerId, p.CategoryId,
       p.DiscountPercent, p.Description, p.Photo, ISNULL(st.Quantity,0) AS Quantity
FROM dbo.Products p
LEFT JOIN dbo.Stock st ON st.Article = p.Article
WHERE p.Article = @article;";

        var table = Db.GetDataTable(sql, new SqlParameter("@article", article));
        if (table.Rows.Count == 0) return;
        var row = table.Rows[0];

        articleTextBox.Text = row["Article"].ToString();
        nameTextBox.Text = row["Name"].ToString();
        unitComboBox.SelectedValue = (int)row["UnitId"];
        supplierComboBox.SelectedValue = (int)row["SupplierId"];
        manufacturerComboBox.SelectedValue = (int)row["ManufacturerId"];
        categoryComboBox.SelectedValue = (int)row["CategoryId"];
        priceNumeric.Value = Convert.ToDecimal(row["Price"]);
        discountNumeric.Value = Convert.ToDecimal(row["DiscountPercent"]);
        stockNumeric.Value = Convert.ToDecimal(row["Quantity"]);
        descriptionTextBox.Text = row["Description"].ToString();
        photoTextBox.Text = row["Photo"].ToString();
    }

    private void OnSave(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(articleTextBox.Text) || string.IsNullOrWhiteSpace(nameTextBox.Text))
        {
            MessageBox.Show("Заполните артикул и наименование.", "Проверка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var article = articleTextBox.Text.Trim();
        var name = nameTextBox.Text.Trim();

        if (!_isEdit)
        {
            var exists = Db.Scalar<int>("SELECT COUNT(*) FROM dbo.Products WHERE Article = @article", new SqlParameter("@article", article)) > 0;
            if (exists)
            {
                MessageBox.Show("Товар с таким артикулом уже существует.", "Проверка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        var unitId = (int)unitComboBox.SelectedValue;
        var supplierId = (int)supplierComboBox.SelectedValue;
        var manufacturerId = (int)manufacturerComboBox.SelectedValue;
        var categoryId = (int)categoryComboBox.SelectedValue;
        var price = priceNumeric.Value;
        var discount = (int)discountNumeric.Value;
        var stock = (int)stockNumeric.Value;
        var description = descriptionTextBox.Text.Trim();
        var photo = photoTextBox.Text.Trim();

        if (_isEdit)
        {
            var sql = @"
UPDATE dbo.Products
SET Name = @name,
    UnitId = @unitId,
    Price = @price,
    SupplierId = @supplierId,
    ManufacturerId = @manufacturerId,
    CategoryId = @categoryId,
    DiscountPercent = @discount,
    Description = @description,
    Photo = @photo
WHERE Article = @article;";
            Db.Execute(sql,
                new SqlParameter("@name", name),
                new SqlParameter("@unitId", unitId),
                new SqlParameter("@price", price),
                new SqlParameter("@supplierId", supplierId),
                new SqlParameter("@manufacturerId", manufacturerId),
                new SqlParameter("@categoryId", categoryId),
                new SqlParameter("@discount", discount),
                new SqlParameter("@description", description),
                new SqlParameter("@photo", photo),
                new SqlParameter("@article", article));

            Db.Execute("UPDATE dbo.Stock SET Quantity = @qty WHERE Article = @article",
                new SqlParameter("@qty", stock),
                new SqlParameter("@article", article));
        }
        else
        {
            var sql = @"
INSERT INTO dbo.Products (Article, Name, UnitId, Price, SupplierId, ManufacturerId, CategoryId, DiscountPercent, Description, Photo)
VALUES (@article, @name, @unitId, @price, @supplierId, @manufacturerId, @categoryId, @discount, @description, @photo);";
            Db.Execute(sql,
                new SqlParameter("@article", article),
                new SqlParameter("@name", name),
                new SqlParameter("@unitId", unitId),
                new SqlParameter("@price", price),
                new SqlParameter("@supplierId", supplierId),
                new SqlParameter("@manufacturerId", manufacturerId),
                new SqlParameter("@categoryId", categoryId),
                new SqlParameter("@discount", discount),
                new SqlParameter("@description", description),
                new SqlParameter("@photo", photo));

            Db.Execute("INSERT INTO dbo.Stock (WarehouseId, Article, Quantity) VALUES ((SELECT TOP 1 WarehouseId FROM dbo.Warehouses), @article, @qty)",
                new SqlParameter("@article", article),
                new SqlParameter("@qty", stock));
        }

        DialogResult = DialogResult.OK;
    }
}


