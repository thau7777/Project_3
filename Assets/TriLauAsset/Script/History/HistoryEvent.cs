using UnityEngine;

namespace MyRule.Event
{
    public struct UpdateHistoryEvent :IEvent
    {
        public readonly HistoryData HistoryData;

        public UpdateHistoryEvent(HistoryData historyData)
        {
            this.HistoryData = historyData;
        }
    }
}