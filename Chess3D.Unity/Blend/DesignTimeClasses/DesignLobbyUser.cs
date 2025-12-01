using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Blend
{
    internal class DesignLobbyUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Image Image { get; set; }
        public bool IsReady { get; set; }
    }
}
