using Autodesk.Revit.DB;

namespace RebarStair.helpers
{
    public static class RebarParameterSetter
    {
        /// <summary>
        /// Gán HostMark và Partition cho Rebar
        /// </summary>
        public static void SetRebarParameters(Autodesk.Revit.DB.Structure.Rebar rebar, Element host)
        {
            if (rebar == null || host == null) return;

            // Lấy HostMark từ host (Stair hoặc FamilyInstance...)
            string hostMark = host.get_Parameter(BuiltInParameter.ALL_MODEL_MARK)?.AsString();

            if (!string.IsNullOrEmpty(hostMark))
            {
               
                var markParam = rebar.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);
                markParam?.Set(hostMark);

             
                var partitionParam = rebar.get_Parameter(BuiltInParameter.NUMBER_PARTITION_PARAM);
                if (partitionParam != null && !partitionParam.IsReadOnly)
                {
                    partitionParam.Set(hostMark);
                }
            }
        }
    }
}