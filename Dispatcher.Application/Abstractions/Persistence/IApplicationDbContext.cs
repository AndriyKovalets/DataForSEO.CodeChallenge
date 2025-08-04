using System.Diagnostics.CodeAnalysis;
using Dispatcher.Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Dispatcher.Application.Abstractions.Persistence;

public interface IApplicationDbContext
{
    public IQueryable<TaskEntity> Tasks => Resolve<TaskEntity>();
    
    public IQueryable<SubTaskEntity> SubTasks => Resolve<SubTaskEntity>();
    
    public EntityEntry<TEntity> Attach<TEntity>([NotNull] TEntity entity) where TEntity : class;
    
    Task Save(CancellationToken cancellationToken = default);
    
    IQueryable<TEntity> Resolve<TEntity>() where TEntity : class;
}