using App.Cards;
using App.Dialogs.Merge;
using UnityEngine;

namespace App.Signals
{
    public struct MergeFilterChangedSignal
    {
        public ICardInfo CardInfo;
        public FilterType FilterType;
    }
}