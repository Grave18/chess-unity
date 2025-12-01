using System.Collections.Generic;
using System.ComponentModel;

namespace Blend.DesignTimeViewModels
{
    class VsPlayerPageViewModelDesign
    {
        public string SelectedItem { get; set; }
        public List<string> Items;

        public VsPlayerPageViewModelDesign()
        {
            Items = new List<string>{ "1", "2", "3" };
        }
    }
}
