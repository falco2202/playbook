using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Application.Common.Models;

namespace Infrastructure.Persistence
{
    internal sealed class UnitOfWork : IUnitOfWork
    {
        private readonly PlayBookDbContext _context;
        private readonly IDomainEventDispatcher _eventDispatcher;


        private readonly UserManager<ApplicationUser> _userManager;
            
        public UnitOfWork(
            PlayBookDbContext context, 
            IDomainEventDispatcher eventDispatcher, 
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _eventDispatcher = eventDispatcher;
            _userManager = userManager;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Extract domain events from tracked aggregates
            var domainEvents = _context.ChangeTracker
                .Entries<AggregateRoot>()
                .SelectMany(e => e.Entity.DomainEvents)
                .ToList();

            // Save changes to database first
            int result = await _context.SaveChangesAsync(cancellationToken);

            // Dispatch domain events after successful save
            await _eventDispatcher.DispatchEventsAsync(domainEvents);

            return result;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

    }
}
