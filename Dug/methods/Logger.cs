// Dug is a DNS lookup tool
// Copyright(C) 2024  Richard Cole
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.


// Simple placeholder for the logging
// For now we will simply output to console
// But this allows for full log files when run with API

using dug.enums;
using dug.models;

namespace dug.methods
{
    public class Logger
    {
        private readonly string ApplicationName = string.Empty;
        private readonly string ClassName = string.Empty;
        private readonly bool Verbose = false;
        public Logger() { }
        public Logger(string application, string className, bool verbose)
        {
            ApplicationName = application;
            ClassName = className;
            Verbose = verbose;
        }
        public async void Log(LogModel log, LoggerOptions loggerOptions = LoggerOptions.Default, bool MessageOnly=false)
        {
            log.Application = ApplicationName;
            log.ClassName = ClassName;

            // Prepare Message
            string logMessage = ($"{log.DateTime.ToString("u")},{ApplicationName}, {ClassName}, {log.SubSystem}, {log.Level},{log.Message}");

            if (loggerOptions.HasFlag(LoggerOptions.ForceConsole) || (Verbose && (loggerOptions.HasFlag(LoggerOptions.ConsoleOnly) || loggerOptions.HasFlag(LoggerOptions.Default))))
            {
                if (MessageOnly)
                {
                    await Console.Out.WriteLineAsync(log.Message);
                }
                else
                {
                    await Console.Out.WriteLineAsync(logMessage);
                }  
            }

        }
    }
}
