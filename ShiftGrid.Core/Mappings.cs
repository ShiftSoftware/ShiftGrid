﻿using System.Collections.Generic;

namespace ShiftSoftware.ShiftGrid.Core
{
    public class Mappings
    {
        public static Dictionary<string, string> OperatorMapping = new Dictionary<string, string>
        {
            { GridFilterOperator.Equals, "=" },
            { GridFilterOperator.NotEquals, "!=" },
            { GridFilterOperator.GreaterThan, ">" },
            { GridFilterOperator.GreaterThanOrEquals, ">=" },
            { GridFilterOperator.LessThan, "<" },
            { GridFilterOperator.LessThanOrEquals, "<=" },
            { GridFilterOperator.Contains, ".Contains" },
            { GridFilterOperator.In, ".Contains" },
            { GridFilterOperator.NotIn, ".Contains" },
            { GridFilterOperator.StartsWith, ".StartsWith" },
            { GridFilterOperator.EndsWith, ".EndsWith" },
        };

        public static Dictionary<string, string> OperatorValuePrefix = new Dictionary<string, string>
        {
            { GridFilterOperator.Contains, "(" },
            { GridFilterOperator.In, "(" },
            { GridFilterOperator.NotIn, "(" },
            { GridFilterOperator.StartsWith, "(" },
            { GridFilterOperator.EndsWith, "(" },
        };

        public static Dictionary<string, string> OperatorValuePostfix = new Dictionary<string, string>
        {
            { GridFilterOperator.Contains, ")" },
            { GridFilterOperator.In, ")" },
            { GridFilterOperator.NotIn, ")" },
            { GridFilterOperator.StartsWith, ")" },
            { GridFilterOperator.EndsWith, ")" },
        };

        public static Dictionary<string, string> ExpressionPrefix = new Dictionary<string, string>
        {
            { GridFilterOperator.NotIn, "!(" },
        };

        public static Dictionary<string, string> ExpressionPostfix = new Dictionary<string, string>
        {
            { GridFilterOperator.NotIn, ")" },
        };
    }
}
