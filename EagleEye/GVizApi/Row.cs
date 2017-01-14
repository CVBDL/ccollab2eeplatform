using System.Collections.Generic;

namespace EagleEye.GVizApi
{
    public class Row
    {
        private List<object> cells = new List<object>();

        public Row(params object[] values)
        {
            foreach (var val in values)
            {
                cells.Add(val);
            }
        }

        public List<object> Cells
        {
            get
            {
                return cells;
            }
        }
    }
}
