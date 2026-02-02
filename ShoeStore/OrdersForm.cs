using System.Data;
using System.Drawing;
using Microsoft.Data.SqlClient;

namespace ShoeStore;

public class OrdersForm : Form
{
    private readonly UserSession _session;
    private DataGridView ordersGrid = null!;
    private Panel headerPanel = null!;
    private Label titleLabel = null!;
    private Panel actionPanel = null!;
    private Button addButton = null!;
    private Button editButton = null!;
    private Button deleteButton = null!;
    private Button refreshButton = null!;

    public OrdersForm(UserSession session)
    {
        _session = session;
        UiTheme.Apply(this);
        InitializeComponent();
        UiTheme.StyleHeader(headerPanel);
        UiTheme.StyleAccent(addButton);
        UiTheme.StyleAccent(editButton);
        UiTheme.StyleAccent(deleteButton);
        UiTheme.StyleAccent(refreshButton);
        ApplyRoleVisibility();
        LoadOrders();
    }

    private void InitializeComponent()
    {
        Text = "ShoeStore";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(1000, 640);

        headerPanel = new Panel { Dock = DockStyle.Top, Height = 70 };
        titleLabel = new Label
        {
            Text = "Заказы",
            AutoSize = true,
            Font = UiTheme.TitleFont,
            Location = new Point(20, 18)
        };
        refreshButton = new Button { Text = "Обновить", Size = new Size(110, 30), Location = new Point(860, 20), Anchor = AnchorStyles.Top | AnchorStyles.Right };
        refreshButton.Click += (_, _) => LoadOrders();

        headerPanel.Controls.Add(titleLabel);
        headerPanel.Controls.Add(refreshButton);

        actionPanel = new Panel { Dock = DockStyle.Top, Height = 50 };
        addButton = new Button { Text = "Добавить", Location = new Point(20, 10), Size = new Size(110, 30) };
        editButton = new Button { Text = "Редактировать", Location = new Point(140, 10), Size = new Size(140, 30) };
        deleteButton = new Button { Text = "Удалить", Location = new Point(290, 10), Size = new Size(110, 30) };
        addButton.Click += OnAdd;
        editButton.Click += OnEdit;
        deleteButton.Click += OnDelete;

        actionPanel.Controls.Add(addButton);
        actionPanel.Controls.Add(editButton);
        actionPanel.Controls.Add(deleteButton);

        ordersGrid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false
        };

        Controls.Add(ordersGrid);
        Controls.Add(actionPanel);
        Controls.Add(headerPanel);
    }

    private void ApplyRoleVisibility()
    {
        actionPanel.Visible = _session.IsAdmin;
    }

    private void LoadOrders()
    {
        try
        {
            var sql = @"
SELECT o.OrderNumber AS Номер,
       o.OrderDate AS ДатаЗаказа,
       o.DeliveryDate AS ДатаДоставки,
       (pp.PostalCode + ', ' + pp.City + ', ' + pp.Street + ', ' + pp.House) AS ПунктВыдачи,
       u.FullName AS Клиент,
       o.PickupCode AS Код,
       s.Name AS Статус,
       (SELECT SUM(oi.Quantity) FROM dbo.OrderItems oi WHERE oi.OrderNumber = o.OrderNumber) AS Позиций
FROM dbo.Orders o
INNER JOIN dbo.PickupPoints pp ON pp.PickupPointId = o.PickupPointId
INNER JOIN dbo.Users u ON u.UserId = o.CustomerId
INNER JOIN dbo.OrderStatuses s ON s.StatusId = o.StatusId
ORDER BY o.OrderNumber DESC;";

            ordersGrid.DataSource = Db.GetDataTable(sql);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки заказов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void OnAdd(object? sender, EventArgs e)
    {
        using var form = new OrderEditForm();
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            LoadOrders();
        }
    }

    private void OnEdit(object? sender, EventArgs e)
    {
        if (ordersGrid.CurrentRow == null) return;
        if (!int.TryParse(ordersGrid.CurrentRow.Cells["Номер"].Value?.ToString(), out var orderNumber)) return;
        using var form = new OrderEditForm(orderNumber);
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            LoadOrders();
        }
    }

    private void OnDelete(object? sender, EventArgs e)
    {
        if (ordersGrid.CurrentRow == null) return;
        if (!int.TryParse(ordersGrid.CurrentRow.Cells["Номер"].Value?.ToString(), out var orderNumber)) return;

        if (MessageBox.Show("Удалить выбранный заказ?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        {
            return;
        }

        Db.Execute("DELETE FROM dbo.OrderItems WHERE OrderNumber = @orderNumber", new SqlParameter("@orderNumber", orderNumber));
        Db.Execute("DELETE FROM dbo.Orders WHERE OrderNumber = @orderNumber", new SqlParameter("@orderNumber", orderNumber));
        LoadOrders();
    }
}

