using System.Runtime.InteropServices;
using System.Text;

namespace ewsense.winapi;

public static class WindowsApi
{
    [StructLayout(LayoutKind.Sequential)]
    private struct Point
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct GuiThreadInfo
    {
        public uint cbSize;
        public uint flags;
        public IntPtr hwndActive;
        public IntPtr hwndFocus;
        public IntPtr hwndCapture;
        public IntPtr hwndMenuOwner;
        public IntPtr hwndMoveSize;
        public IntPtr hwndCaret;
        public Rect rcCaret;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern int ToUnicodeEx(
        uint wVirtKey,
        uint wScanCode,
        byte[] lpKeyState,
        [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff,
        int cchBuff,
        uint wFlags,
        IntPtr dwhkl
    );

    [DllImport("user32.dll")]
    private static extern bool GetKeyboardState(byte[] lpKeyState);

    [DllImport("user32.dll")]
    private static extern bool GetGUIThreadInfo(uint idThread, ref GuiThreadInfo lpgui);

    [DllImport("user32.dll")]
    private static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern IntPtr GetModuleHandle(string lpModuleName);

    public static string GetCharFromKey(int vkCode)
    {
        var keyState = new byte[256];
        GetKeyboardState(keyState);

        var sb = new StringBuilder(2);
        return ToUnicodeEx((uint)vkCode, 0, keyState, sb, sb.Capacity, 0, IntPtr.Zero) == 1
            ? sb.ToString()
            : "";
    }

    public static System.Drawing.Point GetCaretPosition()
    {
        var guiInfo = new GuiThreadInfo();
        guiInfo.cbSize = (uint)Marshal.SizeOf(guiInfo);

        if (GetGUIThreadInfo(0, ref guiInfo) && guiInfo.hwndCaret != IntPtr.Zero)
        {
            var caretPoint = new Point
            {
                X = guiInfo.rcCaret.Left,
                Y = guiInfo.rcCaret.Bottom
            };

            if (ClientToScreen(guiInfo.hwndCaret, ref caretPoint))
            {
                return new System.Drawing.Point(caretPoint.X, caretPoint.Y);
            }
        }

        var foregroundWindow = GetForegroundWindow();
        if (foregroundWindow == IntPtr.Zero)
            return new System.Drawing.Point(
                Screen.PrimaryScreen?.Bounds.Width - 220 ?? 800,
                Screen.PrimaryScreen?.Bounds.Height - 120 ?? 600
            );

        var mousePoint = Control.MousePosition;
        return mousePoint with { Y = mousePoint.Y + 20 };
    }
}
