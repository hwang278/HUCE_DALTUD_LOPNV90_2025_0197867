namespace RebarStair.RebarStair.Model
{
    public static class RebarPartitionManager
    {
        public static int GetPartitionCount(RebarTypeGroup typeGroup)
        {
            return typeGroup switch
            {
                RebarTypeGroup.Type1 => 6,
                RebarTypeGroup.Type2 => 5,
                RebarTypeGroup.Type3 => 11,
                RebarTypeGroup.CN => 2,
                _ => throw new ArgumentOutOfRangeException(nameof(typeGroup), typeGroup, null)
            };
        }

        public static string GetPartitionName(RebarTypeGroup typeGroup, int index)
        {
            if (index < 0 || index >= GetPartitionCount(typeGroup))
                throw new ArgumentOutOfRangeException(nameof(index), index, null);

            var prefix = typeGroup == RebarTypeGroup.CN
                ? "CN"
                : typeGroup.ToString().ToLowerInvariant();

            return $"{prefix}_{index + 1}";
        }
    }
}
