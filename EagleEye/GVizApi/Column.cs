using System;
using System.Collections.Generic;

namespace EagleEye.GVizApi
{
    public class Column
    {
        private readonly List<string> types = new List<string>
        {
            "string",
            "number",
            "boolean",
            "date",
            "datetime",
            "timeofday"
        };

        private string type = null;
        private string label = null;

        public Column(string type, string label = "")
        {
            Type = type;
            Label = label;
        }

        public string Type
        {
            get
            {
                return type;
            }

            set
            {
                if (types.IndexOf(value) > -1)
                {
                    type = value;
                }
                else
                {
                    throw new Exception("Invalid column data type.");
                }
            }
        }

        public string Label
        {
            get
            {
                return label;
            }

            set
            {
                label = value;
            }
        }
    }
}
