namespace SharedKernel.Domain.Others
{
    public class EventInformation
    {
        public EventInformation(object eventData, bool afterSave)
        {
            EventData = eventData;
            AfterSave = afterSave;
        }

        public object EventData { get; }

        public bool AfterSave { get; }
    }
}
