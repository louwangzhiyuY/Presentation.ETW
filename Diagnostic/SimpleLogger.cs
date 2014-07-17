using System;
using Microsoft.Diagnostics.Tracing;

namespace Diagnostic
{
    [EventSource(Name = "ETWPresentation-Product-Component")]
    public sealed class SimpleLogger : EventSource
    {
        public static SimpleLogger Log = new SimpleLogger();

        [Event(1, Message = "User input - {0}", Level = EventLevel.Informational)]
        public void UserInput(String key)
        {
            if (IsEnabled())
            {
                WriteEvent(1, key);
            }
        }

        public void Start()
        {
            if (IsEnabled())
            {
                WriteEvent(2);
            }
        }

        public void Stop()
        {
            if (IsEnabled())
            {
                WriteEvent(3);
            }
        }
    }
}