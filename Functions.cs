using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AppResizer
{
    public partial class Form1
    {
        
        public string GetParamFromLineINI(string line, string param_name)
        {
            Match m = Regex.Match(line, @"♿ " + param_name + @": ([^♿]*) ♿");
            if (!m.Success)
                return null;
            return m.Groups[1].Value;
        }
    }
}
