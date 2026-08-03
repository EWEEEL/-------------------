using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RecipeKeeper;

public partial class MainWindow : Window
{
    private static readonly Dictionary<string, (string Ru, string En)> Translations = new()
    {
        ["AppName"] = ("Книга рецептов", "Recipe Book"),
        ["AppSubtitle"] = ("Коллекция рецептов", "Recipe collection"),
        ["AllRecipes"] = ("Все рецепты", "All recipes"),
        ["Favorites"] = ("Избранное", "Favorites"),
        ["Categories"] = ("Категории", "Categories"),
        ["Collection"] = ("Коллекция", "Collection"),
        ["MyRecipesTitle"] = ("Мои рецепты", "My recipes"),
        ["AllRecipesSubtitle"] = ("Все сохраненные рецепты, ингредиенты и пошаговые заметки.", "All saved recipes, ingredients, and step-by-step notes."),
        ["FavoritesSubtitle"] = ("Рецепты, которые вы отметили как любимые.", "Recipes marked as favorites."),
        ["CategoriesSubtitle"] = ("Группировка рецептов по типам блюд.", "Recipes grouped by dish type."),
        ["CategoryRecipesSubtitle"] = ("Рецепты из категории", "Recipes from category"),
        ["NewRecipe"] = ("Новый рецепт", "New recipe"),
        ["Search"] = ("Поиск", "Search"),
        ["SearchShortcutTooltip"] = ("Перейти к поиску", "Focus search"),
        ["SearchPlaceholder"] = ("Поиск по названию или ингредиенту", "Search by title or ingredient"),
        ["SearchTooltip"] = ("Поиск по названию, категории, ингредиентам и шагам", "Search by title, category, ingredients, and steps"),
        ["AllCategories"] = ("Все категории", "All categories"),
        ["EnableDarkTheme"] = ("Включить темную тему", "Enable dark theme"),
        ["EnableLightTheme"] = ("Включить светлую тему", "Enable light theme"),
        ["SwitchLanguage"] = ("Переключить язык", "Switch language"),
        ["SwitchToEnglish"] = ("Switch to English", "Switch to English"),
        ["SwitchToRussian"] = ("Переключить на русский", "Switch to Russian"),
        ["EditorSubtitle"] = ("Добавьте название, категорию, ингредиенты и пошаговый процесс.", "Add a title, category, ingredients, and cooking steps."),
        ["TitleLabel"] = ("Название", "Title"),
        ["CategoryLabel"] = ("Категория", "Category"),
        ["NewCategoryLabel"] = ("Новая категория", "New category"),
        ["NewCategoryTooltip"] = ("Заполните, если нужной категории нет в списке", "Fill this in when the category is not in the list"),
        ["FavoriteCheckbox"] = ("Добавить в избранное", "Add to favorites"),
        ["Ingredients"] = ("Ингредиенты", "Ingredients"),
        ["Steps"] = ("Шаги приготовления", "Cooking steps"),
        ["AddIngredient"] = ("+ ингредиент", "+ ingredient"),
        ["AddStep"] = ("+ шаг", "+ step"),
        ["Cancel"] = ("Отмена", "Cancel"),
        ["SaveRecipe"] = ("Сохранить рецепт", "Save recipe"),
        ["EnterRecipeTitle"] = ("Введите название рецепта.", "Enter a recipe title."),
        ["AddIngredientValidation"] = ("Добавьте хотя бы один ингредиент.", "Add at least one ingredient."),
        ["AddStepValidation"] = ("Добавьте хотя бы один шаг приготовления.", "Add at least one cooking step."),
        ["NoCategory"] = ("Без категории", "Uncategorized"),
        ["NoResultsTitle"] = ("Ничего не найдено", "Nothing found"),
        ["NoResultsSubtitle"] = ("Попробуйте изменить поиск или категорию.", "Try changing the search text or category."),
        ["SelectRecipeTitle"] = ("Выберите рецепт", "Select a recipe"),
        ["SelectRecipeSubtitle"] = ("Здесь появятся ингредиенты и шаги приготовления.", "Ingredients and cooking steps will appear here."),
        ["CategoriesEmptyTitle"] = ("Категорий пока нет", "No categories yet"),
        ["CategoriesEmptySubtitle"] = ("Создайте рецепт и укажите новую категорию.", "Create a recipe and enter a new category."),
        ["NewCategoryPanelTitle"] = ("Новая категория", "New category"),
        ["NewCategoryPanelText"] = ("Категории создаются автоматически: укажите новое название в форме рецепта.", "Categories are created automatically: enter a new name in the recipe form."),
        ["IngredientNameTooltip"] = ("Название ингредиента", "Ingredient name"),
        ["QuantityTooltip"] = ("Количество", "Quantity"),
        ["StepTextTooltip"] = ("Описание шага", "Step description"),
        ["MinutesTooltip"] = ("Минуты", "Minutes"),
        ["RemoveRow"] = ("Удалить строку", "Remove row"),
        ["DefaultStepOne"] = ("Подготовьте ингредиенты.", "Prepare the ingredients."),
        ["DefaultStepTwo"] = ("Опишите следующий этап приготовления.", "Describe the next cooking step."),
        ["FavoriteOn"] = ("Убрать из избранного", "Remove from favorites"),
        ["FavoriteOff"] = ("Добавить в избранное", "Add to favorites"),
        ["DeleteRecipe"] = ("Удалить рецепт", "Delete recipe"),
        ["DeleteRecipeShort"] = ("Удалить", "Delete"),
        ["DeleteRecipeConfirmation"] = ("Удалить рецепт \"{0}\"? Это действие нельзя отменить.", "Delete \"{0}\"? This action cannot be undone."),
        ["DeleteRecipeTitle"] = ("Удаление рецепта", "Delete recipe"),
        ["DialogOk"] = ("OK", "OK"),
        ["DialogYes"] = ("Да", "Yes"),
        ["DialogNo"] = ("Нет", "No")
    };

