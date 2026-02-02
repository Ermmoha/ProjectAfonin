using System.Drawing;
using System.Runtime.InteropServices;

namespace ShoeStore;

public static class UiTheme
{
    public const string AppIcon = "Icon.ico";
    public const string PlaceholderImage = "picture.png";
    public static readonly Color MainBg = Color.White;
    public static readonly Color SecondaryBg = ColorTranslator.FromHtml("#7FFF00");
    public static readonly Color Accent = ColorTranslator.FromHtml("#00FA9A");
    public static readonly Color HighDiscount = ColorTranslator.FromHtml("#2E8B57");

    public static readonly Font BaseFont = new("Times New Roman", 12F, FontStyle.Regular);
    public static readonly Font TitleFont = new("Times New Roman", 18F, FontStyle.Bold);
    private static Icon? _appIcon;

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool DestroyIcon(IntPtr handle);

    public static void Apply(Form form)
    {
        form.Font = BaseFont;
        form.BackColor = MainBg;
        form.ShowIcon = true;
        var icon = _appIcon ??= LoadAppIcon();
        if (icon != null)
        {
            form.Icon = icon;
        }
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
    public static string ImportPath => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "import"));

    private static string? ResolveAssetPath(string fileName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return null;
            }

            if (Path.IsPathRooted(fileName) && File.Exists(fileName))
            {
                return fileName;
            }

            var assetsPath = Path.Combine(AssetsPath, fileName);
            if (File.Exists(assetsPath))
            {
                return assetsPath;
            }

            var importPath = Path.Combine(ImportPath, fileName);
            return File.Exists(importPath) ? importPath : null;
        }
        catch
        {
            return null;
        }
    }

    public static Image? LoadAsset(string fileName)
    {
        try
        {
            var path = ResolveAssetPath(fileName);
            return path == null ? null : Image.FromFile(path);
        }
        catch
        {
            return null;
        }
    }

    public static Icon? LoadIcon(string fileName)
    {
        try
        {
            var path = ResolveAssetPath(fileName);
            return path == null ? null : new Icon(path);
        }
        catch
        {
            return null;
        }
    }

    private static Icon? LoadAppIcon()
    {
        var icon = LoadIcon(AppIcon);
        if (icon != null && icon.Width <= 64)
        {
            return icon;
        }

        var smallIcon = LoadIconFromPng("Icon.png", 32);
        return smallIcon ?? icon;
    }

    private static Icon? LoadIconFromPng(string fileName, int size)
    {
        try
        {
            using var img = LoadAsset(fileName);
            if (img == null) return null;
            using var bmp = new Bitmap(img, new Size(size, size));
            var hicon = bmp.GetHicon();
            var temp = Icon.FromHandle(hicon);
            var clone = (Icon)temp.Clone();
            temp.Dispose();
            DestroyIcon(hicon);
            return clone;
        }
        catch
        {
            return null;
        }
    }
}
