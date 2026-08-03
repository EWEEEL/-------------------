using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace RecipeKeeper.Tests;

[TestClass]
public sealed class MainWindowXamlTests
{
    [TestMethod]
    public void MainWindowKeepsExpectedApplicationIdentity()
    {
        var window = LoadMainWindow();

        Assert.AreEqual("Книга рецептов", (string?)window.Attribute("Title"));
        Assert.AreEqual("CenterScreen", (string?)window.Attribute("WindowStartupLocation"));
        Assert.AreEqual("1040", (string?)window.Attribute("MinWidth"));
        Assert.AreEqual("660", (string?)window.Attribute("MinHeight"));
        Assert.AreEqual("None", (string?)window.Attribute("WindowStyle"));
        Assert.AreEqual("NoResize", (string?)window.Attribute("ResizeMode"));
    }

    [TestMethod]
    public void MainWindowContainsClickableNavigationAndActions()
    {
        var text = File.ReadAllText(GetMainWindowPath());

        StringAssert.Contains(text, "AllRecipesButton");
        StringAssert.Contains(text, "FavoritesButton");
        StringAssert.Contains(text, "CategoriesButton");
        StringAssert.Contains(text, "NewRecipeButton_Click");
        StringAssert.Contains(text, "SearchButton_Click");
        StringAssert.Contains(text, "ThemeToggleButton_Click");
        StringAssert.Contains(text, "LanguageToggleButton");
        StringAssert.DoesNotMatch(text, new Regex("ShoppingButton"));
        StringAssert.Contains(text, "WindowChrome");
    }

    [TestMethod]
    public void MainWindowContainsRecipeEditorInputs()
    {
        var text = File.ReadAllText(GetMainWindowPath());

        StringAssert.Contains(text, "EditorOverlay");
        StringAssert.Contains(text, "DialogOverlay");
        StringAssert.Contains(text, "DialogPrimaryButton_Click");
        StringAssert.Contains(text, "RecipeTitleTextBox");
        StringAssert.Contains(text, "RecipeCategoryComboBox");
        StringAssert.Contains(text, "IngredientsPanel");
        StringAssert.Contains(text, "StepsPanel");
        StringAssert.Contains(text, "SaveRecipeButton_Click");
    }

    [TestMethod]
    public void MainWindowCodePersistsRecipesAndSupportsViews()
    {
        var text = File.ReadAllText(GetMainWindowCodePath());

        StringAssert.Contains(text, "recipes.json");
        StringAssert.Contains(text, "SaveRecipes");
        StringAssert.Contains(text, "LoadRecipes");
        StringAssert.Contains(text, "AppView.Favorites");
        StringAssert.Contains(text, "AppView.Categories");
        StringAssert.Contains(text, "LanguageToggleButton_Click");
        StringAssert.Contains(text, "UiLanguage.En");
        StringAssert.Contains(text, "DeleteRecipe");
        StringAssert.Contains(text, "DeleteRecipeConfirmation");
        StringAssert.Contains(text, "ShowAppDialogAsync");
        StringAssert.Contains(text, "LegacyStarterRecipes");
        StringAssert.Contains(text, "IsLegacyStarterRecipe");
        StringAssert.DoesNotMatch(text, new Regex("CreateStarterRecipes"));
        StringAssert.DoesNotMatch(text, new Regex("Паста с томатами"));
        StringAssert.DoesNotMatch(text, new Regex("Черничный чизкейк"));
        StringAssert.DoesNotMatch(text, new Regex("MessageBox\\.Show"));
        StringAssert.DoesNotMatch(text, new Regex("AppView.Shopping"));
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

    private static string GetMainWindowCodePath()
    {
        var root = FindRepositoryRoot();
        return Path.Combine(root.FullName, "src", "RecipeKeeper", "MainWindow.xaml.cs");
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
