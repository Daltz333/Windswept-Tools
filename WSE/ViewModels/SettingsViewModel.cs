using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSE.ViewModels
{
    public class SettingsViewModel : PageViewModelBase
    {
        public static string Title => "Settings";

        public string Message => "This is the settings page!";

        public override bool CanNavigateNext
        {
            get => true;
            protected set => throw new NotSupportedException();
        }

        public override bool CanNavigatePrevious
        {
            get => false;
            protected set => throw new NotSupportedException();
        }
    }
}
