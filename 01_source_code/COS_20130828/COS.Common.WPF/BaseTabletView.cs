using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.ComponentModel;
using COS.Application.Shared;
using Telerik.Windows.Controls;

namespace COS.Common.WPF
{
    public class BaseTabletView : BaseUserControl
    {
        public BaseTabletView()
            : base()
        {
            OKButtonVisibility = true;
            CancelButtonVisibility = true;
        }

        public string Code { set; get; }

        public bool IsModal { set; get; }

        public TabletViewModelBase Model { set; get; }

        private List<RadButton> _commandButtons = new List<RadButton>();
        public List<RadButton> CommandButtons { get { return _commandButtons; } }

        public bool OKButtonVisibility { set; get; }
        public bool CancelButtonVisibility { set; get; }
    }
}
