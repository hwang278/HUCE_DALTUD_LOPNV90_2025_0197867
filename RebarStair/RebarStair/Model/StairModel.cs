using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RebarStair.RebarStair.Model
{
    public class StairModel
    {
        public XYZ P1 { get; set; }
        public XYZ P2 { get; set; }
        public XYZ P3 { get; set; }
        public XYZ P4 { get; set; }
        public XYZ P5 { get; set; }
        public XYZ P6 { get; set; }
        public XYZ P7 { get; set; }
        public XYZ P8 { get; set; }
        public XYZ P9 { get; set; }
        public XYZ P10 { get; set; }
        public XYZ P11 { get; set; }
        public XYZ P12 { get; set; }

        public Element Stair { get; set; }
        //them list id thang 
        public StairModel(Element stair)
        {
            Stair = stair;
        }
    }
}
