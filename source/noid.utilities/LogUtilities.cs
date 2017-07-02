

using System;
using System.Diagnostics;

namespace NoID.Utilities
{
    public static class LogUtilities
    {

        public static bool LogEvent(string message)
        {
            bool resultLogEvent = false;
            try
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry(message, EventLogEntryType.Information, 101, 1);
                }
                resultLogEvent = true;
            }
            catch { }
            return resultLogEvent;
        }
    }
}
