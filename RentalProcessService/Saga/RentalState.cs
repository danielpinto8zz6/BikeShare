using Automatonymous;
using Common.Models.Dtos;
using MassTransit.Saga;

namespace RentalProcessorService.Saga
{
    public class RentalState : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }
        
        public RentalDto Rental { get; set; }      
        
        public DateTime Created { get; set; }
        
        public DateTime Updated { get; set; }
        
        public string State { get; set; }
        
        public int Version { get; set; }
    }
}