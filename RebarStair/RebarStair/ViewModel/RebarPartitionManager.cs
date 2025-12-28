using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RebarStair.RebarStair.ViewModel
{

    public static class RebarPartitionManager
    {
        public static Dictionary<RebarTypeGroup, List<string>> PartitionMap
            = new Dictionary<RebarTypeGroup, List<string>>
            {
                {
                    RebarTypeGroup.Type1,
                    new List<string> { "type1_1", "type1_2", "type1_3", "type1_4", "type1_5", "type1_6" }
                },
                {
                    RebarTypeGroup.Type2,
                    new List<string> { "type2_1", "type2_2", "type2_3" , "type2_4", "type2_5" }
                },
                {
                    RebarTypeGroup.Type3,
                    new List<string> { "type3_1", "type3_2" , "type3_3" , "type3_4" , "type3_5" , "type3_6" , "type3_7" , "type3_8" , "type3_9" , "type3_10" , "type3_11" }
                },
                {
                    RebarTypeGroup.CN,
                    new List<string> { "CN_1", "CN_2"  }
                }
            };
    }

}
