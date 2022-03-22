using Common.Models.Dtos;
using Common.Models.Enums;
using MassTransit;

namespace RentalProcessService.Saga
{
    public class RentalState : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }
        
        public RentalDto Rental { get; set; }      
        
        public DateTime Created { get; set; }
        
        public DateTime Updated { get; set; }
        
        public int Status { get; set; }
        
        public int Version { get; set; }
    }
}