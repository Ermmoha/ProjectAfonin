using System.Data;
using System.Drawing;
using Microsoft.Data.SqlClient;

namespace ShoeStore;

public class ProductsForm : Form
{
    private readonly UserSession _session;

    private Panel headerPanel = null!;
    private Label titleLabel = null!;
    private Label userLabel = null!;
    private Button ordersButton = null!;
    private Button refreshButton = null!;

    private Panel filterPanel = null!;
    private TextBox searchTextBox = null!;
    private ComboBox categoryComboBox = null!;
    private ComboBox manufacturerComboBox = null!;
    private ComboBox supplierComboBox = null!;
    private NumericUpDown priceFrom = null!;
    private NumericUpDown priceTo = null!;
    private Button applyFilterButton = null!;
    private Button clearFilterButton = null!;

    private Panel actionPanel = null!;
    private Button addButton = null!;
    private Button editButton = null!;
    private Button deleteButton = null!;

    private DataGridView productsGrid = null!;
    private PictureBox productPicture = null!;
    private Label productInfoLabel = null!;

    public ProductsForm(UserSession session)
    {
        _session = session;
        InitializeComponent();
        UiTheme.Apply(this);
        UiTheme.StyleHeader(headerPanel);
        UiTheme.StyleAccent(applyFilterButton);
        UiTheme.StyleAccent(clearFilterButton);
        UiTheme.StyleAccent(refreshButton);
        UiTheme.StyleAccent(ordersButton);
        UiTheme.StyleAccent(addButton);
        UiTheme.StyleAccent(editButton);
        UiTheme.StyleAccent(deleteButton);

        ApplyRoleVisibility();
        LoadFilters();
        LoadProducts();
    }

    private void InitializeComponent()
    {
        Text = "ООО «Обувь» — товары";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(1200, 720);

        headerPanel = new Panel { Dock = DockStyle.Top, Height = 70 };
        titleLabel = new Label
        {
            Text = "Каталог товаров",
            AutoSize = true,
            Font = UiTheme.TitleFont,
            Location = new Point(20, 18)
        };
        userLabel = new Label
        {
            AutoSize = true,
            Location = new Point(20, 45)
        };
        ordersButton = new Button
        {
            Text = "Заказы",
            Size = new Size(110, 30),
            Location = new Point(980, 20),
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };
        ordersButton.Click += OnOrdersClick;

        refreshButton = new Button
        {
            Text = "Обновить",
            Size = new Size(110, 30),
            Location = new Point(1100, 20),
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };
        refreshButton.Click += (_, _) => LoadProducts();

        headerPanel.Controls.Add(titleLabel);
        headerPanel.Controls.Add(userLabel);
        headerPanel.Controls.Add(ordersButton);
        headerPanel.Controls.Add(refreshButton);

        filterPanel = new Panel { Dock = DockStyle.Top, Height = 90 };

        var searchLabel = new Label { Text = "Поиск", AutoSize = true, Location = new Point(20, 12) };
        searchTextBox = new TextBox { Location = new Point(80, 8), Width = 200 };

        var categoryLabel = new Label { Text = "Категория", AutoSize = true, Location = new Point(300, 12) };
        categoryComboBox = new ComboBox { Location = new Point(380, 8), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };

        var manufacturerLabel = new Label { Text = "Производитель", AutoSize = true, Location = new Point(580, 12) };
        manufacturerComboBox = new ComboBox { Location = new Point(700, 8), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };

        var supplierLabel = new Label { Text = "Поставщик", AutoSize = true, Location = new Point(20, 50) };
        supplierComboBox = new ComboBox { Location = new Point(80, 46), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };

        var priceFromLabel = new Label { Text = "Цена от", AutoSize = true, Location = new Point(300, 50) };
        priceFrom = new NumericUpDown { Location = new Point(370, 46), Width = 90, Maximum = 1000000, DecimalPlaces = 0 };
        var priceToLabel = new Label { Text = "до", AutoSize = true, Location = new Point(470, 50) };
        priceTo = new NumericUpDown { Location = new Point(500, 46), Width = 90, Maximum = 1000000, DecimalPlaces = 0 };

        applyFilterButton = new Button { Text = "Применить", Location = new Point(700, 42), Size = new Size(110, 32) };
        clearFilterButton = new Button { Text = "Сбросить", Location = new Point(820, 42), Size = new Size(110, 32) };
        applyFilterButton.Click += (_, _) => LoadProducts();
        clearFilterButton.Click += (_, _) => ClearFilters();

        filterPanel.Controls.Add(searchLabel);
        filterPanel.Controls.Add(searchTextBox);
        filterPanel.Controls.Add(categoryLabel);
        filterPanel.Controls.Add(categoryComboBox);
        filterPanel.Controls.Add(manufacturerLabel);
        filterPanel.Controls.Add(manufacturerComboBox);
        filterPanel.Controls.Add(supplierLabel);
        filterPanel.Controls.Add(supplierComboBox);
        filterPanel.Controls.Add(priceFromLabel);
        filterPanel.Controls.Add(priceFrom);
        filterPanel.Controls.Add(priceToLabel);
        filterPanel.Controls.Add(priceTo);
        filterPanel.Controls.Add(applyFilterButton);
        filterPanel.Controls.Add(clearFilterButton);

        actionPanel = new Panel { Dock = DockStyle.Top, Height = 50 };
        addButton = new Button { Text = "Добавить", Location = new Point(20, 10), Size = new Size(110, 30) };
        editButton = new Button { Text = "Редактировать", Location = new Point(140, 10), Size = new Size(140, 30) };
        deleteButton = new Button { Text = "Удалить", Location = new Point(290, 10), Size = new Size(110, 30) };
        addButton.Click += OnAddProduct;
        editButton.Click += OnEditProduct;
        deleteButton.Click += OnDeleteProduct;

        actionPanel.Controls.Add(addButton);
        actionPanel.Controls.Add(editButton);
        actionPanel.Controls.Add(deleteButton);

        productsGrid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false
        };
        productsGrid.RowPrePaint += ProductsGrid_RowPrePaint;
        productsGrid.SelectionChanged += ProductsGrid_SelectionChanged;

