using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArvidsonFoto.Data
{
    public class SqlConnConfig
    {
        public SqlConnConfig(string config) => Config = config;

        public string Config { get; }
    }
}
