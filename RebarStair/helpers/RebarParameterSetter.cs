using Autodesk.Revit.DB;
using RebarStair.RebarStair.ViewModel;

namespace RebarStair.helpers
{
    public static class RebarParameterSetter
    {
        
        public static void SetRebarParameters(Autodesk.Revit.DB.Structure.Rebar rebar, Element host)
        {
            if (rebar == null || host == null) return;

           
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
        public static void SetPartition(
            Autodesk.Revit.DB.Structure.Rebar rebar,
            RebarTypeGroup typeGroup,
            int index)
        {
            if (rebar == null) return;

            if (!RebarPartitionManager.PartitionMap.ContainsKey(typeGroup))
                return;

            var list = RebarPartitionManager.PartitionMap[typeGroup];

            if (index < 0 || index >= list.Count)
                return;

            string partitionName = list[index];

            Parameter p = rebar.LookupParameter("Partition");

            if (p != null && !p.IsReadOnly)
            {
                p.Set(partitionName);
            }
        }
    }
}