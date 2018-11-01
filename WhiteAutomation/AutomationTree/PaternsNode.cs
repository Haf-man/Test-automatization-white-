using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AutomationTree
{
    public class PaternsNode:IAutomationNode
    {
        public ControlNode Parent
        {
            get;
            set;
        }
        public string Name
        {
            get; 
            set;
        }
        
        public PaternsNode(ControlNode parent, string name)
        {
            Parent = parent;
            Name = name; 
        }
        public XmlNode ToXmlNode()
        {
            throw new NotImplementedException();
        }
    }
}
