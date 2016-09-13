using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestClients.Formatting
{
    class ConfigurationManagerSource : ValueSource
    {
        public override string this[string name] => 
            ConfigurationManager.AppSettings.AllKeys.Contains(name) ?
                ConfigurationManager.AppSettings[name] : null;       
    }
}
