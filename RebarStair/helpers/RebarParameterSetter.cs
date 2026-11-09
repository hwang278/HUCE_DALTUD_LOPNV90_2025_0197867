using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using RebarStair.RebarStair.Model;


namespace RebarStair.helpers
{
    public static class RebarParameterSetter
    {
        public static void SetRebarParameters(
            Element host,
            RebarTypeGroup typeGroup,
            int startIndex,
            params Autodesk.Revit.DB.Structure.Rebar[] rebars)
        {
            ArgumentNullException.ThrowIfNull(host);
            ArgumentNullException.ThrowIfNull(rebars);

            var partitionCount = RebarPartitionManager.GetPartitionCount(typeGroup);
            if (startIndex < 0 || startIndex + rebars.Length > partitionCount)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            var uniqueIds = new HashSet<ElementId>();
            var hostMark = host.get_Parameter(BuiltInParameter.ALL_MODEL_MARK)?.AsString();

            for (var index = 0; index < rebars.Length; index++)
            {
                var rebar = rebars[index]
                    ?? throw new ArgumentException("Rebar collection cannot contain null values.", nameof(rebars));

                if (!uniqueIds.Add(rebar.Id))
                    throw new ArgumentException("Each partition must target a different rebar.", nameof(rebars));

                if (!string.IsNullOrEmpty(hostMark))
                    rebar.get_Parameter(BuiltInParameter.ALL_MODEL_MARK)?.Set(hostMark);

                var partition = rebar.get_Parameter(BuiltInParameter.NUMBER_PARTITION_PARAM);
                if (partition != null && !partition.IsReadOnly)
                {
                    partition.Set(RebarPartitionManager.GetPartitionName(typeGroup, startIndex + index));
                }
            }
        }
    }
}
