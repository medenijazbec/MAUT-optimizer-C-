using System.Collections.Generic;

namespace MAUTmethod
{
    public class Alternative
    {
        public string Name { get; set; }
        public Dictionary<string, double> ParameterValues { get; set; }

        public Alternative()
        {
            ParameterValues = new Dictionary<string, double>();
        }
    }

}