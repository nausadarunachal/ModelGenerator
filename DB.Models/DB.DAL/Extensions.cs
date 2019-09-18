using System.Collections.Generic;

namespace RxSense.DAL
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using DGDean.Models.Base;

    public static class Extensions
    {
        public static async Task ThrowIfFound<T>(this DbSet<T> dbSet, Expression<Func<T, bool>> filter, string msg = "Entity already Exists.") where T : BaseModel
        {
            var item = await dbSet.Where(filter).FirstOrDefaultAsync();

            if (item != null)
                throw new Exception(msg);
        }

        public static async Task<T> SingleAsync<T>(this DbSet<T> dbSet, Expression<Func<T, bool>> filter, string msg) where T : BaseModel
        {
            var item = await dbSet.Where(filter).SingleOrDefaultAsync();

            if (item == null)
                throw new Exception(msg);

            return item;
        }

        public static async Task<T> FirstAsync<T>(this IQueryable<T> queryable, string msg) where T : BaseModel
        {
            var item = await queryable.FirstOrDefaultAsync();

            if (item == null)
                throw new Exception(msg);

            return item;
        }

        public static Task<List<MedispanModel>> ToActiveListAsync(this IQueryable<MedispanModel> items)
        {
            return items.Where(x => x.ItemStatus == "A").ToListAsync();
        }

        public static bool IsActive(this MedispanModel drug) => drug.ItemStatus == "A";

        private static readonly string[] brandCodes = { "M", "O", "N" };

        public static bool IsBrand(this MedispanModel model)
        {
            return brandCodes.Any(x => x.Equals(model.MultisourceCode, StringComparison.OrdinalIgnoreCase));
        }
           public static bool IsGeneric(this MedispanModel model) => !model.IsBrand();
    }
}