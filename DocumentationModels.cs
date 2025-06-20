using System;
using System.Collections.Generic;

namespace Сетевое_программирование
{
    public class MethodInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }

    public class DocumentationParseResult
    {
        public List<MethodInfo> Methods { get; set; } = new List<MethodInfo>();
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
} 