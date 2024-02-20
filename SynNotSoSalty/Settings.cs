using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynNotSoSalty
{
    public class Settings
    {
        public bool EnableMagicSalt { get; set; } = false;
        public bool MagicSaltRequiresRegularSalt = true;
    }
}
