using System;
using System.Collections.Generic;
using System.Text;

namespace Nemeio.Core.DataModels.Configurator
{
    public class DeadKey
    {
        public string Key { get; set; }

        public IList<Association> Associations { get; set; }
    }
}
