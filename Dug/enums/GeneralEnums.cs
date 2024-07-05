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

namespace dug.enums
{
    /// <summary>
    /// Syslog Levels
    /// </summary>
    public enum SeverityLevel : int
    {
        /// <summary>
        /// System is unusable.
        /// A panic condition.
        /// </summary>
        Emergency = 0,
        /// <summary>
        /// Action must be taken immediately.
        /// A condition that should be corrected immediately.
        /// </summary>
        Alert = 1,
        /// <summary>
        /// Critical conditions.
        /// </summary>
        Critical = 2,
        /// <summary>
        /// Error conditions.
        /// </summary>
        Error = 3,
        /// <summary>
        /// Warning conditions.
        /// </summary>
        Warning = 4,
        /// <summary>
        /// Normal but significant conditions.
        /// Conditions that are not error conditions, but that may require special handling.
        /// </summary>
        Notice = 5,
        /// <summary>
        /// Informational messages.
        /// Confirmation that the program is working as expected.
        /// </summary>
        Info = 6,
        /// <summary>
        /// Debug-level messages.
        /// Messages that contain information normally of use only when debugging a program.
        /// </summary>
        Debug = 7
    }

    [Flags]
    public enum LoggerOptions : int
    {
        Verbose = 1,
        Default = 2,
        ConsoleOnly = 4,
        LogOnly = 8,
        ForceConsole = 16,
        ForceLog = 32
    }
}
