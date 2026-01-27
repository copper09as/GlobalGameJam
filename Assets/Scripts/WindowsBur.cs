using System;
using System.Runtime.InteropServices;
using UnityEngine;

public static class WindowsBlur
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("dwmapi.dll")]
    private static extern int DwmEnableBlurBehindWindow(
        IntPtr hWnd,
        ref DWM_BLURBEHIND blurBehind
    );

    private struct DWM_BLURBEHIND
    {
        public uint dwFlags;
        public bool fEnable;
        public IntPtr hRgnBlur;
        public bool fTransitionOnMaximized;
    }

    private const uint DWM_BB_ENABLE = 0x00000001;

    public static void EnableBlur()
    {
        IntPtr hwnd = GetActiveWindow();

        DWM_BLURBEHIND blur = new DWM_BLURBEHIND
        {
            dwFlags = DWM_BB_ENABLE,
            fEnable = true,
            hRgnBlur = IntPtr.Zero,
            fTransitionOnMaximized = false
        };

        DwmEnableBlurBehindWindow(hwnd, ref blur);
    }
}
