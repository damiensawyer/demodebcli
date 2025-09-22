using System.Reflection;
using Terminal.Gui;
using Microsoft.Data.Sqlite;
using SQLitePCL;

// Initialize SQLite for AOT compilation
#if WINDOWS
Batteries_V2.Init();
#else
raw.SetProvider(new SQLitePCL.SQLite3Provider_sqlite3());
#endif

if (args.Length > 0)
{
    if (args[0] == "--version" || args[0] == "-v")
    {
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version?.ToString() ?? "Unknown";
        var buildDate = GetBuildTimeUtc();
        
        Console.WriteLine($"demodebcli version {version}");
        Console.WriteLine($"Built: {buildDate:yyyy-MM-dd HH:mm:ss} UTC");
        Console.WriteLine($"Local time: {buildDate.ToLocalTime():yyyy-MM-dd HH:mm:ss}");
    }
    else if (args[0] == "--help" || args[0] == "-h")
    {
        Console.WriteLine("demodebcli - Terminal.Gui Todo List Manager");
        Console.WriteLine();
        Console.WriteLine("USAGE:");
        Console.WriteLine("  demodebcli          Start the interactive todo list interface");
        Console.WriteLine("  demodebcli -v       Show version information");
        Console.WriteLine("  demodebcli -h       Show this help message");
        Console.WriteLine();
        Console.WriteLine("DATABASE:");
        var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "demodebcli", "todos.db");
        Console.WriteLine($"  Todo data is stored in: {dbPath}");
        Console.WriteLine();
        Console.WriteLine("CONTROLS:");
        Console.WriteLine("  Enter       Add new todo");
        Console.WriteLine("  Space       Toggle completion status");
        Console.WriteLine("  Delete      Delete selected todo");
        Console.WriteLine("  F5          Refresh list");
        Console.WriteLine("  Ctrl+Q      Quit application");
        Console.WriteLine();
        Console.WriteLine("FEATURES:");
        Console.WriteLine("  - Persistent SQLite database storage");
        Console.WriteLine("  - Full Terminal.Gui interface with mouse support");
        Console.WriteLine("  - Add, complete, and delete todo items");
        Console.WriteLine("  - Timestamps for each todo item");
    }
    else
    {
        Console.WriteLine("Unknown option. Use -h for help.");
    }
}
else
{
    var app = new TodoApp();
    app.Run();
}

static DateTime GetBuildTimeUtc()
{
    // Store build time as a compile-time constant string in UTC
    const string BuildTimeUtcString = "2025-09-12T01:55:08Z"; // This will be replaced during build
    
    if (DateTime.TryParse(BuildTimeUtcString, out var result))
    {
        return result.ToUniversalTime();
    }
    
    // Fallback to current time if parsing fails
    return DateTime.UtcNow;
}

public class TodoItem
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TodoDatabase
{
    private readonly string _connectionString;

    public TodoDatabase()
    {
        var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "demodebcli", "todos.db");
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
        _connectionString = $"Data Source={dbPath}";
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS TodoItems (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Text TEXT NOT NULL,
                IsCompleted BOOLEAN NOT NULL DEFAULT 0,
                CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
            )";
        command.ExecuteNonQuery();
    }

    public List<TodoItem> GetAllTodos()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Text, IsCompleted, CreatedAt FROM TodoItems ORDER BY CreatedAt DESC";

        var todos = new List<TodoItem>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            todos.Add(new TodoItem
            {
                Id = reader.GetInt32(0),
                Text = reader.GetString(1),
                IsCompleted = reader.GetBoolean(2),
                CreatedAt = reader.GetDateTime(3)
            });
        }

        return todos;
    }

    public void AddTodo(string text)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO TodoItems (Text) VALUES (@text)";
        command.Parameters.AddWithValue("@text", text);
        command.ExecuteNonQuery();
    }

    public void ToggleTodo(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "UPDATE TodoItems SET IsCompleted = NOT IsCompleted WHERE Id = @id";
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
    }

    public void DeleteTodo(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM TodoItems WHERE Id = @id";
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
    }
}

public class TodoApp
{
    private readonly TodoDatabase _database;
    private ListView _todoListView = null!;
    private TextField _newTodoField = null!;
    private List<TodoItem> _todos = new();

    public TodoApp()
    {
        _database = new TodoDatabase();
    }

