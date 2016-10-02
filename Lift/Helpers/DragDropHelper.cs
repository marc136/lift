using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lift
{
    class DragDropHelper
    {
        public System.Windows.Point StartPosition { get; set; }

        public System.Windows.Controls.ListBoxItem ListBoxItem { get; set; }

        public Data.LiftItem Entry { get; set; }
    }
}
