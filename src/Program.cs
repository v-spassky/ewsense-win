using ewsense.storage;
using ewsense.keyboard;
using ewsense.ui;

namespace ewsense;

public class Program
{
    private static KeywordRepository? _keywordRepository;
    private static KeyboardListener? _keyboardListener;

    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        _keywordRepository = new KeywordRepository();
        _keyboardListener = new KeyboardListener(_keywordRepository);
        _keyboardListener.KeywordDetected += Popup.ShowKeywordDetectedPopup;
        _keyboardListener.StartListening();
        using var mainForm = new MainForm(_keywordRepository);
        Application.Run(mainForm);
        _keyboardListener.Dispose();
    }
}
