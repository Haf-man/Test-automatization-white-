using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLogger
{
    static public class Logger
    {
#region public
        static public void WriteToLog(string message)
        {

            using (var file = new System.IO.StreamWriter(@"./log.txt"))
            {
                file.WriteLine(DateTime.Now.ToString(CultureInfo.InvariantCulture) + " : " + message);
            }
        }

        static public void WriteToLogNormally(string message)
        {
            using (var file = new System.IO.StreamWriter(@"./log.txt",true))
            {
                file.Write(DateTime.Now.ToString(CultureInfo.InvariantCulture) + " : " + message);
            }
        }
#endregion 
#region private

#endregion
    }
}
