using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace UIAutomation.Events
{
    public class ChangeRowOrderEvent: PubSubEvent<Dictionary<string,int>>
    {
    }
}
