using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestClients.Formatting
{
    public abstract class ValueSource
    {
        public abstract string this[string name] { get; }
    }
}