    public void Run()
    {
        Application.Init();
        
        // Set up a dark color scheme
        var colors = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.White, Color.Black),
            Focus = new Terminal.Gui.Attribute(Color.BrightYellow, Color.DarkGray),
            HotNormal = new Terminal.Gui.Attribute(Color.Cyan, Color.Black),
            HotFocus = new Terminal.Gui.Attribute(Color.BrightYellow, Color.DarkGray),
            Disabled = new Terminal.Gui.Attribute(Color.Gray, Color.Black)
        };
        
        Application.Top.ColorScheme = colors;

        var top = Application.Top;

        var win = new Window("Todo List Manager")
        {
            X = 0,
            Y = 1,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            ColorScheme = colors
        };

        var menu = new MenuBar(new MenuBarItem[]
        {
            new("_App", new MenuItem[]
            {
                new("_Quit", "", () => Application.RequestStop())
            }),
            new("_Help", new MenuItem[]
            {
                new("_About", "", () => MessageBox.Query("About", "Todo List Manager v1.0\nBuilt with Terminal.Gui and SQLite", "OK"))
            })
        });

        _newTodoField = new TextField("")
        {
            X = 1,
            Y = 1,
            Width = Dim.Fill() - 15,
            Height = 1,
            ColorScheme = colors
        };

        var addButton = new Button("Add Todo")
        {
            X = Pos.Right(_newTodoField) + 1,
            Y = 1,
            ColorScheme = colors
        };

        _todoListView = new ListView(_todos)
        {
            X = 1,
            Y = 3,
            Width = Dim.Fill() - 1,
            Height = Dim.Fill() - 6,
            ColorScheme = colors
        };

        var toggleButton = new Button("Toggle Complete")
        {
            X = 1,
            Y = Pos.Bottom(_todoListView) + 1,
            ColorScheme = colors
        };

        var deleteButton = new Button("Delete")
        {
            X = Pos.Right(toggleButton) + 2,
            Y = Pos.Bottom(_todoListView) + 1,
            ColorScheme = colors
        };

        var refreshButton = new Button("Refresh")
        {
            X = Pos.Right(deleteButton) + 2,
            Y = Pos.Bottom(_todoListView) + 1,
            ColorScheme = colors
        };

        var statusBar = new StatusBar(new StatusItem[]
        {
            new(Key.CtrlMask | Key.Q, "~^Q~ Quit", () => Application.RequestStop()),
            new(Key.F5, "~F5~ Refresh", RefreshTodos),
            new(Key.Enter, "~Enter~ Add Todo", AddTodo)
        });

        addButton.Clicked += AddTodo;
        toggleButton.Clicked += ToggleSelectedTodo;
        deleteButton.Clicked += DeleteSelectedTodo;
        refreshButton.Clicked += RefreshTodos;

        _newTodoField.KeyPress += (args) =>
        {
            if (args.KeyEvent.Key == Key.Enter)
            {
                AddTodo();
                args.Handled = true;
            }
        };

        _todoListView.OpenSelectedItem += (args) => ToggleSelectedTodo();

        win.Add(_newTodoField, addButton, _todoListView, toggleButton, deleteButton, refreshButton);
        top.Add(menu, win, statusBar);

        RefreshTodos();

        Application.Run();
        Application.Shutdown();
    }

    private void RefreshTodos()
    {
        _todos = _database.GetAllTodos();
        var displayItems = _todos.Select(todo => 
            $"[{(todo.IsCompleted ? "✓" : " ")}] {todo.Text} ({todo.CreatedAt:yyyy-MM-dd HH:mm})"
        ).ToList();
        
        _todoListView.SetSource(displayItems);
    }

    private void AddTodo()
    {
        var text = _newTodoField.Text.ToString()?.Trim();
        if (!string.IsNullOrEmpty(text))
        {
            _database.AddTodo(text);
            _newTodoField.Text = "";
            RefreshTodos();
        }
    }

    private void ToggleSelectedTodo()
    {
        var selectedIndex = _todoListView.SelectedItem;
        if (selectedIndex >= 0 && selectedIndex < _todos.Count)
        {
            _database.ToggleTodo(_todos[selectedIndex].Id);
            RefreshTodos();
        }
    }

    private void DeleteSelectedTodo()
    {
        var selectedIndex = _todoListView.SelectedItem;
        if (selectedIndex >= 0 && selectedIndex < _todos.Count)
        {
            var result = MessageBox.Query("Delete Todo", 
                $"Are you sure you want to delete:\n'{_todos[selectedIndex].Text}'?", 
                "Yes", "No");
            
            if (result == 0)
            {
                _database.DeleteTodo(_todos[selectedIndex].Id);
                RefreshTodos();
            }
        }
    }
}
