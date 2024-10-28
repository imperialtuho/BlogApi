using Microsoft.EntityFrameworkCore;

namespace Blog.Domain.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> IncludeAllNavigations<T>(this IQueryable<T> query, DbContext dbContext) where T : class
        {
            var entityType = dbContext.Model.FindEntityType(typeof(T));
            if (entityType == null)
            {
                throw new InvalidOperationException($"Entity type {typeof(T).Name} not found in the model.");
            }

            var navigations = entityType.GetNavigations();

            foreach (var navigation in navigations)
            {
                query = query.Include(navigation.Name);
            }

            return query;
        }
    }
}