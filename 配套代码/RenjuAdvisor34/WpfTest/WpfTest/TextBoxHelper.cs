using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfTest
{
    public class TextBoxHelper
    {
        public static bool GetIsOnlyNumber(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsOnlyNumberProperty);
        }

        public static void SetIsOnlyNumber(DependencyObject obj, bool value)
        {
            obj.SetValue(IsOnlyNumberProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsOnlyNumber.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOnlyNumberProperty =
            DependencyProperty.RegisterAttached("IsOnlyNumber", typeof(bool), typeof(TextBoxHelper), new PropertyMetadata(false, OnIsOnlyNumberChanged));

        private static void OnIsOnlyNumberChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox textBox = d as TextBox;
            if ((bool)e.NewValue)
            {
                textBox.PreviewTextInput += TextBox_PreviewTextInput;
            }
            else
            {
                textBox.PreviewTextInput -= TextBox_PreviewTextInput;
            }
        }

        private static void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (int.TryParse(e.Text, out int resutl))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
