using System.Data;
using System.Drawing;
using Microsoft.Data.SqlClient;

namespace ShoeStore;

public class ProductsForm : Form
{
    private readonly UserSession _session;
    private readonly LoginForm _loginForm;

    private Panel headerPanel = null!;
    private FlowLayoutPanel headerActionsPanel = null!;
    private FlowLayoutPanel headerUserActionsPanel = null!;
    private Label titleLabel = null!;
    private Label userLabel = null!;
    private Button logoutButton = null!;
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

    public ProductsForm(UserSession session, LoginForm loginForm)
    {
        _session = session;
        _loginForm = loginForm;
        UiTheme.Apply(this);
        InitializeComponent();
        UiTheme.StyleHeader(headerPanel);
        UiTheme.StyleAccent(applyFilterButton);
        UiTheme.StyleAccent(clearFilterButton);
        UiTheme.StyleAccent(refreshButton);
        UiTheme.StyleAccent(ordersButton);
        UiTheme.StyleAccent(logoutButton);
        UiTheme.StyleAccent(addButton);
        UiTheme.StyleAccent(editButton);
        UiTheme.StyleAccent(deleteButton);

        ApplyRoleVisibility();
        LoadFilters();
        LoadProducts();
    }

    private void InitializeComponent()
    {
        Text = "ShoeStore";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(1200, 720);

        headerPanel = new Panel { Dock = DockStyle.Top, Height = 120, Padding = new Padding(16, 8, 16, 8) };
        headerPanel.SizeChanged += (_, _) => PositionHeaderActions();

        titleLabel = new Label
        {
            Text = "Каталог товаров",
            AutoSize = true,
            Font = UiTheme.TitleFont,
            Margin = new Padding(4, 6, 4, 0)
        };

        userLabel = new Label
        {
            AutoSize = true,
            Margin = new Padding(4, 0, 4, 0)
        };

        var headerLeftPanel = new TableLayoutPanel
        {
            ColumnCount = 1,
            RowCount = 3,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Location = new Point(headerPanel.Padding.Left, headerPanel.Padding.Top)
        };
        headerLeftPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        headerLeftPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        headerLeftPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        headerLeftPanel.Controls.Add(titleLabel, 0, 0);
        headerLeftPanel.Controls.Add(userLabel, 0, 1);

        ordersButton = new Button
        {
            Text = "Заказы",
            Size = new Size(110, 30)
        };
        ordersButton.Click += OnOrdersClick;

        refreshButton = new Button
        {
            Text = "Обновить",
            Size = new Size(110, 30)
        };
        refreshButton.Click += (_, _) => LoadProducts();

        headerUserActionsPanel = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = new Padding(4, 6, 4, 0)
        };
        headerUserActionsPanel.Controls.Add(ordersButton);
        headerUserActionsPanel.Controls.Add(refreshButton);
        headerLeftPanel.Controls.Add(headerUserActionsPanel, 0, 2);

