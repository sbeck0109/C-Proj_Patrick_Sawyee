using Microsoft.EntityFrameworkCore;

namespace CsharpProject.Models
{
  public class ProjectContext : DbContext
  {
    public ProjectContext(DbContextOptions options) : base(options) { }

    // for every model / entity that is going to be part of the db
    // the names of these properties will be the names of the tables in the db
    public DbSet<User> Users { get; set; }
    public DbSet<Property> Properties { get; set; }
	public DbSet<Association> Associations { get; set; }
  }
}