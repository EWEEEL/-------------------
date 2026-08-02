using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RecipeKeeper;

public partial class MainWindow : Window
{
    private readonly Dictionary<DependencyObject, Dictionary<DependencyProperty, object?>> _originalThemeValues = [];
    private bool _isDarkTheme;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            ToggleWindowState();
            return;
        }

        DragMove();
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        ToggleWindowState();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ThemeToggleButton_Click(object sender, RoutedEventArgs e)
    {
        _isDarkTheme = !_isDarkTheme;
        ThemeToggleButton.Content = _isDarkTheme ? "☀" : "☾";
        ThemeToggleButton.ToolTip = _isDarkTheme ? "Включить светлую тему" : "Включить темную тему";
        ApplyTheme(_isDarkTheme);
    }

    private void SearchButton_Click(object sender, RoutedEventArgs e)
    {
        SearchTextBox.Focus();
        SearchTextBox.SelectAll();
    }

    private void ToggleWindowState()
    {
        WindowState = WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }

    private void ApplyTheme(bool dark)
    {
        if (dark)
        {
            ApplyDarkTheme(this);
            return;
        }

        RestoreLightTheme(this);
        _originalThemeValues.Clear();
    }

    private void ApplyDarkTheme(DependencyObject element)
    {
        switch (element)
        {
            case Window window:
                ApplyBrush(window, Window.BackgroundProperty, ThemeBrushKind.Surface);
                break;
            case Border border:
                ApplyBrush(border, Border.BackgroundProperty, ThemeBrushKind.Surface);
                ApplyBrush(border, Border.BorderBrushProperty, ThemeBrushKind.Border);
                break;
            case Panel panel:
                ApplyBrush(panel, Panel.BackgroundProperty, ThemeBrushKind.Surface);
                break;
            case Control control:
                ApplyBrush(control, Control.BackgroundProperty, ThemeBrushKind.Surface);
                ApplyBrush(control, Control.ForegroundProperty, ThemeBrushKind.Text);
                ApplyBrush(control, Control.BorderBrushProperty, ThemeBrushKind.Border);
                break;
            case TextBlock textBlock:
                ApplyBrush(textBlock, TextBlock.ForegroundProperty, ThemeBrushKind.Text);
                ApplyBrush(textBlock, TextBlock.BackgroundProperty, ThemeBrushKind.Surface);
                break;
            case Shape shape:
                ApplyBrush(shape, Shape.StrokeProperty, ThemeBrushKind.Text);
                break;
        }

        var childrenCount = VisualTreeHelper.GetChildrenCount(element);
        for (var i = 0; i < childrenCount; i++)
        {
            ApplyDarkTheme(VisualTreeHelper.GetChild(element, i));
        }
    }

    private void RestoreLightTheme(DependencyObject element)
    {
        if (_originalThemeValues.TryGetValue(element, out var values))
        {
            foreach (var (property, value) in values)
            {
                element.SetCurrentValue(property, value);
            }
        }

        var childrenCount = VisualTreeHelper.GetChildrenCount(element);
        for (var i = 0; i < childrenCount; i++)
        {
            RestoreLightTheme(VisualTreeHelper.GetChild(element, i));
        }
    }

    private void ApplyBrush(DependencyObject element, DependencyProperty property, ThemeBrushKind kind)
    {
        if (element.GetValue(property) is not Brush brush)
        {
            return;
        }

        var converted = ConvertBrush(brush, kind);
        if (ReferenceEquals(converted, brush))
        {
            return;
        }

        SaveOriginalValue(element, property, brush);
        element.SetCurrentValue(property, converted);
    }

    private void SaveOriginalValue(DependencyObject element, DependencyProperty property, object? value)
    {
        if (!_originalThemeValues.TryGetValue(element, out var values))
        {
            values = [];
            _originalThemeValues[element] = values;
        }

        values.TryAdd(property, value);
    }

    private Brush ConvertBrush(Brush brush, ThemeBrushKind kind)
    {
        return brush switch
        {
            SolidColorBrush solidColorBrush when TryConvertColor(solidColorBrush.Color, kind, out var color) => new SolidColorBrush(color),
            LinearGradientBrush gradientBrush => ConvertGradientBrush(gradientBrush, kind),
            _ => brush
        };
    }

    private LinearGradientBrush ConvertGradientBrush(LinearGradientBrush source, ThemeBrushKind kind)
    {
        var converted = source.Clone();
        var changed = false;

        foreach (var gradientStop in converted.GradientStops)
        {
            if (TryConvertColor(gradientStop.Color, kind, out var color))
            {
                gradientStop.Color = color;
                changed = true;
            }
        }

        return changed ? converted : source;
    }

    private static bool TryConvertColor(Color color, ThemeBrushKind kind, out Color converted)
    {
        var map = kind switch
        {
            ThemeBrushKind.Surface => SurfaceColors,
            ThemeBrushKind.Text => TextColors,
            ThemeBrushKind.Border => BorderColors,
            _ => SurfaceColors
        };

        return map.TryGetValue(color, out converted);
    }

    private static readonly Dictionary<Color, Color> SurfaceColors = new()
    {
        [FromHex("#F7F8F4")] = FromHex("#0F1115"),
        [FromHex("#F1F3F6")] = FromHex("#12151B"),
        [FromHex("#EEF4F1")] = FromHex("#12151B"),
        [FromHex("#FBF3EA")] = FromHex("#171411"),
        [FromHex("#FBFCF8")] = FromHex("#111318"),
        [FromHex("#FFFFFF")] = FromHex("#181B22"),
        [FromHex("#F3F6F4")] = FromHex("#20242B"),
        [FromHex("#F3F7F3")] = FromHex("#20242B"),
        [FromHex("#E8EFE9")] = FromHex("#252A32"),
        [FromHex("#E7EEE8")] = FromHex("#252A32"),
        [FromHex("#DDE8DF")] = FromHex("#2A3038"),
        [FromHex("#E8F0EA")] = FromHex("#20242B"),
        [FromHex("#EAF0FF")] = FromHex("#172033"),
        [FromHex("#EEF2FF")] = FromHex("#1D2433"),
        [FromHex("#EDEFF5")] = FromHex("#222832"),
        [FromHex("#F2F4F7")] = FromHex("#20242B"),
        [FromHex("#F7EFE4")] = FromHex("#33291E"),
        [FromHex("#F0EDF8")] = FromHex("#2C2638"),
        [FromHex("#EBF2F8")] = FromHex("#21313B"),
        [FromHex("#ECF2EE")] = FromHex("#252A32"),
        [FromHex("#E5EFE8")] = FromHex("#252A32"),
        [FromHex("#EDF4EE")] = FromHex("#172033"),
        [FromHex("#F4EEE5")] = FromHex("#34291F"),
        [FromHex("#E8E3F4")] = FromHex("#2E273A"),
        [FromHex("#DCEADE")] = FromHex("#24352D"),
        [FromHex("#F3D8C6")] = FromHex("#3D2B24"),
        [FromHex("#F7F1EA")] = FromHex("#30271F"),
        [FromHex("#F2EEF7")] = FromHex("#2C2635"),
        [FromHex("#EEF3F8")] = FromHex("#202E37"),
        [FromHex("#1F2937")] = FromHex("#111827"),
        [FromHex("#111827")] = FromHex("#0F172A"),
        [FromHex("#0B1220")] = FromHex("#111827"),
        [FromHex("#2563EB")] = FromHex("#60A5FA"),
        [FromHex("#2F6F57")] = FromHex("#2563EB"),
        [FromHex("#3F8268")] = FromHex("#2563EB"),
        [FromHex("#275D49")] = FromHex("#1D4ED8"),
        [FromHex("#1F4D3C")] = FromHex("#1E40AF"),
        [FromHex("#243A31")] = FromHex("#111827"),
    };

    private static readonly Dictionary<Color, Color> TextColors = new()
    {
        [FromHex("#1F2F2A")] = FromHex("#E8F0EA"),
        [FromHex("#20352D")] = FromHex("#EAF3ED"),
        [FromHex("#243A31")] = FromHex("#E7F0EA"),
        [FromHex("#111827")] = FromHex("#F8FAFC"),
        [FromHex("#40524A")] = FromHex("#CAD8D0"),
        [FromHex("#42524B")] = FromHex("#C9D7D0"),
        [FromHex("#485951")] = FromHex("#C5D4CC"),
        [FromHex("#52645C")] = FromHex("#C7D6CE"),
        [FromHex("#566760")] = FromHex("#CBD9D1"),
        [FromHex("#66756E")] = FromHex("#AEBDB6"),
        [FromHex("#6C7B74")] = FromHex("#AAB9B1"),
        [FromHex("#728079")] = FromHex("#A9B7B0"),
        [FromHex("#77847E")] = FromHex("#A8B5AF"),
        [FromHex("#6A7771")] = FromHex("#A7B5AE"),
        [FromHex("#2563EB")] = FromHex("#93C5FD"),
        [FromHex("#2F6F57")] = FromHex("#93C5FD"),
        [FromHex("#9A5A2B")] = FromHex("#D7A267"),
        [FromHex("#6B4C9A")] = FromHex("#B89CE5"),
        [FromHex("#7A5836")] = FromHex("#D0A070"),
        [FromHex("#684C8F")] = FromHex("#BDA5E3"),
        [FromHex("#365B73")] = FromHex("#91BED8"),
    };

    private static readonly Dictionary<Color, Color> BorderColors = new()
    {
        [FromHex("#DDE6DF")] = FromHex("#2B3038"),
        [FromHex("#E5ECE7")] = FromHex("#343A44"),
        [FromHex("#E2EAE4")] = FromHex("#343A44"),
        [FromHex("#D4E1D8")] = FromHex("#424A55"),
        [FromHex("#B8D0C2")] = FromHex("#5A6472"),
    };

    private static Color FromHex(string hex)
    {
        return (Color)ColorConverter.ConvertFromString(hex)!;
    }

    private enum ThemeBrushKind
    {
        Surface,
        Text,
        Border
    }
}
