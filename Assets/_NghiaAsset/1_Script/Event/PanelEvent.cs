
namespace Turnbase
{
    public class HidePanelEvent : IEvent
    {
        public string panelName;

        public HidePanelEvent(string panelName)
        {
            this.panelName = panelName;
        }
    }

    public class ShowPanelEvent : IEvent
    {
        public string panelName;

        public ShowPanelEvent(string panelName)
        {
            this.panelName = panelName;
        }
    }


    public class OffUIAction : IEvent
    {
        public string panelName;
        public OffUIAction(string panelName)
        {
            this.panelName = panelName;
        }

    }

    public class OnUIAction : IEvent
    {
        public string panelName;
        public OnUIAction(string panelName)
        {
            this.panelName = panelName;
        }

    }

}
