using System;
using System.Collections.Generic;

namespace lab
{
    public class BuildNormalFormEventArgs : EventArgs
    {
        public List<string> ListItems { get; }

        public BuildNormalFormEventArgs(List<string> listItems)
        {
            ListItems = listItems;
        }
    }
}
