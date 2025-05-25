using Application.Common.Interfaces;
using Application.Common.Models; 
using Microsoft.EntityFrameworkCore.ChangeTracking; 
using System.Linq; 
using System.Threading; 
using System.Threading.Tasks;


namespace Infrastructure.Persistence
{
    internal sealed class UnitOfWork : IUnitOfWork
    {
        private readonly PlayBookDbContext _context;
        private readonly IDomainEventDispatcher _eventDispatcher;

        public UnitOfWork(
            PlayBookDbContext context, 
            IDomainEventDispatcher eventDispatcher)
        {
            _context = context;
            _eventDispatcher = eventDispatcher;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var aggregateRoots = _context.ChangeTracker
                .Entries<AggregateRoot>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents.Any())
                .ToList();

            // Extract domain events from tracked aggregates
            var domainEvents = aggregateRoots
                .SelectMany(ar => ar.DomainEvents)
                .ToList();

            int result = await _context.SaveChangesAsync(cancellationToken);

            await _eventDispatcher.DispatchEventsAsync(domainEvents);

            foreach (var aggregateRoot in aggregateRoots)
            {
                aggregateRoot.ClearDomainEvents();
            }

            return result;

        }

        public void Dispose()
        {
            _context.Dispose();
        }

    }
}
