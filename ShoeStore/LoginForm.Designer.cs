using System.ComponentModel;
using System.Drawing;

namespace ShoeStore;

partial class LoginForm
{
    private IContainer? components = null;
    private Panel headerPanel = null!;
    private Label titleLabel = null!;
    private PictureBox logoPicture = null!;
    private Label loginLabel = null!;
    private TextBox loginTextBox = null!;
    private Label passwordLabel = null!;
    private TextBox passwordTextBox = null!;
    private Button loginButton = null!;
    private Button guestButton = null!;
    private Label errorLabel = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new Container();
        headerPanel = new Panel();
        titleLabel = new Label();
        logoPicture = new PictureBox();
        loginLabel = new Label();
        loginTextBox = new TextBox();
        passwordLabel = new Label();
        passwordTextBox = new TextBox();
        loginButton = new Button();
        guestButton = new Button();
        errorLabel = new Label();

        SuspendLayout();

        headerPanel.Dock = DockStyle.Top;
        headerPanel.Height = 80;
        headerPanel.Controls.Add(titleLabel);
        headerPanel.Controls.Add(logoPicture);

        titleLabel.AutoSize = true;
        titleLabel.Font = UiTheme.TitleFont;
        titleLabel.Location = new Point(20, 20);
        titleLabel.Text = "Вход в систему";

        logoPicture.Size = new Size(60, 60);
        logoPicture.Location = new Point(800, 10);
        logoPicture.SizeMode = PictureBoxSizeMode.Zoom;
        logoPicture.Anchor = AnchorStyles.Top | AnchorStyles.Right;

        loginLabel.AutoSize = true;
        loginLabel.Location = new Point(250, 150);
        loginLabel.Text = "Логин";

        loginTextBox.Location = new Point(250, 175);
        loginTextBox.Width = 400;

        passwordLabel.AutoSize = true;
        passwordLabel.Location = new Point(250, 225);
        passwordLabel.Text = "Пароль";

        passwordTextBox.Location = new Point(250, 250);
        passwordTextBox.Width = 400;

        loginButton.Location = new Point(250, 310);
        loginButton.Size = new Size(400, 36);
        loginButton.Text = "Войти";
        loginButton.Click += OnLoginClick;

        guestButton.Location = new Point(250, 360);
        guestButton.Size = new Size(400, 36);
        guestButton.Text = "Войти как гость";
        guestButton.Click += OnGuestClick;

        errorLabel.AutoSize = true;
        errorLabel.ForeColor = Color.Firebrick;
        errorLabel.Location = new Point(250, 410);
        errorLabel.Text = string.Empty;

        ClientSize = new Size(900, 520);
        Controls.Add(headerPanel);
        Controls.Add(loginLabel);
        Controls.Add(loginTextBox);
        Controls.Add(passwordLabel);
        Controls.Add(passwordTextBox);
        Controls.Add(loginButton);
        Controls.Add(guestButton);
        Controls.Add(errorLabel);
        Text = "ООО «Обувь» — вход";
        StartPosition = FormStartPosition.CenterScreen;

        ResumeLayout(false);
        PerformLayout();
    }
}
