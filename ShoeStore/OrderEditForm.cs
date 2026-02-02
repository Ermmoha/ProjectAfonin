using System.Data;
using System.Drawing;
using Microsoft.Data.SqlClient;

namespace ShoeStore;

public class OrderEditForm : Form
{
    private readonly int? _orderNumber;
    private readonly bool _isEdit;

    private TextBox orderNumberTextBox = null!;
    private DateTimePicker orderDatePicker = null!;
    private DateTimePicker deliveryDatePicker = null!;
    private ComboBox pickupPointComboBox = null!;
    private ComboBox customerComboBox = null!;
    private ComboBox statusComboBox = null!;
    private TextBox pickupCodeTextBox = null!;
    private DataGridView itemsGrid = null!;
    private Button saveButton = null!;
    private Button cancelButton = null!;

    public OrderEditForm(int? orderNumber = null)
    {
        _orderNumber = orderNumber;
        _isEdit = orderNumber.HasValue;
        UiTheme.Apply(this);
        InitializeComponent();
        UiTheme.StyleAccent(saveButton);
        UiTheme.StyleAccent(cancelButton);
        LoadCombos();
        LoadProducts();

        if (_isEdit)
        {
            Text = "ShoeStore";
            orderNumberTextBox.ReadOnly = true;
            LoadOrder(orderNumber!.Value);
        }
        else
        {
            Text = "ShoeStore";
        }
    }

    private void InitializeComponent()
    {
        ClientSize = new Size(900, 650);
        StartPosition = FormStartPosition.CenterParent;

        var left = 20;
        var labelWidth = 150;
        var inputWidth = 220;
        var top = 20;
        var rowHeight = 28;
        var gap = 8;

        Controls.Add(MakeLabel("Номер", left, top));
        orderNumberTextBox = MakeTextBox(left + labelWidth, top, inputWidth);
        Controls.Add(orderNumberTextBox);
        top += rowHeight + gap;

        Controls.Add(MakeLabel("Дата заказа", left, top));
        orderDatePicker = new DateTimePicker { Location = new Point(left + labelWidth, top), Width = inputWidth, Format = DateTimePickerFormat.Short };
        Controls.Add(orderDatePicker);
        top += rowHeight + gap;

        Controls.Add(MakeLabel("Дата доставки", left, top));
        deliveryDatePicker = new DateTimePicker { Location = new Point(left + labelWidth, top), Width = inputWidth, Format = DateTimePickerFormat.Short };
        Controls.Add(deliveryDatePicker);
        top += rowHeight + gap;

        Controls.Add(MakeLabel("Пункт выдачи", left, top));
        pickupPointComboBox = MakeComboBox(left + labelWidth, top, inputWidth + 280);
        Controls.Add(pickupPointComboBox);
        top += rowHeight + gap;

        Controls.Add(MakeLabel("Клиент", left, top));
        customerComboBox = MakeComboBox(left + labelWidth, top, inputWidth + 280);
        Controls.Add(customerComboBox);
        top += rowHeight + gap;

        Controls.Add(MakeLabel("Статус", left, top));
        statusComboBox = MakeComboBox(left + labelWidth, top, inputWidth);
        Controls.Add(statusComboBox);
        top += rowHeight + gap;

        Controls.Add(MakeLabel("Код выдачи", left, top));
        pickupCodeTextBox = MakeTextBox(left + labelWidth, top, inputWidth);
        Controls.Add(pickupCodeTextBox);
        top += rowHeight + gap + 10;

        var itemsLabel = new Label { Text = "Состав заказа", Location = new Point(left, top), AutoSize = true };
        Controls.Add(itemsLabel);
        top += rowHeight;

        var buttonsTop = ClientSize.Height - 70;
        var gridHeight = buttonsTop - top - 20;
        if (gridHeight < 200)
        {
            gridHeight = 200;
            buttonsTop = top + gridHeight + 20;
            ClientSize = new Size(ClientSize.Width, buttonsTop + 70);
        }
        itemsGrid = new DataGridView
        {
            Location = new Point(left, top),
            Size = new Size(840, gridHeight),
            AllowUserToAddRows = true,
            AllowUserToDeleteRows = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };
        var articleCol = new DataGridViewComboBoxColumn
        {
            Name = "Article",
            HeaderText = "Артикул",
            DataPropertyName = "Article"
        };
        var qtyCol = new DataGridViewTextBoxColumn
        {
            Name = "Quantity",
            HeaderText = "Кол-во",
            DataPropertyName = "Quantity"
        };
        itemsGrid.Columns.Add(articleCol);
        itemsGrid.Columns.Add(qtyCol);
        Controls.Add(itemsGrid);

        saveButton = new Button { Text = "Сохранить", Location = new Point(left + 200, buttonsTop), Size = new Size(160, 36) };
        cancelButton = new Button { Text = "Отмена", Location = new Point(left + 380, buttonsTop), Size = new Size(160, 36) };
        saveButton.Click += OnSave;
        cancelButton.Click += (_, _) => DialogResult = DialogResult.Cancel;
        Controls.Add(saveButton);
        Controls.Add(cancelButton);
    }

