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

using dug.enums;

namespace dug.models
{
    public class LogModel
    {
        public DateTime DateTime { get; set; }
        public string Application {  get; set; }
        public string ClassName { get; set; }
        public string SubSystem { get; set; }
        public SeverityLevel Level { get; set; }
        public string Message { get; set; }
        
    
        public LogModel (string subSystem, SeverityLevel level, string message)
        {
            DateTime = DateTime.UtcNow;
            Application = string.Empty;
            ClassName = string.Empty;
            SubSystem = subSystem;
            Level = level;
            Message = message;
        }

        public LogModel(string application, string subSystem, SeverityLevel level, string message)
        {
            DateTime = DateTime.UtcNow;
            Application = application;
            ClassName = string.Empty;
            SubSystem = subSystem;
            Level = level;
            Message = message;
        }
        public LogModel(string application,string className ,string subSystem, SeverityLevel level, string message)
        {
            DateTime = DateTime.UtcNow;
            Application = application;
            ClassName = className;
            SubSystem = subSystem;
            Level = level;
            Message = message;
        }
    }
    
}
