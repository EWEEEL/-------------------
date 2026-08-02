using System.Xml.Linq;

namespace RecipeKeeper.Tests;

[TestClass]
public sealed class MainWindowXamlTests
{
    private static readonly XNamespace Presentation = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

    [TestMethod]
    public void MainWindowKeepsExpectedApplicationIdentity()
    {
        var window = LoadMainWindow();

        Assert.AreEqual("Книга рецептов", (string?)window.Attribute("Title"));
        Assert.AreEqual("CenterScreen", (string?)window.Attribute("WindowStartupLocation"));
        Assert.AreEqual("1040", (string?)window.Attribute("MinWidth"));
        Assert.AreEqual("660", (string?)window.Attribute("MinHeight"));
        Assert.AreEqual("None", (string?)window.Attribute("WindowStyle"));
    }

    [TestMethod]
    public void MainWindowContainsCoreRecipeInterfaceSections()
    {
        var text = File.ReadAllText(GetMainWindowPath());

        StringAssert.Contains(text, "Мои рецепты");
        StringAssert.Contains(text, "Новый рецепт");
        StringAssert.Contains(text, "Поиск по названию или ингредиенту");
        StringAssert.Contains(text, "Быстрые категории");
        StringAssert.Contains(text, "Паста с томатами и базиликом");
        StringAssert.Contains(text, "WindowChrome");
        StringAssert.Contains(text, "ThemeToggleButton");
        StringAssert.Contains(text, "CategoryComboBoxStyle");
        StringAssert.Contains(text, "ThemeToggleButton_Click");
        StringAssert.Contains(text, "SearchButton_Click");
    }

    [TestMethod]
    public void MainWindowUsesLightweightXamlAnimations()
    {
        var window = LoadMainWindow();

        var storyboards = window.Descendants(Presentation + "Storyboard").ToList();
        var doubleAnimations = window.Descendants(Presentation + "DoubleAnimation").ToList();

        Assert.IsGreaterThanOrEqualTo(3, storyboards.Count);
        Assert.IsGreaterThanOrEqualTo(6, doubleAnimations.Count);
    }

    private static XElement LoadMainWindow()
    {
        return XDocument.Load(GetMainWindowPath()).Root
            ?? throw new AssertFailedException("MainWindow.xaml does not contain a root element.");
    }

    private static string GetMainWindowPath()
    {
        var root = FindRepositoryRoot();
        return Path.Combine(root.FullName, "src", "RecipeKeeper", "MainWindow.xaml");
    }

    private static DirectoryInfo FindRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);

        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "RecipeKeeper.slnx")))
            {
                return current;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Could not find repository root with RecipeKeeper.slnx.");
    }
}
