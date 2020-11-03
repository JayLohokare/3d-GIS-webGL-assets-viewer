using System;

namespace OrchestrationService
{
    public class EventServiceMapView
    {
        public int Id { get; set; }
        public string ServiceId { get; set; }
        public string EventId { get; set; }
        public bool NotifyCompletion { get; set; }
        public string OriginationSystem { get; set; }
        public string Name { get; set; }
        public string ServiceBusTopicId { get; set; }
        public int ExecutionOrder { get; set; }
    }
}