        var split = new SplitContainer
        {
            Dock = DockStyle.Fill,
            SplitterDistance = 820
        };
        split.Panel1.Controls.Add(productsGrid);

        var previewPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
        productPicture = new PictureBox { Dock = DockStyle.Top, Height = 260, SizeMode = PictureBoxSizeMode.Zoom };
        productInfoLabel = new Label { Dock = DockStyle.Fill, AutoSize = false };
        previewPanel.Controls.Add(productInfoLabel);
        previewPanel.Controls.Add(productPicture);
        split.Panel2.Controls.Add(previewPanel);

        Controls.Add(split);
        Controls.Add(actionPanel);
        Controls.Add(filterPanel);
        Controls.Add(headerPanel);
    }

    private void ApplyRoleVisibility()
    {
        userLabel.Text = $"Пользователь: {_session.FullName} ({_session.RoleName})";
        var canFilter = _session.IsAdmin || _session.IsManager;
        filterPanel.Visible = canFilter;

        ordersButton.Visible = _session.IsAdmin || _session.IsManager;
        actionPanel.Visible = _session.IsAdmin;
    }

    private void ClearFilters()
    {
        searchTextBox.Text = string.Empty;
        if (categoryComboBox.Items.Count > 0) categoryComboBox.SelectedIndex = 0;
        if (manufacturerComboBox.Items.Count > 0) manufacturerComboBox.SelectedIndex = 0;
        if (supplierComboBox.Items.Count > 0) supplierComboBox.SelectedIndex = 0;
        priceFrom.Value = 0;
        priceTo.Value = 0;
        LoadProducts();
    }

    private void LoadFilters()
    {
        try
        {
            FillCombo(categoryComboBox, "SELECT CategoryId AS Id, Name FROM dbo.Categories ORDER BY Name");
            FillCombo(manufacturerComboBox, "SELECT ManufacturerId AS Id, Name FROM dbo.Manufacturers ORDER BY Name");
            FillCombo(supplierComboBox, "SELECT SupplierId AS Id, Name FROM dbo.Suppliers ORDER BY Name");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки фильтров: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static void FillCombo(ComboBox combo, string sql)
    {
        var table = Db.GetDataTable(sql);
        var row = table.NewRow();
        row["Id"] = 0;
        row["Name"] = "Все";
        table.Rows.InsertAt(row, 0);
        combo.DataSource = table;
        combo.DisplayMember = "Name";
        combo.ValueMember = "Id";
        combo.SelectedIndex = 0;
    }

    private void LoadProducts()
    {
        try
        {
            var sql = new List<string>
            {
                "SELECT p.Article AS Артикул,",
                "p.Name AS Наименование,",
                "u.Name AS ЕдИзм,",
                "p.Price AS Цена,",
                "s.Name AS Поставщик,",
                "m.Name AS Производитель,",
                "c.Name AS Категория,",
                "p.DiscountPercent AS Скидка,",
                "ISNULL(st.Quantity, 0) AS Остаток,",
                "p.Description AS Описание,",
                "p.Photo AS Фото",
                "FROM dbo.Products p",
                "INNER JOIN dbo.Units u ON u.UnitId = p.UnitId",
                "INNER JOIN dbo.Suppliers s ON s.SupplierId = p.SupplierId",
                "INNER JOIN dbo.Manufacturers m ON m.ManufacturerId = p.ManufacturerId",
                "INNER JOIN dbo.Categories c ON c.CategoryId = p.CategoryId",
                "LEFT JOIN dbo.Stock st ON st.Article = p.Article",
                "WHERE 1=1"
            };

            var parameters = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(searchTextBox.Text))
            {
                sql.Add("AND (p.Name LIKE @search OR p.Article LIKE @search)");
                parameters.Add(new SqlParameter("@search", $"%{searchTextBox.Text.Trim()}%"));
            }

            if (categoryComboBox.SelectedValue is int categoryId && categoryId > 0)
            {
                sql.Add("AND p.CategoryId = @categoryId");
                parameters.Add(new SqlParameter("@categoryId", categoryId));
            }
            if (manufacturerComboBox.SelectedValue is int manufacturerId && manufacturerId > 0)
            {
                sql.Add("AND p.ManufacturerId = @manufacturerId");
                parameters.Add(new SqlParameter("@manufacturerId", manufacturerId));
            }
            if (supplierComboBox.SelectedValue is int supplierId && supplierId > 0)
            {
                sql.Add("AND p.SupplierId = @supplierId");
                parameters.Add(new SqlParameter("@supplierId", supplierId));
            }
            if (priceFrom.Value > 0)
            {
                sql.Add("AND p.Price >= @priceFrom");
                parameters.Add(new SqlParameter("@priceFrom", priceFrom.Value));
            }
            if (priceTo.Value > 0)
            {
                sql.Add("AND p.Price <= @priceTo");
                parameters.Add(new SqlParameter("@priceTo", priceTo.Value));
            }

            sql.Add("ORDER BY p.Name");

            var table = Db.GetDataTable(string.Join("\n", sql), parameters.ToArray());
            productsGrid.DataSource = table;

            if (_session.IsGuest || _session.IsClient)
            {
                foreach (DataGridViewColumn column in productsGrid.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки товаров: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ProductsGrid_RowPrePaint(object? sender, DataGridViewRowPrePaintEventArgs e)
    {
        if (productsGrid.Rows.Count <= e.RowIndex) return;
        var row = productsGrid.Rows[e.RowIndex];
        if (row.Cells["Скидка"].Value is DBNull) return;
        if (int.TryParse(row.Cells["Скидка"].Value?.ToString(), out var discount) && discount > 15)
        {
            row.DefaultCellStyle.BackColor = UiTheme.HighDiscount;
            row.DefaultCellStyle.ForeColor = Color.White;
        }
        else
        {
            row.DefaultCellStyle.BackColor = UiTheme.MainBg;
            row.DefaultCellStyle.ForeColor = Color.Black;
        }
    }

    private void ProductsGrid_SelectionChanged(object? sender, EventArgs e)
    {
        if (productsGrid.CurrentRow == null) return;
        var photo = productsGrid.CurrentRow.Cells["Фото"].Value?.ToString();
        productInfoLabel.Text = string.Empty;
        if (!string.IsNullOrWhiteSpace(photo))
        {
            var img = UiTheme.LoadAsset(photo);
            productPicture.Image?.Dispose();
            productPicture.Image = img;
        }
        else
        {
            productPicture.Image?.Dispose();
            productPicture.Image = null;
        }

        var name = productsGrid.CurrentRow.Cells["Наименование"].Value?.ToString();
        var price = productsGrid.CurrentRow.Cells["Цена"].Value?.ToString();
        var stock = productsGrid.CurrentRow.Cells["Остаток"].Value?.ToString();
        var desc = productsGrid.CurrentRow.Cells["Описание"].Value?.ToString();
        productInfoLabel.Text = $"{name}\nЦена: {price}\nОстаток: {stock}\n\n{desc}";
    }

    private void OnOrdersClick(object? sender, EventArgs e)
    {
        var form = new OrdersForm(_session);
        form.ShowDialog(this);
    }

    private void OnAddProduct(object? sender, EventArgs e)
    {
        using var form = new ProductEditForm();
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            LoadProducts();
        }
    }

    private void OnEditProduct(object? sender, EventArgs e)
    {
        if (productsGrid.CurrentRow == null) return;
        var article = productsGrid.CurrentRow.Cells["Артикул"].Value?.ToString();
        if (string.IsNullOrWhiteSpace(article)) return;
        using var form = new ProductEditForm(article);
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            LoadProducts();
        }
    }

    private void OnDeleteProduct(object? sender, EventArgs e)
    {
        if (productsGrid.CurrentRow == null) return;
        var article = productsGrid.CurrentRow.Cells["Артикул"].Value?.ToString();
        if (string.IsNullOrWhiteSpace(article)) return;

        var hasOrders = Db.Scalar<int>("SELECT COUNT(*) FROM dbo.OrderItems WHERE Article = @article",
            new SqlParameter("@article", article)) > 0;
        if (hasOrders)
        {
            MessageBox.Show("Нельзя удалить товар, который используется в заказах.", "Ограничение", MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        if (MessageBox.Show("Удалить выбранный товар?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        {
            return;
        }

        Db.Execute("DELETE FROM dbo.Stock WHERE Article = @article", new SqlParameter("@article", article));
        Db.Execute("DELETE FROM dbo.Products WHERE Article = @article", new SqlParameter("@article", article));
        LoadProducts();
    }
}



