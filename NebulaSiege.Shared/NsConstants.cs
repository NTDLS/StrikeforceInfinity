﻿namespace NebulaSiege.Shared
{
    public static class NsConstants
    {
        public static string FriendlyName = "NebulaSiege";

        public enum NsLogSeverity
        {
            Trace = 0, //Super-verbose, debug-like information.
            Verbose = 1, //General status messages.
            Warning = 2, //Something the user might want to be aware of.
            Exception = 3 //An actual exception has been thrown.
        }
    }
}
