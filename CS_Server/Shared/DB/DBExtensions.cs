using Microsoft.EntityFrameworkCore;

namespace Shared;

public static class DBExtensions
{
    public static bool SaveChangesEx(this DbContext context)
    {
        try
        {
            context.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