        headerActionsPanel = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = new Padding(0)
        };

        logoutButton = new Button
        {
            Text = "Выйти",
            Size = new Size(110, 30),
            Margin = new Padding(0)
        };
        logoutButton.Click += OnLogoutClick;
        headerActionsPanel.Controls.Add(logoutButton);

        headerPanel.Controls.Add(headerLeftPanel);
        headerPanel.Controls.Add(headerActionsPanel);

        filterPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 160,
            Padding = new Padding(16, 8, 16, 8)
        };

        var filterLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };
        filterLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        filterLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var row1 = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = new Padding(0, 0, 0, 10)
        };

        var row2 = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = new Padding(0)
        };

        searchTextBox = new TextBox();
        row1.Controls.Add(MakeFilterGroup("Поиск", searchTextBox, 230));

        categoryComboBox = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        row1.Controls.Add(MakeFilterGroup("Категория", categoryComboBox, 210));

        manufacturerComboBox = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        row1.Controls.Add(MakeFilterGroup("Производитель", manufacturerComboBox, 240));

        supplierComboBox = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        row2.Controls.Add(MakeFilterGroup("Поставщик", supplierComboBox, 230));

        priceFrom = new NumericUpDown { Maximum = 1000000, DecimalPlaces = 0 };
        row2.Controls.Add(MakeFilterGroup("Цена от", priceFrom, 110));

        priceTo = new NumericUpDown { Maximum = 1000000, DecimalPlaces = 0 };
        row2.Controls.Add(MakeFilterGroup("до", priceTo, 110));

        applyFilterButton = new Button { Text = "Применить", Size = new Size(120, 32) };
        clearFilterButton = new Button { Text = "Сбросить", Size = new Size(120, 32) };
        var buttonsPanel = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = new Padding(0)
        };
        buttonsPanel.Controls.Add(applyFilterButton);
        buttonsPanel.Controls.Add(clearFilterButton);
        row2.Controls.Add(MakeFilterGroup(" ", buttonsPanel, 260, rightMargin: 0));
        applyFilterButton.Click += (_, _) => LoadProducts();
        clearFilterButton.Click += (_, _) => ClearFilters();

        filterLayout.Controls.Add(row1, 0, 0);
        filterLayout.Controls.Add(row2, 0, 1);
        filterPanel.Controls.Add(filterLayout);

        actionPanel = new Panel { Dock = DockStyle.Top, Height = 44 };
        addButton = new Button { Text = "Добавить", Location = new Point(20, 4), Size = new Size(110, 30) };
        editButton = new Button { Text = "Редактировать", Location = new Point(140, 4), Size = new Size(140, 30) };
        deleteButton = new Button { Text = "Удалить", Location = new Point(290, 4), Size = new Size(110, 30) };
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
        productPicture.Image = UiTheme.LoadAsset(UiTheme.PlaceholderImage);
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
        logoutButton.Text = _session.IsGuest ? "Назад" : "Выйти";
        var canFilter = _session.IsAdmin || _session.IsManager;
        filterPanel.Visible = canFilter;

        ordersButton.Visible = _session.IsAdmin || _session.IsManager;
        actionPanel.Visible = _session.IsAdmin;
        PositionHeaderActions();
    }

    private void PositionHeaderActions()
    {
        if (headerActionsPanel == null || headerPanel == null) return;
        var padding = headerPanel.Padding;
        var innerWidth = headerPanel.ClientSize.Width - padding.Left - padding.Right;
        var innerHeight = headerPanel.ClientSize.Height - padding.Top - padding.Bottom;
        var x = padding.Left + Math.Max(0, innerWidth - headerActionsPanel.Width);
        var y = padding.Top + Math.Max(0, (innerHeight - headerActionsPanel.Height) / 2);
        headerActionsPanel.Location = new Point(x, y);
    }

    private static Control MakeFilterGroup(string text, Control control, int width, int rightMargin = 28)
    {
        var group = new TableLayoutPanel
        {
            RowCount = 2,
            ColumnCount = 1,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = new Padding(0, 0, rightMargin, 0),
            MinimumSize = new Size(width, 0)
        };
        group.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        group.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var label = new Label { Text = text, AutoSize = true, Margin = new Padding(0, 0, 0, 4) };
        group.Controls.Add(label, 0, 0);

        control.Margin = new Padding(0);
        control.Width = width;
        if (control is FlowLayoutPanel flow)
        {
            flow.MinimumSize = new Size(width, 0);
        }
        group.Controls.Add(control, 0, 1);

        return group;
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
        var img = !string.IsNullOrWhiteSpace(photo) ? UiTheme.LoadAsset(photo) : null;
        if (img == null)
        {
            img = UiTheme.LoadAsset(UiTheme.PlaceholderImage);
        }
        productPicture.Image?.Dispose();
        productPicture.Image = img;

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

    private void OnLogoutClick(object? sender, EventArgs e)
    {
        _loginForm.ShowForLogout();
        Close();
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



