using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolTipCommand;

namespace WpfApp
{
    public class TooltipWithDisableReason
    {
        public object OriginalTooltip { get; set; }

        public DisableReason DisableReason { get; set; }

        public string DisableReasonTip { get; set; }
    }
}
