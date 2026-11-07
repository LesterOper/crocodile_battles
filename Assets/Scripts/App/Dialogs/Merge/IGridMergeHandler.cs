using App.DAO;

namespace App.Dialogs.Merge
{
    public interface IGridMergeHandler
    {
        void Setup(FilterType filterType, IDataService data, MergePlace left, MergePlace right);
        void UpdateGrid();
    }
}