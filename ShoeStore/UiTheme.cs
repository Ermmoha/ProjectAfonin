using System.Drawing;

namespace ShoeStore;

public static class UiTheme
{
    public static readonly Color MainBg = Color.White;
    public static readonly Color SecondaryBg = ColorTranslator.FromHtml("#7FFF00");
    public static readonly Color Accent = ColorTranslator.FromHtml("#00FA9A");
    public static readonly Color HighDiscount = ColorTranslator.FromHtml("#2E8B57");

    public static readonly Font BaseFont = new("Times New Roman", 12F, FontStyle.Regular);
    public static readonly Font TitleFont = new("Times New Roman", 18F, FontStyle.Bold);

    public static void Apply(Form form)
    {
        form.Font = BaseFont;
        form.BackColor = MainBg;
    }

    public static void StyleHeader(Panel panel)
    {
        panel.BackColor = SecondaryBg;
    }

    public static void StyleAccent(Button button)
    {
        button.BackColor = Accent;
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
    }

    public static string AssetsPath => Path.Combine(AppContext.BaseDirectory, "Assets");

    public static Image? LoadAsset(string fileName)
    {
        try
        {
            var path = Path.Combine(AssetsPath, fileName);
            return File.Exists(path) ? Image.FromFile(path) : null;
        }
        catch
        {
            return null;
        }
    }
}
