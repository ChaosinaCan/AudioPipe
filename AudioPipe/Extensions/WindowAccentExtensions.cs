﻿using AudioPipe.Services;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace AudioPipe.Extensions
{
    static class WindowAccentExtensions
    {
        static class Interop
        {
            [DllImport("user32.dll")]
            internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttribData data);

            [StructLayout(LayoutKind.Sequential)]
            internal struct WindowCompositionAttribData
            {
                public WindowCompositionAttribute Attribute;
                public IntPtr Data;
                public int SizeOfData;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct AccentPolicy
            {
                public AccentState AccentState;
                public AccentFlags AccentFlags;
                public int GradientColor;
                public int AnimationId;
            }

            [Flags]
            internal enum AccentFlags
            {
                // ...
                DrawLeftBorder = 0x20,
                DrawTopBorder = 0x40,
                DrawRightBorder = 0x80,
                DrawBottomBorder = 0x100,
                DrawAllBorders = (DrawLeftBorder | DrawTopBorder | DrawRightBorder | DrawBottomBorder)
                // ...
            }

            internal enum WindowCompositionAttribute
            {
                // ...
                WCA_ACCENT_POLICY = 19
                // ...
            }

            internal enum AccentState
            {
                ACCENT_DISABLED = 0,
                ACCENT_ENABLE_GRADIENT = 1,
                ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
                ACCENT_ENABLE_BLURBEHIND = 3,
                ACCENT_INVALID_STATE = 4
            }
        }

        public static void SetBlur(this Window window, bool enabled)
        {
            Interop.AccentState state;

            // Blur is not useful in high contrast mode
            if (enabled && !SystemParameters.HighContrast)
            {
                state = Interop.AccentState.ACCENT_ENABLE_BLURBEHIND;
            }
            else
            {
                state = Interop.AccentState.ACCENT_DISABLED;
            }

            SetAccentPolicy(window, state, GetAccentFlagsForTaskbarPosition());
        }

        private static void SetAccentPolicy(Window window, Interop.AccentState accentState, Interop.AccentFlags accentFlags)
        {
            var windowHelper = new WindowInteropHelper(window);

            var accent = new Interop.AccentPolicy();
            accent.AccentState = accentState;
            accent.AccentFlags = accentFlags;

            var structSize = Marshal.SizeOf(accent);
            var structPtr = Marshal.AllocHGlobal(structSize);

            try
            {
                Marshal.StructureToPtr(accent, structPtr, false);

                var data = new Interop.WindowCompositionAttribData();
                data.Attribute = Interop.WindowCompositionAttribute.WCA_ACCENT_POLICY;
                data.SizeOfData = structSize;
                data.Data = structPtr;

                Interop.SetWindowCompositionAttribute(windowHelper.Handle, ref data);
            }
            finally
            {
                Marshal.FreeHGlobal(structPtr);
            }
        }

        private static Interop.AccentFlags GetAccentFlagsForTaskbarPosition()
        {
            var flags = Interop.AccentFlags.DrawAllBorders;

            switch (TaskbarService.GetTaskbarState().TaskbarPosition)
            {
                case TaskbarPosition.Top:
                    flags &= ~Interop.AccentFlags.DrawTopBorder;
                    break;

                case TaskbarPosition.Bottom:
                    flags &= ~Interop.AccentFlags.DrawBottomBorder;
                    break;

                case TaskbarPosition.Left:
                    flags &= ~Interop.AccentFlags.DrawLeftBorder;
                    break;

                case TaskbarPosition.Right:
                    flags &= ~Interop.AccentFlags.DrawRightBorder;
                    break;
            }

            return flags;
        }
    }
}
