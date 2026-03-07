using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AgentStep
{
    public string Thought { get; set; }
    public string Action { get; set; }   // "GetDate" or "FinalAnswer"
    public string Input { get; set; }
}
