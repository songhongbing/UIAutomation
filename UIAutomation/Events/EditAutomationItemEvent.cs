using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;
using UIAutomation.Models;

namespace UIAutomation.Events
{
    public class EditAutomationItemEvent:PubSubEvent<Dictionary<string,object>>
    {
    }
}
