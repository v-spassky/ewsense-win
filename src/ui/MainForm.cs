using ewsense.storage;

namespace ewsense.ui;

public class MainForm : Form
{
    private readonly KeywordRepository _keywordManager;
    private ListBox _keywordListBox = null!;
    private TextBox _keywordTextBox = null!;
    private Button _addButton = null!;
    private Button _removeButton = null!;

    public MainForm(KeywordRepository keywordRepository)
    {
        _keywordManager = keywordRepository ?? throw new ArgumentNullException(nameof(keywordRepository));
        InitializeComponent();
        _keywordManager.KeywordsChanged += OnKeywordsChanged;
        RefreshKeywordList();
    }

    private void InitializeComponent()
    {
        Text = "EWSense - Keyword Manager";
        Size = new Size(400, 300);
        StartPosition = FormStartPosition.CenterScreen;

        _keywordListBox = new ListBox
        {
            Location = new Point(10, 10),
            Size = new Size(200, 200)
        };
        _keywordListBox.KeyDown += KeywordListBox_KeyDown;

        _keywordTextBox = new TextBox
        {
            Location = new Point(220, 10),
            Size = new Size(150, 23)
        };
        _keywordTextBox.KeyDown += KeywordTextBox_KeyDown;

        _addButton = new Button
        {
            Text = "Add",
            Location = new Point(220, 40),
            Size = new Size(70, 23)
        };
        _addButton.Click += AddButton_Click;

        _removeButton = new Button
        {
            Text = "Remove",
            Location = new Point(300, 40),
            Size = new Size(70, 23)
        };
        _removeButton.Click += RemoveButton_Click;

        Controls.Add(_keywordListBox);
        Controls.Add(_keywordTextBox);
        Controls.Add(_addButton);
        Controls.Add(_removeButton);
    }

    private void KeywordTextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            AddButton_Click(sender, e);
            e.Handled = true;
        }
    }

    private void KeywordListBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Delete)
        {
            RemoveButton_Click(sender, e);
            e.Handled = true;
        }
    }

    private void AddButton_Click(object? sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(_keywordTextBox.Text))
        {
            _keywordManager.AddKeyword(_keywordTextBox.Text.Trim());
            _keywordTextBox.Clear();
        }
    }

    private void RemoveButton_Click(object? sender, EventArgs e)
    {
        if (_keywordListBox.SelectedItem != null)
        {
            _keywordManager.RemoveKeyword(_keywordListBox.SelectedItem.ToString()!);
        }
    }

    private void OnKeywordsChanged()
    {
        if (InvokeRequired)
        {
            Invoke(RefreshKeywordList);
        }
        else
        {
            RefreshKeywordList();
        }
    }

    private void RefreshKeywordList()
    {
        _keywordListBox.Items.Clear();
        foreach (string keyword in _keywordManager.Keywords)
        {
            _keywordListBox.Items.Add(keyword);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _keywordManager.KeywordsChanged -= OnKeywordsChanged;
        }
        base.Dispose(disposing);
    }
}
