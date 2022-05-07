using System;
using Common.Models.Dtos;
using MassTransit;

namespace PaymentService.Saga
{
    public class PaymentState : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }
        
        public PaymentDto Payment { get; set; }      
        
        public DateTime Created { get; set; }
        
        public DateTime Updated { get; set; }
        
        public int Status { get; set; }
        
        public int Version { get; set; }
    }
}