using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public class CommonlyUsedVarVisitor : AutoVisitor
    {
        private Dictionary<string, int> VarUsesCount = new Dictionary<string, int>();
        public string mostCommonlyUsedVar()
        {
            KeyValuePair<string, int> result = new KeyValuePair<string, int>("temp", -1);
            foreach (var tuple in VarUsesCount)
                if (tuple.Value > result.Value)
                    result = tuple;
            return result.Key;
        }

        public override void VisitIdNode(IdNode id)
        {
            if (VarUsesCount.ContainsKey(id.Name))
                VarUsesCount[id.Name]++;
            else
                VarUsesCount[id.Name] = 1;
        }
    }
}
