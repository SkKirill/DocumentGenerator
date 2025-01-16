using DocumentGenerator.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DocumentGenerator.Data.Services.DataBase.Repositories;

public class LayoutRepository : BaseModelRepository<Layout>
{
    protected override DbSet<Layout> Table => DatabaseContext.Layouts;
    
    public async Task<Layout[]> GetLayoutsAsync()
    {
        return await DatabaseContext.Layouts.ToArrayAsync();
    }
}