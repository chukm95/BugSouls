using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace BugSouls.Util
{
    public delegate void OnResize(int width, int height);
    public delegate void OnFocusChange(bool isFocused);
    public delegate void OnCloseRequested();

    internal class Window
    {
        public int Width
        {
            get => nativeWindow.ClientSize.X;
        }

        public int Height
        {
            get => nativeWindow.ClientSize.Y;
        }

        public Vector2i Size
        {
            get => nativeWindow.ClientSize;
        }

        public float AspectRatio
        {
            get => (float)nativeWindow.ClientSize.X / (float)nativeWindow.ClientSize.Y;
        }

        public bool CursorGrabbed
        {
            get => nativeWindow.CursorGrabbed;
            set => nativeWindow.CursorGrabbed = value;
        }

        public bool CursorVisible
        {
            get => nativeWindow.CursorVisible;
            set => nativeWindow.CursorVisible = value;
        }

        public event OnResize OnResize;
        public event OnFocusChange OnFocusChange;
        public event OnCloseRequested OnCloseRequested;

        private NativeWindow nativeWindow;

        internal Window(NativeWindow nativeWindow)
        {
            this.nativeWindow = nativeWindow;
            nativeWindow.Resize += NativeWindow_Resize;
            nativeWindow.FocusedChanged += NativeWindow_FocusedChanged;
            nativeWindow.Closing += NativeWindow_Closing;
        }

        private void NativeWindow_Resize(OpenTK.Windowing.Common.ResizeEventArgs obj)
        {
            OnResize?.Invoke(obj.Width, obj.Height);
        }

        private void NativeWindow_FocusedChanged(OpenTK.Windowing.Common.FocusedChangedEventArgs obj)
        {
            OnFocusChange?.Invoke(obj.IsFocused);
        }

        private void NativeWindow_Closing(System.ComponentModel.CancelEventArgs obj)
        {
            obj.Cancel = true;
            OnCloseRequested?.Invoke();
        }
    }
}
