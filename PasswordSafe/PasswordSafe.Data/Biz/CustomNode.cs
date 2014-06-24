using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PasswordSafe.Data.Biz
{
    public class CustomNode:NodeBase
    {
        public CustomNode(string name, IEnumerable<NodeBase> children)
            : base(0, name)
        {
            this.Children = children;
        }       

    }
}
