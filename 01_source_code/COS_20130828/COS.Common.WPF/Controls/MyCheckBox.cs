using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace COS.Common.WPF.Controls
{
    public class CheckBoxDisabled : CheckBox
    {
        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = true;
        }
    }
}
