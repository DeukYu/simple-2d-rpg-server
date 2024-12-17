﻿using Microsoft.EntityFrameworkCore;

namespace Shared.DB;

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

    public static void SetModifiedProperties(this DbContext context, object entity, params string[] properties)
    {
        context.Entry(entity).State = EntityState.Unchanged;
        foreach (var property in properties)
        {
            context.Entry(entity).Property(property).IsModified = true;
        }
    }
}
