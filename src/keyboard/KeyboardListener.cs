using System.Diagnostics;
using System.Runtime.InteropServices;
using ewsense.storage;
using ewsense.winapi;

namespace ewsense.keyboard;

public class KeyboardListener : IDisposable
{
    private const int WhKeyboardLl = 13;
    private const int WmKeydown = 0x0100;

    private readonly WindowsApi.LowLevelKeyboardProc _proc;
    private IntPtr _hookId = IntPtr.Zero;
    private string _typedText = "";
    private readonly KeywordRepository _keywordRepository;
    private bool _disposed;

    public event Action? KeywordDetected;

    public KeyboardListener(KeywordRepository keywordRepository)
    {
        _keywordRepository = keywordRepository ?? throw new ArgumentNullException(nameof(keywordRepository));
        _proc = HookCallback;
    }

    public void StartListening()
    {
        if (_hookId == IntPtr.Zero)
        {
            _hookId = SetHook(_proc);
        }
    }

    private void StopListening()
    {
        if (_hookId == IntPtr.Zero)
            return;

        WindowsApi.UnhookWindowsHookEx(_hookId);
        _hookId = IntPtr.Zero;
    }

    private static IntPtr SetHook(WindowsApi.LowLevelKeyboardProc proc)
    {
        using var curProcess = Process.GetCurrentProcess();
        using var curModule = curProcess.MainModule;
        return WindowsApi.SetWindowsHookEx(
            WhKeyboardLl,
            proc,
            WindowsApi.GetModuleHandle(curModule?.ModuleName ?? ""),
            0
        );
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode < 0 || wParam != WmKeydown)
            return WindowsApi.CallNextHookEx(_hookId, nCode, wParam, lParam);

        var vkCode = Marshal.ReadInt32(lParam);
        var keyChar = WindowsApi.GetCharFromKey(vkCode);

        if (keyChar.Length == 1 && (char.IsLetterOrDigit(keyChar[0]) || keyChar[0] == ' '))
        {
            _typedText += keyChar;

            if (!_keywordRepository.HasKeywordEndingWith(_typedText))
                return WindowsApi.CallNextHookEx(_hookId, nCode, wParam, lParam);

            KeywordDetected?.Invoke();
            _typedText = "";
        }
        else if (vkCode == 8 && _typedText.Length > 0) // Backspace
        {
            _typedText = _typedText[..^1];
        }

        return WindowsApi.CallNextHookEx(_hookId, nCode, wParam, lParam);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            StopListening();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }

    ~KeyboardListener()
    {
        Dispose();
    }
}
