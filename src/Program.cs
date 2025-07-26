using ewsense.storage;
using ewsense.keyboard;
using ewsense.ui;

namespace ewsense;

public class Program
{
    private static KeywordRepository? _keywordRepository;
    private static KeyboardListener? _keyboardListener;
    private static Popup? _popupManager;

    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        _keywordRepository = new KeywordRepository();
        _popupManager = new Popup();
        _keyboardListener = new KeyboardListener(_keywordRepository);
        _keyboardListener.KeywordDetected += () => _popupManager?.ShowKeywordDetectedPopup();
        _keyboardListener.StartListening();
        using (var mainForm = new MainForm(_keywordRepository))
        {
            Application.Run(mainForm);
        }
        _keyboardListener.Dispose();
    }
}
