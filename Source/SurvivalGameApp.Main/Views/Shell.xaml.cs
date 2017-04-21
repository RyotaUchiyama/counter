﻿using Microsoft.Practices.ServiceLocation;
using SurvivalGameApp.Main.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SurvivalGameApp.Main.Views
{
    /// <summary>
    /// Shell.xaml の相互作用ロジック
    /// </summary>
    public partial class Shell : Window
    {
        const int GWL_STYLE = -16;
        const int WS_SYSMENU = 0x00080000;
        const int CYCAPTION = 0x04;
        const int CXFRAME = 0x20;
        const int CYFRAME = 0x21;
        const int CXPADDEDBORDER = 92;

        //public static readonly DependencyProperty BorderWidthProperty = DependencyProperty.Register("BorderWidth", typeof(int), typeof(Window), new FrameworkPropertyMetadata(0));
        //public static readonly DependencyProperty BorderHeightProperty = DependencyProperty.Register("BorderHeight", typeof(int), typeof(Window), new FrameworkPropertyMetadata(0));
        //public static readonly DependencyProperty CaptionHeightProperty = DependencyProperty.Register("CaptionHeight", typeof(int), typeof(Window), new FrameworkPropertyMetadata(0));
        //public static readonly DependencyProperty PaddingThcknessProperty = DependencyProperty.Register("PaddingThckness", typeof(int), typeof(Shell), new FrameworkPropertyMetadata(0));

        //public int BorderWidth { set => SetValue(BorderWidthProperty, value); get => (int)GetValue(BorderWidthProperty); }
        //public int BorderHeight { set => SetValue(BorderHeightProperty, value); get => (int) GetValue(BorderHeightProperty); }
        //public int CaptionHeight { set => SetValue(CaptionHeightProperty, value); get => (int) GetValue(CaptionHeightProperty); }
        //public int PaddingThckness { set => SetValue(PaddingThcknessProperty, value); get => (int)GetValue(PaddingThcknessProperty); }

        public Shell()
        {
            InitializeComponent();

            this.Height = SystemParameters.PrimaryScreenHeight;
            this.Width = SystemParameters.PrimaryScreenWidth;
            this.Top = 0d;
            this.Left = 0d;

            string debugText = string.Empty;

            DebugTextBlock.Inlines.Add("Height ");
            DebugTextBlock.Inlines.Add(this.Height.ToString());
            DebugTextBlock.Inlines.Add(new LineBreak());
            DebugTextBlock.Inlines.Add("Width ");
            DebugTextBlock.Inlines.Add(this.Width.ToString());
            DebugTextBlock.Inlines.Add(new LineBreak());
            this.Closed += Shell_Closed;
        }

        private void Shell_Closed(object sender, EventArgs e)
        {
            //ApplicationContext a = ServiceLocator.Current.GetInstance<ApplicationContext>();
            //ApplicationContext.SaveSetting(a, @"config/applicationConfing.json");
        }

        private void BindingSetting()
        {
            //SetBinding(TitleProperty, "Title");
            //SetBinding(WidthProperty, "Width");
            //SetBinding(HeightProperty, "Height");
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            IntPtr handle = (new WindowInteropHelper(this)).Handle;
            var style = GetWindowLong(handle, GWL_STYLE);
            style &= (~WS_SYSMENU);
            SetWindowLong(handle, GWL_STYLE, style);
        }

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        // Blur
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            EnableBlur();
        }

        internal void EnableBlur()
        {
            var windowHelper = new WindowInteropHelper(this);

            var accent = new AccentPolicy();
            accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;

            var accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData();
            data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentPtr;

            SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }
    }
}

internal enum AccentState
{
    ACCENT_DISABLED = 1,
    ACCENT_ENABLE_GRADIENT = 0,
    ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
    ACCENT_ENABLE_BLURBEHIND = 3,
    ACCENT_INVALID_STATE = 4
}

[StructLayout(LayoutKind.Sequential)]
internal struct AccentPolicy
{
    public AccentState AccentState;
    public int AccentFlags;
    public int GradientColor;
    public int AnimationId;
}

[StructLayout(LayoutKind.Sequential)]
internal struct WindowCompositionAttributeData
{
    public WindowCompositionAttribute Attribute;
    public IntPtr Data;
    public int SizeOfData;
}

internal enum WindowCompositionAttribute
{
    // ...
    WCA_ACCENT_POLICY = 19
    // ...
}
