using Microsoft.EntityFrameworkCore;

namespace Database.Entity;

public interface IEntity
{
    static abstract void Configure(ModelBuilder modelBuilder);
}