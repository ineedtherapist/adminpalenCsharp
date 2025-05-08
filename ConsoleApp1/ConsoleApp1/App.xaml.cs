using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace ConsoleApp1
{
    public partial class App : Application
    {
        public static bool IsFullscreenMode { get; set; } = true;
        
        public App()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }
        
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"Виникла непередбачена помилка: {e.Exception.Message}. Додаток буде закрито.", 
                "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            
            e.Handled = true;
            Current.Shutdown();
        }
        
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            StartupUri = new System.Uri("Views/LoginWindow.xaml", System.UriKind.Relative);
        }
        
        public static void SetFullscreenMode(Window window)
        {
            if (IsFullscreenMode)
            {
                window.WindowState = WindowState.Normal;
                window.WindowStyle = WindowStyle.None;
                window.ResizeMode = ResizeMode.NoResize;
                window.Left = 0;
                window.Top = 0;
                window.Width = SystemParameters.PrimaryScreenWidth;
                window.Height = SystemParameters.PrimaryScreenHeight;
                
                window.KeyDown -= Window_KeyDown;
                window.KeyDown += Window_KeyDown;
            }
        }
        
        public static void ToggleFullscreenMode(Window window)
        {
            IsFullscreenMode = !IsFullscreenMode;
            
            Button? toggleButton = window.FindName("ToggleFullscreenButton") as Button;
            
            if (IsFullscreenMode)
            {
                window.Tag = new Tuple<WindowState, double, double, double, double>(
                    window.WindowState, window.Left, window.Top, window.Width, window.Height);
                
                window.WindowState = WindowState.Normal;
                window.WindowStyle = WindowStyle.None;
                window.ResizeMode = ResizeMode.NoResize;
                window.Left = 0;
                window.Top = 0;
                window.Width = SystemParameters.PrimaryScreenWidth;
                window.Height = SystemParameters.PrimaryScreenHeight;
                
                if (toggleButton != null)
                {
                    toggleButton.Content = "Згорнути";
                }
            }
            else
            {
                if (window.Tag is Tuple<WindowState, double, double, double, double> windowProps)
                {
                    window.WindowState = windowProps.Item1;
                    window.Left = windowProps.Item2;
                    window.Top = windowProps.Item3;
                    window.Width = windowProps.Item4;
                    window.Height = windowProps.Item5;
                }
                
                window.WindowStyle = WindowStyle.SingleBorderWindow;
                window.ResizeMode = ResizeMode.CanResize;
                
                if (toggleButton != null)
                {
                    toggleButton.Content = "На весь екран";
                }
            }
        }
        
        private static void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender is not Window window) return;
            
            if (e.Key == Key.Escape || (e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt))
            {
                ToggleFullscreenMode(window);
                e.Handled = true;
            }
        }
    }
}