    private static Label MakeLabel(string text, int x, int y)
    {
        return new Label { Text = text, Location = new Point(x, y + 4), Width = 150 };
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
        var pickupTable = Db.GetDataTable(@"
SELECT PickupPointId AS Id,
       (PostalCode + ', ' + City + ', ' + Street + ', ' + House) AS Name
FROM dbo.PickupPoints
ORDER BY PickupPointId;");
        pickupPointComboBox.DataSource = pickupTable;
        pickupPointComboBox.DisplayMember = "Name";
        pickupPointComboBox.ValueMember = "Id";

        var customerTable = Db.GetDataTable(@"
SELECT u.UserId AS Id, u.FullName AS Name
FROM dbo.Users u
INNER JOIN dbo.Roles r ON r.RoleId = u.RoleId
WHERE r.Name = N'Авторизированный клиент'
ORDER BY u.FullName;");
        customerComboBox.DataSource = customerTable;
        customerComboBox.DisplayMember = "Name";
        customerComboBox.ValueMember = "Id";

        var statusTable = Db.GetDataTable("SELECT StatusId AS Id, Name FROM dbo.OrderStatuses ORDER BY Name");
        statusComboBox.DataSource = statusTable;
        statusComboBox.DisplayMember = "Name";
        statusComboBox.ValueMember = "Id";
    }

    private void LoadProducts()
    {
        var products = Db.GetDataTable("SELECT Article, (Article + ' - ' + Name) AS Display FROM dbo.Products ORDER BY Name");
        if (itemsGrid.Columns["Article"] is DataGridViewComboBoxColumn combo)
        {
            combo.DataSource = products;
            combo.DisplayMember = "Display";
            combo.ValueMember = "Article";
        }
    }

    private void LoadOrder(int orderNumber)
    {
        var orderTable = Db.GetDataTable(@"
SELECT OrderNumber, OrderDate, DeliveryDate, PickupPointId, CustomerId, PickupCode, StatusId
FROM dbo.Orders
WHERE OrderNumber = @orderNumber;", new SqlParameter("@orderNumber", orderNumber));

        if (orderTable.Rows.Count == 0) return;
        var row = orderTable.Rows[0];
        orderNumberTextBox.Text = row["OrderNumber"].ToString();
        orderDatePicker.Value = Convert.ToDateTime(row["OrderDate"]);
        deliveryDatePicker.Value = Convert.ToDateTime(row["DeliveryDate"]);
        pickupPointComboBox.SelectedValue = (int)row["PickupPointId"];
        customerComboBox.SelectedValue = (int)row["CustomerId"];
        statusComboBox.SelectedValue = (int)row["StatusId"];
        pickupCodeTextBox.Text = row["PickupCode"].ToString();

        var items = Db.GetDataTable("SELECT Article, Quantity FROM dbo.OrderItems WHERE OrderNumber = @orderNumber",
            new SqlParameter("@orderNumber", orderNumber));
        foreach (DataRow item in items.Rows)
        {
            itemsGrid.Rows.Add(item["Article"], item["Quantity"]);
        }
    }

    private void OnSave(object? sender, EventArgs e)
    {
        if (!int.TryParse(orderNumberTextBox.Text.Trim(), out var orderNumber))
        {
            MessageBox.Show("Введите корректный номер заказа.", "Проверка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var items = new List<(string Article, int Quantity)>();
        foreach (DataGridViewRow row in itemsGrid.Rows)
        {
            if (row.IsNewRow) continue;
            var article = row.Cells["Article"].Value?.ToString();
            var qtyText = row.Cells["Quantity"].Value?.ToString();
            if (string.IsNullOrWhiteSpace(article)) continue;
            if (!int.TryParse(qtyText, out var qty) || qty <= 0) continue;
            items.Add((article, qty));
        }

        if (items.Count == 0)
        {
            MessageBox.Show("Добавьте хотя бы одну позицию заказа.", "Проверка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var conn = new SqlConnection(Db.ConnectionString);
        conn.Open();
        using var tx = conn.BeginTransaction();
        try
        {
            if (!_isEdit)
            {
                var exists = new SqlCommand("SELECT COUNT(*) FROM dbo.Orders WHERE OrderNumber = @orderNumber", conn, tx);
                exists.Parameters.AddWithValue("@orderNumber", orderNumber);
                var count = (int)exists.ExecuteScalar();
                if (count > 0)
                {
                    MessageBox.Show("Заказ с таким номером уже существует.", "Проверка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tx.Rollback();
                    return;
                }

                var insertOrder = new SqlCommand(@"
INSERT INTO dbo.Orders (OrderNumber, OrderDate, DeliveryDate, PickupPointId, CustomerId, PickupCode, StatusId)
VALUES (@orderNumber, @orderDate, @deliveryDate, @pickupPointId, @customerId, @pickupCode, @statusId);", conn, tx);
                insertOrder.Parameters.AddWithValue("@orderNumber", orderNumber);
                insertOrder.Parameters.AddWithValue("@orderDate", orderDatePicker.Value.Date);
                insertOrder.Parameters.AddWithValue("@deliveryDate", deliveryDatePicker.Value.Date);
                insertOrder.Parameters.AddWithValue("@pickupPointId", (int)pickupPointComboBox.SelectedValue);
                insertOrder.Parameters.AddWithValue("@customerId", (int)customerComboBox.SelectedValue);
                insertOrder.Parameters.AddWithValue("@pickupCode", pickupCodeTextBox.Text.Trim());
                insertOrder.Parameters.AddWithValue("@statusId", (int)statusComboBox.SelectedValue);
                insertOrder.ExecuteNonQuery();
            }
            else
            {
                var updateOrder = new SqlCommand(@"
UPDATE dbo.Orders
SET OrderDate = @orderDate,
    DeliveryDate = @deliveryDate,
    PickupPointId = @pickupPointId,
    CustomerId = @customerId,
    PickupCode = @pickupCode,
    StatusId = @statusId
WHERE OrderNumber = @orderNumber;", conn, tx);
                updateOrder.Parameters.AddWithValue("@orderNumber", orderNumber);
                updateOrder.Parameters.AddWithValue("@orderDate", orderDatePicker.Value.Date);
                updateOrder.Parameters.AddWithValue("@deliveryDate", deliveryDatePicker.Value.Date);
                updateOrder.Parameters.AddWithValue("@pickupPointId", (int)pickupPointComboBox.SelectedValue);
                updateOrder.Parameters.AddWithValue("@customerId", (int)customerComboBox.SelectedValue);
                updateOrder.Parameters.AddWithValue("@pickupCode", pickupCodeTextBox.Text.Trim());
                updateOrder.Parameters.AddWithValue("@statusId", (int)statusComboBox.SelectedValue);
                updateOrder.ExecuteNonQuery();

                var clearItems = new SqlCommand("DELETE FROM dbo.OrderItems WHERE OrderNumber = @orderNumber", conn, tx);
                clearItems.Parameters.AddWithValue("@orderNumber", orderNumber);
                clearItems.ExecuteNonQuery();
            }

            foreach (var item in items)
            {
                var insertItem = new SqlCommand(@"
INSERT INTO dbo.OrderItems (OrderNumber, Article, Quantity, UnitPrice, DiscountPercent)
SELECT @orderNumber, p.Article, @qty, p.Price, p.DiscountPercent
FROM dbo.Products p
WHERE p.Article = @article;", conn, tx);
                insertItem.Parameters.AddWithValue("@orderNumber", orderNumber);
                insertItem.Parameters.AddWithValue("@article", item.Article);
                insertItem.Parameters.AddWithValue("@qty", item.Quantity);
                insertItem.ExecuteNonQuery();
            }

            tx.Commit();
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex)
        {
            tx.Rollback();
            MessageBox.Show($"Ошибка сохранения заказа: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

