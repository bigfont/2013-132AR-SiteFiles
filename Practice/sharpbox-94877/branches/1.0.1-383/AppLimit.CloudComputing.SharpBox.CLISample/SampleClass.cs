using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace AppLimit.CloudComputing.SharpBox.CLISample
{
    [Serializable]
    public class SampleClass
    {
        public String value1;
        public String value2;

        public int iv1;
        public int iv2;
    }
}
