using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace Lca.EProcurement.EntityFrameworkCore.Migrations;

[DbContext(typeof(EProcurementDbContext))]
partial class EProcurementDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "10.0.0");
    }
}
