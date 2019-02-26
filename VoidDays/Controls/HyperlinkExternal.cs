using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace VoidDays.Controls
{
    public class HyperlinkExternal : Hyperlink
    {
        public HyperlinkExternal(Run run) : base(run)
        {

        }
        protected override void OnClick()
        {
            base.OnClick();

            Process p = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = this.NavigateUri.AbsoluteUri
                }
            };
            p.Start();
        }
    }
}
