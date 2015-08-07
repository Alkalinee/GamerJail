using System.Collections.ObjectModel;
using System.Linq;

namespace PieControls
{
    public static class PieCollectionHelper
    {
        public static double GetTotal(this ObservableCollection<PieSegment> collection)
        {
            return collection.Sum(a => a.Value);
        }
    }
}
