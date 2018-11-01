using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhiteAutomation
{
    public enum Modes
    {
        Usual,
        Special,
        SpecialWithRegularExpression
    };

    public class Pair<T1, T2>
    {
        public Pair(T1 property1, T2 property2)
        {
            this.Property1 = property1;
            this.Property2 = property2;
            Mode = Modes.Usual;
            TechPath = string.Empty;
        }
        public Pair(T1 property1, T2 property2, Modes mode):this(property1,property2)
        {
            
            Mode = mode;
        }

        public Pair(T1 property1, T2 property2, Modes mode, List<string> techPath) : this(property1, property2, mode)
        {
            PathFromList(techPath);
        }

       
        public T1 Property1 { get; set; }
        public T2 Property2 { get; set; }

        public Modes Mode { get; set; }

        public string TechPath { get; set; }

        public void PathFromList(List<string> path )
        {
            TechPath =  path.Aggregate("", (current, node) => current + ("/" + node));
        }

       
        
    };
}
