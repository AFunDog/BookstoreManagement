using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using WinRT.Interop;
using 书店管理系统.Contracts;

namespace 书店管理系统.Services
{
    internal partial class DragMove : IDragMove
    {
        private sealed class PointerState
        {
            public required AppWindow Window { get; init; }
            public bool IsDragging { get; set; }
            public global::Windows.Graphics.PointInt32 WindowPos { get; set; }
            public global::System.Drawing.Point PointerPos { get; set; }
        }

        private readonly Dictionary<UIElement, PointerState> _elements = [];

        public void Register(UIElement element, AppWindow window)
        {
            if (_elements.TryAdd(element, new() { Window = window }))
            {
                element.PointerPressed += OnPointerPressed;
                element.PointerReleased += OnPointerReleased;
                element.PointerMoved += OnPointerMoved;
                window.Destroying += (_, _) =>
                {
                    Unregister(element);
                };
            }

        }

        public void Unregister(UIElement element)
        {
            if (_elements.Remove(element))
            {
                element.PointerPressed -= OnPointerPressed;
                element.PointerReleased -= OnPointerReleased;
                element.PointerMoved -= OnPointerMoved;
            }


        }

        private void OnPointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (sender is UIElement element && e.GetCurrentPoint(element).Properties.IsLeftButtonPressed)
            {
                if(_elements.TryGetValue(element,out var state))
                {
                    state.IsDragging = true;
                    state.WindowPos = state.Window.Position;
                    state.PointerPos = GetCursorPos(out var point) ? point: default;
                    element.CapturePointer(e.Pointer);
                    e.Handled = true;
                }
                
            }

        }

        private void OnPointerReleased(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (sender is UIElement element)
            {
                _elements[element].IsDragging = false;
                element.ReleasePointerCapture(e.Pointer);
                e.Handled = true;
            }
        }

        private void OnPointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (sender is UIElement element && _elements.TryGetValue(element,out var state) && state.IsDragging && GetCursorPos(out System.Drawing.Point pointerPos))
            {
                state.Window.Move(
                    new(
                        state.WindowPos.X + pointerPos.X - state.PointerPos.X,
                        state.WindowPos.Y + pointerPos.Y - state.PointerPos.Y
                    ));
            }
        }


        /// <summary>
        /// 获取鼠标相对于屏幕的位置
        /// </summary>
        /// <param name="lpPoint"></param>
        /// <returns></returns>
        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool GetCursorPos(out System.Drawing.Point lpPoint);

    }
}
