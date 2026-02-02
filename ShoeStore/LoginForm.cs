namespace ShoeStore;

public partial class LoginForm : Form
{
    public LoginForm()
    {
        InitializeComponent();
        UiTheme.Apply(this);
        UiTheme.StyleHeader(headerPanel);
        UiTheme.StyleAccent(loginButton);
        UiTheme.StyleAccent(guestButton);
        passwordTextBox.UseSystemPasswordChar = true;
        AcceptButton = loginButton;

        var logo = UiTheme.LoadAsset("Logo.png");
        if (logo != null)
        {
            logoPicture.Image = logo;
        }
    }

    private void OnLoginClick(object? sender, EventArgs e)
    {
        errorLabel.Text = string.Empty;
        try
        {
            var session = AuthService.Authenticate(loginTextBox.Text, passwordTextBox.Text);
            if (session == null)
            {
                errorLabel.Text = "Неверный логин или пароль";
                return;
            }

            OpenProducts(session);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка подключения к БД: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void OnGuestClick(object? sender, EventArgs e)
    {
        OpenProducts(UserSession.Guest());
    }

    private void OpenProducts(UserSession session)
    {
        Hide();
        var form = new ProductsForm(session);
        form.FormClosed += (_, _) => Close();
        form.Show();
    }
}