    private static readonly string[] AllCategoryLabels = ["Все категории", "All categories"];
    private static readonly (string Title, string Category)[] LegacyStarterRecipes =
    [
        ("\u041F\u0430\u0441\u0442\u0430 \u0441 \u0442\u043E\u043C\u0430\u0442\u0430\u043C\u0438 \u0438 \u0431\u0430\u0437\u0438\u043B\u0438\u043A\u043E\u043C", "\u0418\u0442\u0430\u043B\u0438\u044F"),
        ("\u041A\u0443\u0440\u0438\u043D\u044B\u0439 \u0441\u0443\u043F \u0441 \u0437\u0435\u043B\u0435\u043D\u044C\u044E", "\u0414\u043E\u043C\u0430\u0448\u043D\u0435\u0435"),
        ("\u0427\u0435\u0440\u043D\u0438\u0447\u043D\u044B\u0439 \u0447\u0438\u0437\u043A\u0435\u0439\u043A", "\u0412\u044B\u043F\u0435\u0447\u043A\u0430")
    ];

    private readonly string _dataFilePath = System.IO.Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "RecipeKeeper",
        "recipes.json");

    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };
    private readonly List<Recipe> _recipes = [];

    private AppView _currentView = AppView.AllRecipes;
    private UiLanguage _language = UiLanguage.Ru;
    private Recipe? _selectedRecipe;
    private bool _isDarkTheme;
    private bool _isUpdatingFilters;
    private TaskCompletionSource<bool>? _dialogCompletionSource;

    public MainWindow()
    {
        InitializeComponent();
        LoadRecipes();
        ApplyLanguage();
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        // Window resizing is intentionally disabled for a stable fixed layout.
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void LanguageToggleButton_Click(object sender, RoutedEventArgs e)
    {
        _language = _language == UiLanguage.Ru ? UiLanguage.En : UiLanguage.Ru;
        ApplyLanguage();
    }

    private void ThemeToggleButton_Click(object sender, RoutedEventArgs e)
    {
        _isDarkTheme = !_isDarkTheme;
        ApplyTheme(_isDarkTheme);
        UpdateThemeButtonText();
        RenderCurrentView();
    }

    private void SearchButton_Click(object sender, RoutedEventArgs e)
    {
        SearchTextBox.Focus();
        SearchTextBox.SelectAll();
    }

    private void NewRecipeButton_Click(object sender, RoutedEventArgs e)
    {
        OpenRecipeEditor();
    }

    private void ShowAllRecipes_Click(object sender, RoutedEventArgs e)
    {
        SetCurrentView(AppView.AllRecipes);
    }

    private void ShowFavorites_Click(object sender, RoutedEventArgs e)
    {
        SetCurrentView(AppView.Favorites);
    }

    private void ShowCategories_Click(object sender, RoutedEventArgs e)
    {
        SetCurrentView(AppView.Categories);
    }

    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!_isUpdatingFilters)
        {
            RenderCurrentView();
        }
    }

    private void CategoryFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_isUpdatingFilters)
        {
            RenderCurrentView();
        }
    }

    private void AddIngredientButton_Click(object sender, RoutedEventArgs e)
    {
        AddIngredientRow();
    }

    private void AddStepButton_Click(object sender, RoutedEventArgs e)
    {
        AddStepRow();
    }

    private void CancelEditorButton_Click(object sender, RoutedEventArgs e)
    {
        EditorOverlay.Visibility = Visibility.Collapsed;
    }

    private async void SaveRecipeButton_Click(object sender, RoutedEventArgs e)
    {
        var title = RecipeTitleTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(title))
        {
            await ShowInfoDialogAsync(T("AppName"), T("EnterRecipeTitle"));
            RecipeTitleTextBox.Focus();
            return;
        }

        var category = NewCategoryTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(category))
        {
            category = RecipeCategoryComboBox.Text.Trim();
        }

        if (string.IsNullOrWhiteSpace(category))
        {
            category = T("NoCategory");
        }

        var ingredients = ReadIngredientRows();
        if (ingredients.Count == 0)
        {
            await ShowInfoDialogAsync(T("AppName"), T("AddIngredientValidation"));
            return;
        }

        var steps = ReadStepRows();
        if (steps.Count == 0)
        {
            await ShowInfoDialogAsync(T("AppName"), T("AddStepValidation"));
            return;
        }

        var recipe = new Recipe
        {
            Id = Guid.NewGuid(),
            Title = title,
            Category = category,
            IsFavorite = FavoriteRecipeCheckBox.IsChecked == true,
            Ingredients = ingredients,
            Steps = steps,
            CreatedAt = DateTimeOffset.Now
        };

        _recipes.Insert(0, recipe);
        _selectedRecipe = recipe;
        SaveRecipes();
        RefreshCategoryControls();
        EditorOverlay.Visibility = Visibility.Collapsed;
        SetCurrentView(AppView.AllRecipes);
    }

    private void SetCurrentView(AppView view)
    {
        _currentView = view;
        RenderCurrentView();
    }

    private void LoadRecipes()
    {
        try
        {
            if (File.Exists(_dataFilePath))
            {
                var savedRecipes = JsonSerializer.Deserialize<List<Recipe>>(File.ReadAllText(_dataFilePath)) ?? [];
                var recipes = savedRecipes
                    .Where(recipe => !IsLegacyStarterRecipe(recipe))
                    .ToList();

                _recipes.AddRange(recipes);
                _selectedRecipe = _recipes.FirstOrDefault();

                if (recipes.Count != savedRecipes.Count)
                {
                    SaveRecipes();
                }

                return;
            }
        }
        catch
        {
            // If the local file is malformed, keep the app open and let the next save replace it.
            _selectedRecipe = null;
            return;
        }

        _selectedRecipe = null;
        SaveRecipes();
    }

    private void SaveRecipes()
    {
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(_dataFilePath)!);
        File.WriteAllText(_dataFilePath, JsonSerializer.Serialize(_recipes, _jsonOptions));
    }

    private void ApplyLanguage()
    {
        Title = T("AppName");
        TitleBarAppNameText.Text = T("AppName");
        SidebarTitleText.Text = T("AppName");
        SidebarSubtitleText.Text = T("AppSubtitle");
        AllRecipesNavText.Text = T("AllRecipes");
        FavoritesNavText.Text = T("Favorites");
        CategoriesNavText.Text = T("Categories");
        CollectionLabelText.Text = T("Collection");

        SearchShortcutButton.ToolTip = T("SearchShortcutTooltip");
        SearchShortcutButton.SetValue(AutomationProperties.NameProperty, T("Search"));
        NewRecipeButton.SetValue(AutomationProperties.NameProperty, T("NewRecipe"));
        NewRecipeButtonText.Text = T("NewRecipe");
        SearchTextBox.ToolTip = T("SearchTooltip");
        SearchPlaceholderText.Text = T("SearchPlaceholder");

        EditorTitleText.Text = T("NewRecipe");
        EditorSubtitleText.Text = T("EditorSubtitle");
        RecipeTitleLabelText.Text = T("TitleLabel");
        RecipeCategoryLabelText.Text = T("CategoryLabel");
        NewCategoryLabelText.Text = T("NewCategoryLabel");
        NewCategoryTextBox.ToolTip = T("NewCategoryTooltip");
        FavoriteRecipeCheckBox.Content = T("FavoriteCheckbox");
        IngredientsLabelText.Text = T("Ingredients");
        StepsLabelText.Text = T("Steps");
        AddIngredientButton.Content = T("AddIngredient");
        AddStepButton.Content = T("AddStep");
        CancelEditorButton.Content = T("Cancel");
        SaveRecipeButton.Content = T("SaveRecipe");

        LanguageToggleButton.Content = _language == UiLanguage.Ru ? "EN" : "RU";
        LanguageToggleButton.ToolTip = _language == UiLanguage.Ru ? T("SwitchToEnglish") : T("SwitchToRussian");
        LanguageToggleButton.SetValue(AutomationProperties.NameProperty, T("SwitchLanguage"));
        UpdateThemeButtonText();

        RefreshCategoryControls();
        RenderCurrentView();
    }

    private void RefreshCategoryControls()
    {
        _isUpdatingFilters = true;

        var selectedCategory = CategoryFilterComboBox.SelectedItem as string;
        if (IsAllCategoriesLabel(selectedCategory))
        {
            selectedCategory = T("AllCategories");
        }

        var categories = GetCategories();
        var allCategories = T("AllCategories");

        CategoryFilterComboBox.Items.Clear();
        CategoryFilterComboBox.Items.Add(allCategories);
        foreach (var category in categories)
        {
            CategoryFilterComboBox.Items.Add(category);
        }

        CategoryFilterComboBox.SelectedItem = selectedCategory is not null && categories.Contains(selectedCategory)
            ? selectedCategory
            : allCategories;

        RecipeCategoryComboBox.Items.Clear();
        foreach (var category in categories)
        {
            RecipeCategoryComboBox.Items.Add(category);
        }

        _isUpdatingFilters = false;
    }

    private List<string> GetCategories()
    {
        return _recipes
            .Select(recipe => recipe.Category)
            .Where(category => !string.IsNullOrWhiteSpace(category))
            .Distinct(StringComparer.CurrentCultureIgnoreCase)
            .OrderBy(category => category)
            .ToList();
    }

    private void RenderCurrentView()
    {
        UpdateHeader();
        UpdateStats();
        UpdateNavigationState();
        RecipeListPanel.Children.Clear();
        RightPanel.Children.Clear();

        switch (_currentView)
        {
            case AppView.Categories:
                RenderCategories();
                break;
            default:
                RenderRecipeList();
                break;
        }
    }

    private void UpdateHeader()
    {
        switch (_currentView)
        {
            case AppView.Favorites:
                HeaderTitle.Text = T("Favorites");
                HeaderSubtitle.Text = T("FavoritesSubtitle");
                break;
            case AppView.Categories:
                HeaderTitle.Text = T("Categories");
                HeaderSubtitle.Text = T("CategoriesSubtitle");
                break;
            default:
                HeaderTitle.Text = T("MyRecipesTitle");
                HeaderSubtitle.Text = T("AllRecipesSubtitle");
                break;
        }
    }

    private void UpdateNavigationState()
    {
        SetNavigationButtonState(AllRecipesButton, _currentView == AppView.AllRecipes);
        SetNavigationButtonState(FavoritesButton, _currentView == AppView.Favorites);
        SetNavigationButtonState(CategoriesButton, _currentView == AppView.Categories);
    }

    private void SetNavigationButtonState(Button button, bool isActive)
    {
        button.Background = isActive ? (Brush)FindResource("InputBrush") : Brushes.Transparent;
        button.Foreground = isActive ? (Brush)FindResource("TextBrush") : (Brush)FindResource("MutedTextBrush");
    }

    private void RenderRecipeList()
    {
        var recipes = GetFilteredRecipes().ToList();

        if (recipes.Count == 0)
        {
            RecipeListPanel.Children.Add(CreateEmptyState(T("NoResultsTitle"), T("NoResultsSubtitle")));
        }
        else
        {
            foreach (var recipe in recipes)
            {
                RecipeListPanel.Children.Add(CreateRecipeCard(recipe));
            }
        }

        _selectedRecipe = _selectedRecipe is not null && recipes.Any(recipe => recipe.Id == _selectedRecipe.Id)
            ? _selectedRecipe
            : recipes.FirstOrDefault();

        RenderRecipeDetails(_selectedRecipe);
    }

    private IEnumerable<Recipe> GetFilteredRecipes()
    {
        IEnumerable<Recipe> query = _recipes;

        if (_currentView == AppView.Favorites)
        {
            query = query.Where(recipe => recipe.IsFavorite);
        }

        var selectedCategory = CategoryFilterComboBox.SelectedItem as string;
        if (!string.IsNullOrWhiteSpace(selectedCategory) && !IsAllCategoriesLabel(selectedCategory))
        {
            query = query.Where(recipe => string.Equals(recipe.Category, selectedCategory, StringComparison.CurrentCultureIgnoreCase));
        }

        var search = SearchTextBox.Text.Trim();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(recipe =>
                recipe.Title.Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                recipe.Category.Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                recipe.Ingredients.Any(ingredient => ingredient.Name.Contains(search, StringComparison.CurrentCultureIgnoreCase)) ||
                recipe.Steps.Any(step => step.Text.Contains(search, StringComparison.CurrentCultureIgnoreCase)));
        }

        return query;
    }

    private Border CreateRecipeCard(Recipe recipe)
    {
        var card = new Border { Style = (Style)FindResource("RecipeCardStyle"), Cursor = Cursors.Hand };
        card.MouseLeftButtonUp += (_, _) =>
        {
            _selectedRecipe = recipe;
            RenderCurrentView();
        };

        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(54) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var icon = new Border
        {
            Width = 42,
            Height = 42,
            CornerRadius = new CornerRadius(8),
            Background = new SolidColorBrush(GetCategoryColor(recipe.Category)),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center
        };
        icon.Child = new TextBlock
        {
            Text = recipe.Title[..1].ToUpperInvariant(),
            Foreground = Brushes.White,
            FontWeight = FontWeights.Bold,
            FontSize = 17,
            LineHeight = 20,
            LineStackingStrategy = LineStackingStrategy.BlockLineHeight,
            TextAlignment = TextAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        grid.Children.Add(icon);

        var content = new StackPanel { VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 12, 0) };
        content.Children.Add(new TextBlock
        {
            Text = recipe.Title,
            Foreground = (Brush)FindResource("TextBrush"),
            FontSize = 16,
            FontWeight = FontWeights.SemiBold,
            TextTrimming = TextTrimming.CharacterEllipsis
        });
        content.Children.Add(new TextBlock
        {
            Text = $"{FormatCount(recipe.Ingredients.Count, "Ingredient")} · {FormatCount(recipe.Steps.Count, "Step")} · {recipe.Category}",
            Foreground = (Brush)FindResource("MutedTextBrush"),
            FontSize = 13,
            Margin = new Thickness(0, 5, 0, 0),
            TextTrimming = TextTrimming.CharacterEllipsis
        });
        Grid.SetColumn(content, 1);
        grid.Children.Add(content);

        var actions = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
        var favoriteButton = new Button
        {
            Content = recipe.IsFavorite ? "★" : "☆",
            Style = (Style)FindResource("IconButtonStyle"),
            ToolTip = recipe.IsFavorite ? T("FavoriteOn") : T("FavoriteOff"),
            Margin = new Thickness(0, 0, 8, 0)
        };
        favoriteButton.Click += (sender, args) =>
        {
            args.Handled = true;
            recipe.IsFavorite = !recipe.IsFavorite;
            SaveRecipes();
            RenderCurrentView();
        };
        actions.Children.Add(favoriteButton);
        var deleteButton = new Button
        {
            Content = "×",
            Style = (Style)FindResource("IconButtonStyle"),
            ToolTip = T("DeleteRecipe"),
            Margin = new Thickness(0, 0, 8, 0)
        };
        deleteButton.Click += async (sender, args) =>
        {
            args.Handled = true;
            await DeleteRecipeAsync(recipe);
        };
        actions.Children.Add(deleteButton);
        actions.Children.Add(CreateChip(recipe.Category, GetCategoryColor(recipe.Category)));
        Grid.SetColumn(actions, 2);
        grid.Children.Add(actions);

        card.Child = grid;
        return card;
    }

    private void RenderRecipeDetails(Recipe? recipe)
    {
        if (recipe is null)
        {
            RightPanel.Children.Add(CreateEmptyState(T("SelectRecipeTitle"), T("SelectRecipeSubtitle")));
            return;
        }

        RightPanel.Children.Add(CreatePanelBlock(stack =>
        {
            stack.Children.Add(new TextBlock
            {
                Text = recipe.Title,
                Foreground = (Brush)FindResource("TextBrush"),
                FontSize = 22,
                FontWeight = FontWeights.SemiBold,
                TextWrapping = TextWrapping.Wrap
            });
            stack.Children.Add(new TextBlock
            {
                Text = recipe.Category,
                Foreground = (Brush)FindResource("MutedTextBrush"),
                FontSize = 13,
                Margin = new Thickness(0, 5, 0, 14)
            });
            stack.Children.Add(new TextBlock
            {
                Text = T("Ingredients"),
                Foreground = (Brush)FindResource("TextBrush"),
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 8)
            });
            foreach (var ingredient in recipe.Ingredients)
            {
                stack.Children.Add(new TextBlock
                {
                    Text = $"• {ingredient.Name} — {FormatQuantity(ingredient.Quantity)} {ingredient.Unit}",
                    Foreground = (Brush)FindResource("MutedTextBrush"),
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 0, 0, 4)
                });
            }

            stack.Children.Add(new TextBlock
            {
                Text = T("Steps"),
                Foreground = (Brush)FindResource("TextBrush"),
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 14, 0, 8)
            });
            for (var i = 0; i < recipe.Steps.Count; i++)
            {
                var step = recipe.Steps[i];
                var duration = step.DurationMinutes > 0 ? $" · {step.DurationMinutes} {MinuteLabel()}" : "";
                stack.Children.Add(new TextBlock
                {
                    Text = $"{i + 1}. {step.Text}{duration}",
                    Foreground = (Brush)FindResource("MutedTextBrush"),
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 0, 0, 6)
                });
            }

            var deleteButton = new Button
            {
                Content = T("DeleteRecipeShort"),
                Style = (Style)FindResource("SecondaryButtonStyle"),
                Foreground = new SolidColorBrush(FromHex("#D94F45")),
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 14, 0, 0),
                ToolTip = T("DeleteRecipe")
            };
            deleteButton.Click += async (_, _) => await DeleteRecipeAsync(recipe);
            stack.Children.Add(deleteButton);
        }));
    }

    private async Task DeleteRecipeAsync(Recipe recipe)
    {
        var message = string.Format(CultureInfo.CurrentCulture, T("DeleteRecipeConfirmation"), recipe.Title);
        var confirmed = await ShowConfirmDialogAsync(T("DeleteRecipeTitle"), message);
        if (!confirmed)
        {
            return;
        }

        var index = _recipes.FindIndex(item => item.Id == recipe.Id);
        if (index < 0)
        {
            return;
        }

        _recipes.RemoveAt(index);
        _selectedRecipe = _recipes.Count == 0
            ? null
            : _recipes[Math.Min(index, _recipes.Count - 1)];

        SaveRecipes();
        RefreshCategoryControls();
        RenderCurrentView();
    }

    private Task ShowInfoDialogAsync(string title, string message)
    {
        return ShowAppDialogAsync(title, message, AppDialogKind.Info, false);
    }

    private Task<bool> ShowConfirmDialogAsync(string title, string message)
    {
        return ShowAppDialogAsync(title, message, AppDialogKind.Warning, true);
    }

    private Task<bool> ShowAppDialogAsync(string title, string message, AppDialogKind kind, bool isConfirmation)
    {
        if (_dialogCompletionSource is not null)
        {
            return Task.FromResult(false);
        }

        _dialogCompletionSource = new TaskCompletionSource<bool>();
        DialogTitleText.Text = title;
        DialogMessageText.Text = message;
        DialogIconText.Text = kind == AppDialogKind.Warning ? "!" : "i";
        DialogIconText.Foreground = kind == AppDialogKind.Warning
            ? new SolidColorBrush(FromHex("#D97706"))
            : (Brush)FindResource("BlueAccentBrush");
        DialogPrimaryButton.Content = isConfirmation ? T("DialogYes") : T("DialogOk");
        DialogSecondaryButton.Content = T("DialogNo");
        DialogSecondaryButton.Visibility = isConfirmation ? Visibility.Visible : Visibility.Collapsed;
        DialogOverlay.Visibility = Visibility.Visible;
        DialogPrimaryButton.Focus();

        return _dialogCompletionSource.Task;
    }

    private void DialogPrimaryButton_Click(object sender, RoutedEventArgs e)
    {
        CompleteDialog(true);
    }

    private void DialogSecondaryButton_Click(object sender, RoutedEventArgs e)
    {
        CompleteDialog(false);
    }

    private void CompleteDialog(bool result)
    {
        DialogOverlay.Visibility = Visibility.Collapsed;
        var completionSource = _dialogCompletionSource;
        _dialogCompletionSource = null;
        completionSource?.TrySetResult(result);
    }

    private void RenderCategories()
    {
        var categories = _recipes
            .GroupBy(recipe => recipe.Category)
            .OrderBy(group => group.Key)
            .ToList();

        if (categories.Count == 0)
        {
            RecipeListPanel.Children.Add(CreateEmptyState(T("CategoriesEmptyTitle"), T("CategoriesEmptySubtitle")));
        }

        foreach (var group in categories)
        {
            var card = new Border { Style = (Style)FindResource("RecipeCardStyle"), Cursor = Cursors.Hand };
            card.MouseLeftButtonUp += (_, _) =>
            {
                _currentView = AppView.AllRecipes;
                CategoryFilterComboBox.SelectedItem = group.Key;
                HeaderTitle.Text = T("MyRecipesTitle");
                HeaderSubtitle.Text = $"{T("CategoryRecipesSubtitle")} \"{group.Key}\".";
                RenderCurrentView();
            };

            var stack = new StackPanel();
            stack.Children.Add(new TextBlock
            {
                Text = group.Key,
                Foreground = (Brush)FindResource("TextBrush"),
                FontSize = 18,
                FontWeight = FontWeights.SemiBold
            });
            stack.Children.Add(new TextBlock
            {
                Text = FormatCount(group.Count(), "Recipe"),
                Foreground = (Brush)FindResource("MutedTextBrush"),
                FontSize = 13,
                Margin = new Thickness(0, 4, 0, 0)
            });
            card.Child = stack;
            RecipeListPanel.Children.Add(card);
        }

        RightPanel.Children.Add(CreatePanelBlock(stack =>
        {
            stack.Children.Add(new TextBlock
            {
                Text = T("NewCategoryPanelTitle"),
                Foreground = (Brush)FindResource("TextBrush"),
                FontSize = 20,
                FontWeight = FontWeights.SemiBold
            });
            stack.Children.Add(new TextBlock
            {
                Text = T("NewCategoryPanelText"),
                Foreground = (Brush)FindResource("MutedTextBrush"),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 8, 0, 14)
            });
            var button = new Button { Content = T("NewRecipe"), Style = (Style)FindResource("PrimaryButtonStyle") };
            button.Click += NewRecipeButton_Click;
            stack.Children.Add(button);
        }));
    }

    private void OpenRecipeEditor()
    {
        RecipeTitleTextBox.Clear();
        NewCategoryTextBox.Clear();
        FavoriteRecipeCheckBox.IsChecked = false;
        RefreshCategoryControls();
        RecipeCategoryComboBox.Text = "";
        IngredientsPanel.Children.Clear();
        StepsPanel.Children.Clear();
        AddIngredientRow();
        AddIngredientRow();
        AddStepRow(T("DefaultStepOne"), 5);
        AddStepRow(T("DefaultStepTwo"), 10);
        EditorOverlay.Visibility = Visibility.Visible;
        RecipeTitleTextBox.Focus();
    }

    private void AddIngredientRow(string name = "", double quantity = 0, string unit = "")
    {
        var row = new Grid { Margin = new Thickness(0, 0, 0, 8), Tag = "IngredientRow" };
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(86) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(42) });

        var nameBox = new TextBox { Text = name, Style = (Style)FindResource("InputTextBoxStyle"), Margin = new Thickness(0, 0, 8, 0), ToolTip = T("IngredientNameTooltip") };
        var quantityBox = new TextBox { Text = quantity > 0 ? FormatQuantity(quantity) : "", Style = (Style)FindResource("InputTextBoxStyle"), Margin = new Thickness(0, 0, 8, 0), ToolTip = T("QuantityTooltip") };
        var unitBox = new ComboBox { Style = (Style)FindResource("InputComboBoxStyle"), Margin = new Thickness(0, 0, 8, 0) };
        var units = _language == UiLanguage.Ru
            ? new[] { "г", "мл", "шт", "ч. л.", "ст. л." }
            : new[] { "g", "ml", "pcs", "tsp", "tbsp" };
        foreach (var item in units)
        {
            unitBox.Items.Add(item);
        }
        unitBox.SelectedItem = string.IsNullOrWhiteSpace(unit) ? units[0] : unit;

        var removeButton = new Button { Content = "×", Style = (Style)FindResource("IconButtonStyle"), ToolTip = T("RemoveRow") };
        removeButton.Click += (_, _) => IngredientsPanel.Children.Remove(row);

        Grid.SetColumn(nameBox, 0);
        Grid.SetColumn(quantityBox, 1);
        Grid.SetColumn(unitBox, 2);
        Grid.SetColumn(removeButton, 3);
        row.Children.Add(nameBox);
        row.Children.Add(quantityBox);
        row.Children.Add(unitBox);
        row.Children.Add(removeButton);
        IngredientsPanel.Children.Add(row);
    }

    private void AddStepRow(string text = "", int duration = 0)
    {
        var row = new Grid { Margin = new Thickness(0, 0, 0, 8), Tag = "StepRow" };
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(96) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(42) });

        var textBox = new TextBox { Text = text, Style = (Style)FindResource("InputTextBoxStyle"), Margin = new Thickness(0, 0, 8, 0), ToolTip = T("StepTextTooltip") };
        var durationBox = new TextBox { Text = duration > 0 ? duration.ToString(CultureInfo.InvariantCulture) : "", Style = (Style)FindResource("InputTextBoxStyle"), Margin = new Thickness(0, 0, 8, 0), ToolTip = T("MinutesTooltip") };
        var removeButton = new Button { Content = "×", Style = (Style)FindResource("IconButtonStyle"), ToolTip = T("RemoveRow") };
        removeButton.Click += (_, _) => StepsPanel.Children.Remove(row);

        Grid.SetColumn(textBox, 0);
        Grid.SetColumn(durationBox, 1);
        Grid.SetColumn(removeButton, 2);
        row.Children.Add(textBox);
        row.Children.Add(durationBox);
        row.Children.Add(removeButton);
        StepsPanel.Children.Add(row);
    }

    private List<Ingredient> ReadIngredientRows()
    {
        var ingredients = new List<Ingredient>();
        foreach (var row in IngredientsPanel.Children.OfType<Grid>())
        {
            var boxes = row.Children.OfType<TextBox>().ToList();
            var unitBox = row.Children.OfType<ComboBox>().FirstOrDefault();
            if (boxes.Count < 2)
            {
                continue;
            }

            var name = boxes[0].Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            double.TryParse(boxes[1].Text.Trim().Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out var quantity);
            ingredients.Add(new Ingredient
            {
                Name = name,
                Quantity = quantity,
                Unit = unitBox?.Text.Trim() is { Length: > 0 } unit ? unit : (_language == UiLanguage.Ru ? "г" : "g")
            });
        }

        return ingredients;
    }

    private List<RecipeStep> ReadStepRows()
    {
        var steps = new List<RecipeStep>();
        foreach (var row in StepsPanel.Children.OfType<Grid>())
        {
            var boxes = row.Children.OfType<TextBox>().ToList();
            if (boxes.Count < 2)
            {
                continue;
            }

            var text = boxes[0].Text.Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                continue;
            }

            int.TryParse(boxes[1].Text.Trim(), out var duration);
            steps.Add(new RecipeStep { Text = text, DurationMinutes = duration });
        }

        return steps;
    }

    private void UpdateStats()
    {
        RecipeCountText.Text = FormatCount(_recipes.Count, "Recipe");
        var favoriteCount = _recipes.Count(recipe => recipe.IsFavorite);
        FavoriteCountText.Text = _language == UiLanguage.Ru
            ? $"{favoriteCount} любимых блюд"
            : $"{favoriteCount} favorite {(favoriteCount == 1 ? "dish" : "dishes")}";
    }

    private Border CreatePanelBlock(Action<StackPanel> build)
    {
        var border = new Border
        {
            Style = (Style)FindResource("RecipeCardStyle"),
            Margin = new Thickness(0, 0, 0, 16)
        };
        var stack = new StackPanel();
        build(stack);
        border.Child = stack;
        return border;
    }

    private Border CreateEmptyState(string title, string subtitle)
    {
        return CreatePanelBlock(stack =>
        {
            stack.Children.Add(new TextBlock { Text = title, Foreground = (Brush)FindResource("TextBrush"), FontSize = 18, FontWeight = FontWeights.SemiBold });
            stack.Children.Add(new TextBlock { Text = subtitle, Foreground = (Brush)FindResource("MutedTextBrush"), TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 6, 0, 0) });
        });
    }

    private Border CreateChip(string text, Color color)
    {
        return new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(28, color.R, color.G, color.B)),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(12, 7, 12, 7),
            Child = new TextBlock
            {
                Text = text,
                Foreground = new SolidColorBrush(color),
                FontSize = 12,
                FontWeight = FontWeights.SemiBold
            }
        };
    }

    private Color GetCategoryColor(string category)
    {
        return (Math.Abs(category.GetHashCode()) % 5) switch
        {
            0 => FromHex("#2563EB"),
            1 => FromHex("#9A5A2B"),
            2 => FromHex("#6B4C9A"),
            3 => FromHex("#365B73"),
            _ => FromHex("#D97706")
        };
    }

    private void ApplyTheme(bool dark)
    {
        if (dark)
        {
            SetBrush("AppBackgroundBrush", "#0F1115");
            SetBrush("TopBarBrush", "#111318");
            SetBrush("PanelBrush", "#181B22");
            SetBrush("CardBrush", "#181B22");
            SetBrush("InputBrush", "#20242B");
            SetBrush("TextBrush", "#F8FAFC");
            SetBrush("MutedTextBrush", "#AEBDB6");
            SetBrush("BorderBrushSoft", "#2B3038");
            SetBrush("AccentBrush", "#2563EB");
            SetBrush("AccentHoverBrush", "#1D4ED8");
            SetBrush("IconSurfaceBrush", "#20242B");
            SetBrush("PromoBrush", "#111827");
            SetBrush("BlueAccentBrush", "#60A5FA");
            return;
        }

        SetBrush("AppBackgroundBrush", "#F7F8F4");
        SetBrush("TopBarBrush", "#FBFCF8");
        SetBrush("PanelBrush", "#FFFFFF");
        SetBrush("CardBrush", "#FFFFFF");
        SetBrush("InputBrush", "#F3F6F4");
        SetBrush("TextBrush", "#111827");
        SetBrush("MutedTextBrush", "#66756E");
        SetBrush("BorderBrushSoft", "#DDE6DF");
        SetBrush("AccentBrush", "#1F2937");
        SetBrush("AccentHoverBrush", "#111827");
        SetBrush("IconSurfaceBrush", "#F3F7F3");
        SetBrush("PromoBrush", "#111827");
        SetBrush("BlueAccentBrush", "#2563EB");
    }

    private void UpdateThemeButtonText()
    {
        ThemeToggleButton.Content = _isDarkTheme ? "☀" : "☾";
        ThemeToggleButton.ToolTip = _isDarkTheme ? T("EnableLightTheme") : T("EnableDarkTheme");
        ThemeToggleButton.SetValue(AutomationProperties.NameProperty, ThemeToggleButton.ToolTip?.ToString() ?? T("SwitchLanguage"));
    }

    private void SetBrush(string key, string color)
    {
        Resources[key] = new SolidColorBrush(FromHex(color));
    }

    private string FormatCount(int value, string kind)
    {
        return kind switch
        {
            "Recipe" when _language == UiLanguage.Ru => $"{value} {Pluralize(value, "рецепт", "рецепта", "рецептов")}",
            "Ingredient" when _language == UiLanguage.Ru => $"{value} {Pluralize(value, "ингредиент", "ингредиента", "ингредиентов")}",
            "Step" when _language == UiLanguage.Ru => $"{value} {Pluralize(value, "шаг", "шага", "шагов")}",
            "Recipe" => $"{value} {(value == 1 ? "recipe" : "recipes")}",
            "Ingredient" => $"{value} {(value == 1 ? "ingredient" : "ingredients")}",
            "Step" => $"{value} {(value == 1 ? "step" : "steps")}",
            _ => value.ToString(CultureInfo.CurrentCulture)
        };
    }

    private string MinuteLabel()
    {
        return _language == UiLanguage.Ru ? "мин" : "min";
    }

    private string T(string key)
    {
        var value = Translations[key];
        return _language == UiLanguage.Ru ? value.Ru : value.En;
    }

    private static bool IsAllCategoriesLabel(string? value)
    {
        return value is not null && AllCategoryLabels.Contains(value);
    }

    private static bool IsLegacyStarterRecipe(Recipe recipe)
    {
        return LegacyStarterRecipes.Any(starterRecipe =>
            string.Equals(recipe.Title, starterRecipe.Title, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(recipe.Category, starterRecipe.Category, StringComparison.OrdinalIgnoreCase));
    }

    private static string FormatQuantity(double value)
    {
        return Math.Abs(value % 1) < 0.001 ? value.ToString("0", CultureInfo.InvariantCulture) : value.ToString("0.##", CultureInfo.InvariantCulture);
    }

    private static string Pluralize(int value, string one, string few, string many)
    {
        var mod10 = value % 10;
        var mod100 = value % 100;
        if (mod10 == 1 && mod100 != 11)
        {
            return one;
        }

        if (mod10 is >= 2 and <= 4 && mod100 is < 12 or > 14)
        {
            return few;
        }

        return many;
    }

    private static Color FromHex(string hex)
    {
        return (Color)ColorConverter.ConvertFromString(hex)!;
    }

    private enum AppView
    {
        AllRecipes,
        Favorites,
        Categories
    }

    private enum UiLanguage
    {
        Ru,
        En
    }

    private enum AppDialogKind
    {
        Info,
        Warning
    }

    private sealed class Recipe
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public string Category { get; set; } = "";
        public bool IsFavorite { get; set; }
        public List<Ingredient> Ingredients { get; set; } = [];
        public List<RecipeStep> Steps { get; set; } = [];
        public DateTimeOffset CreatedAt { get; set; }
    }

    private sealed class Ingredient
    {
        public string Name { get; set; } = "";
        public double Quantity { get; set; }
        public string Unit { get; set; } = "г";
    }

    private sealed class RecipeStep
    {
        public string Text { get; set; } = "";
        public int DurationMinutes { get; set; }
    }
}
