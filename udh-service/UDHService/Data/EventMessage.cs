using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Common.Data
{
    class EventMessage
    {
        public Guid? CorrelationId;
        public string CreatedBy;
        public string CreatedBySystem;
        public DateTime? CreatedTime;
        public string EventType;
        public string Message;
        public bool NotifyCompletion;

        public EventMessage(string eventMessage)
        {
            var objToken = JsonConvert.DeserializeObject<JToken>(eventMessage);
            CorrelationId = objToken["CorrelationId"] != null ? new Guid(objToken["CorrelationId"].ToString()) : Guid.NewGuid();
            CreatedBy = objToken["CreatedBy"].ToString();
            CreatedBySystem = objToken["CreatedBySystem"].ToString();
            CreatedTime = DateTime.Now;
            EventType = objToken["EventType"].ToString();
            NotifyCompletion = objToken["NotifyCompletion"] == null? false : objToken["NotifyCompletion"].Value<bool>();
            Message = objToken["Message"].ToString();
        }
    }
}
