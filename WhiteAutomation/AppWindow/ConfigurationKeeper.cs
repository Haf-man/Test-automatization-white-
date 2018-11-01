using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWindow
{
    class ConfigurationKeeper
    {
        public List<ConfigurationReader.Configuration> configurations { get; set; }
        

        

    }

    private class ConfigurationWithName :ConfigurationReader.Configuration
    {
        public string Name { get; set; }

    }
}
