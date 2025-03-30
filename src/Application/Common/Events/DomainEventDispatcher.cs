using Application.Common.Interfaces;
using MediatR;

namespace Application.Common.Events
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IPublisher _mediator;

        public DomainEventDispatcher(IPublisher mediator)
        {
            _mediator = mediator;
        }

        public async Task DispatchEventsAsync(IEnumerable<IDomainEvent> domainEvents)
        {
            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent);
            }
        }

    }
}
