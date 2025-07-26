using System.Runtime.InteropServices;
using System.Text;

namespace ewsense.winapi;

public static class WindowsApi
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GuiThreadInfo
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
    public struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState,
        [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);

    [DllImport("user32.dll")]
    public static extern bool GetKeyboardState(byte[] lpKeyState);

    [DllImport("user32.dll")]
    public static extern bool GetGUIThreadInfo(uint idThread, ref GuiThreadInfo lpgui);

    [DllImport("user32.dll")]
    public static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr GetModuleHandle(string lpModuleName);

    public static string GetCharFromKey(int vkCode)
    {
        byte[] keyState = new byte[256];
        GetKeyboardState(keyState);

        StringBuilder sb = new StringBuilder(2);
        if (ToUnicodeEx((uint)vkCode, 0, keyState, sb, sb.Capacity, 0, IntPtr.Zero) == 1)
        {
            return sb.ToString();
        }
        return "";
    }

    public static System.Drawing.Point GetCaretPosition()
    {
        GuiThreadInfo guiInfo = new GuiThreadInfo();
        guiInfo.cbSize = (uint)Marshal.SizeOf(guiInfo);

        if (GetGUIThreadInfo(0, ref guiInfo) && guiInfo.hwndCaret != IntPtr.Zero)
        {
            Point caretPoint = new Point
            {
                X = guiInfo.rcCaret.Left,
                Y = guiInfo.rcCaret.Bottom
            };

            if (ClientToScreen(guiInfo.hwndCaret, ref caretPoint))
            {
                return new System.Drawing.Point(caretPoint.X, caretPoint.Y);
            }
        }

        IntPtr foregroundWindow = GetForegroundWindow();
        if (foregroundWindow != IntPtr.Zero)
        {
            System.Drawing.Point mousePoint = Control.MousePosition;
            return new System.Drawing.Point(mousePoint.X, mousePoint.Y + 20);
        }

        return new System.Drawing.Point(Screen.PrimaryScreen?.Bounds.Width - 220 ?? 800, Screen.PrimaryScreen?.Bounds.Height - 120 ?? 600);
    }
}
