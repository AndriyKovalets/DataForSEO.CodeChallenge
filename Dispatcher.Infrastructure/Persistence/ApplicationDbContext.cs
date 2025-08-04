using System.Diagnostics.CodeAnalysis;
using Dispatcher.Application.Abstractions.Persistence;
using Dispatcher.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Dispatcher.Infrastructure.Persistence;

public class ApplicationDbContext: DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
    {
    }
    
    public DbSet<TaskEntity>  Tasks { get; set; }
    
    public DbSet<SubTaskEntity>  SubTasks { get; set; }
    
    public IQueryable<TEntity> Resolve<TEntity>() where TEntity : class
    {
        return Set<TEntity>();
    }
    
    public EntityEntry<TEntity> Attach<TEntity>([NotNull] TEntity entity) where TEntity : class
    {
        return base.Attach(entity);
    }
    
    public async Task Save(CancellationToken cancellationToken = default)
    {
        await SaveChangesAsync(cancellationToken);

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskEntity>(entyty =>
        {
            entyty.HasKey(x => x.Id);
        });
        
        modelBuilder.Entity<SubTaskEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            
            entity.HasOne(x => x.Task)
                .WithMany(x  => x.SubTasks)
                .HasForeignKey(x => x.TaskId);
        });
        
        base.OnModelCreating(modelBuilder);
    }
}