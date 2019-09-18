
namespace DB.Models
{
    using Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;


    public static class Extensions
    {

        public static bool ExistsOn<T>(this T item, DateTime? date = null) where T : IPopulateDates
        {
            date = date ?? DateTime.UtcNow;
            if (date == DateTime.MinValue) date = DateTime.UtcNow;

            // TODO:  date arg not used ?  
            var exists = true;

            if (item is IDeletable deletable)
                exists &= deletable.IsActive;

            return exists;
        }

        public static IEnumerable<T> GetHistoryToDate<T>(this IEnumerable<T> ie, DateTime? date = null) where T : IDateRange
        {
            date = date ?? DateTime.UtcNow;
            if (date == DateTime.MinValue) date = DateTime.UtcNow;

            ie = ie.Where(x => x.IsActive).OrderByDescending(x => x.IDateRangeId);

            return ie;
        }

        public static T GetApplicableItem<T>(this IEnumerable<T> ie, DateTime? date = null) where T : IDateRange
        {
            date = date ?? DateTime.UtcNow;
            if (date == DateTime.MinValue) date = DateTime.UtcNow;

            var items = ie.GetApplicableItems(date);
            return items.FirstOrDefault();
        }

        public static IEnumerable<T> GetApplicableItems<T>(this IEnumerable<T> ie, DateTime? date = null) where T : IDateRange
        {
            date = date ?? DateTime.UtcNow;
            if (date == DateTime.MinValue) date = DateTime.UtcNow;

            // var items = ie.Where(x => x.IsDateRangeMatch(date)).OrderByDescending(x => x.CreatedDate).ThenByDescending(x => x.ModifiedDate).ThenByDescending(x => x.IDateRangeId);
            var items = ie.Where(x => x.IsDateRangeMatch(date)).OrderByDescending(x => x.IDateRangeId);
            return items;
        }

        //public static IEnumerable<T> GetConflictingItems<T>(this IEnumerable<T> ie, T model) where T : IDateRange
        //{
        //    return model == null
        //        ? Enumerable.Empty<T>()
        //        : ie.Where(x => x.IsDateRangeOverlap(model.StartDate, model.EndDate))
        //            .OrderByDescending(x => x.IDateRangeId);
        //}

        //public static bool IsDateRangeOverlap<T>(this T item, DateTime? start, DateTime? end) where T : IDateRange
        //{
        //    start = start ?? Default.StartDate;
        //    end = end ?? Default.EndDate;

        //    return item.IsActive && item.StartDate < end && item.EndDate > start;
        //}

        public static bool IsDateRangeOverlap<T>(this T item1, T item2) where T : IDateRange
        {
            return item1.IsActive && item2.IsActive && item1.StartDate < item2.EndDate && item1.EndDate > item2.StartDate;
        }

        public static bool IsDateRangeMatch<T>(this T item, DateTime? date = null, bool allowFutureItems = false) where T : IDateRange
        {
            date = date ?? DateTime.UtcNow;
            if (date == DateTime.MinValue) date = DateTime.UtcNow;

            return item.ExistsOn(date) && item.StartDate <= date && item.EndDate > date;
        }

        public static T GetFutureApplicableItem<T>(this IEnumerable<T> ie, DateTime? date = null) where T : IDateRange
        {
            date = date ?? DateTime.UtcNow;
            if (date == DateTime.MinValue) date = DateTime.UtcNow;

            var items = ie.GetFutureApplicableItems(date)
                .OrderBy(x => x.StartDate) //pick soonest startdate to be applicable ("next applicable")
                .ThenByDescending(x => x.IDateRangeId); //break ties by highest idaterangeid

            return items.FirstOrDefault();
        }

        public static IEnumerable<T> GetFutureApplicableItems<T>(this IEnumerable<T> ie, DateTime? date = null) where T : IDateRange
        {
            date = date ?? DateTime.UtcNow;
            if (date == DateTime.MinValue) date = DateTime.UtcNow;

            var items = ie.Where(x => x.IsFutureDateRangeMatch(date)).OrderByDescending(x => x.IDateRangeId);
            return items;
        }

        public static T GetPastApplicableItem<T>(this IEnumerable<T> ie, DateTime? date = null) where T : IDateRange
        {
            date = date ?? DateTime.UtcNow;
            if (date == DateTime.MinValue) date = DateTime.UtcNow;

            var items = ie.GetPastApplicableItems(date);
            return items.FirstOrDefault();
        }

        public static IEnumerable<T> GetPastApplicableItems<T>(this IEnumerable<T> ie, DateTime? date = null) where T : IDateRange
        {
            date = date ?? DateTime.UtcNow;
            if (date == DateTime.MinValue) date = DateTime.UtcNow;

            var items = ie.Where(x => x.IsPastDateRangeMatch(date)).OrderByDescending(x => x.IDateRangeId);
            return items;
        }


        public static IEnumerable<T> GetOverlappingTimeLineItems<T>(this IEnumerable<T> ie, DateTime startDate , DateTime endDate) where T : IDateRange
        {
            var items = ie.Where(x => ((startDate.Date < x.EndDate && x.StartDate < endDate.Date)
                            || (x.StartDate < startDate.Date && endDate.Date < x.EndDate)
                            || (startDate.Date < x.StartDate && x.EndDate < endDate.Date)));
            return items;
        }

        public static bool IsFutureDateRangeMatch<T>(this T item, DateTime? date = null) where T : IDateRange
        {
            date = date ?? DateTime.UtcNow;
            if (date == DateTime.MinValue) date = DateTime.UtcNow;

            return item.ExistsOn(date) && item.StartDate > date;
        }

        public static bool IsPastDateRangeMatch<T>(this T item, DateTime? date = null) where T : IDateRange
        {
            date = date ?? DateTime.UtcNow;
            if (date == DateTime.MinValue) date = DateTime.UtcNow;

            return item.ExistsOn(date) && item.StartDate < date;
        }

        /// <summary>
        /// Starts the given tasks and waits for them to complete. This will run, at most, the specified number of tasks in parallel.
        /// <para>NOTE: If one of the given tasks has already been started, an exception will be thrown.</para>
        /// </summary>
        /// <param name="tasksToRun">The tasks to run.</param>
        /// <param name="maxTasksToRunInParallel">The maximum number of tasks to run in parallel.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public static void StartAndWaitAllThrottled(this IEnumerable<Task> tasksToRun, int maxTasksToRunInParallel, CancellationToken cancellationToken = new CancellationToken())
        {
            StartAndWaitAllThrottled(tasksToRun, maxTasksToRunInParallel, -1, cancellationToken);
        }

        /// <summary>
        /// Starts the given tasks and waits for them to complete. This will run, at most, the specified number of tasks in parallel.
        /// <para>NOTE: If one of the given tasks has already been started, an exception will be thrown.</para>
        /// </summary>
        /// <param name="tasksToRun">The tasks to run.</param>
        /// <param name="maxTasksToRunInParallel">The maximum number of tasks to run in parallel.</param>
        /// <param name="timeoutInMilliseconds">The maximum milliseconds we should allow the max tasks to run in parallel before allowing another task to start. Specify -1 to wait indefinitely.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public static void StartAndWaitAllThrottled(this IEnumerable<Task> tasksToRun, int maxTasksToRunInParallel, int timeoutInMilliseconds, CancellationToken cancellationToken = new CancellationToken())
        {
            // Convert to a list of tasks so that we don&#39;t enumerate over it multiple times needlessly.
            var tasks = tasksToRun.ToList();

            using (var throttler = new SemaphoreSlim(maxTasksToRunInParallel))
            {
                var postTaskTasks = new List<Task>();

                // Have each task notify the throttler when it completes so that it decrements the number of tasks currently running.
                tasks.ForEach(t => postTaskTasks.Add(t.ContinueWith(tsk => throttler.Release())));

                // Start running each task.
                foreach (var task in tasks)
                {
                    // Increment the number of tasks currently running and wait if too many are running.
                    throttler.Wait(timeoutInMilliseconds, cancellationToken);

                    cancellationToken.ThrowIfCancellationRequested();
                    task.Start();
                }

                // Wait for all of the provided tasks to complete.
                // We wait on the list of "post" tasks instead of the original tasks, otherwise there is a potential race condition where the throttler&#39;s using block is exited before some Tasks have had their "post" action completed, which references the throttler, resulting in an exception due to accessing a disposed object.
                Task.WaitAll(postTaskTasks.ToArray(), cancellationToken);
            }
        }
        
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var known = new HashSet<TKey>();
            return source.Where(element => known.Add(keySelector(element)));
        }

        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static bool IsActiveInDateRange<T>(this T item, DateTime dos) where T : IDateRange
        {
            return item.IsActive && item.StartDate <= dos && item.EndDate > dos;
        }

        public static string ToMyDateString(this DateTime? dt) => dt.HasValue ? ((DateTime)dt).ToString("MM'/'dd'/'yyyy") : string.Empty;

        public static void TrimAllStrings<T>(this T obj)
        {
            var stringProperties = obj.GetType().GetProperties()
                          .Where(p => p.PropertyType == typeof(string));
            foreach (var stringProperty in stringProperties)
            {
                string currentValue = (string)stringProperty.GetValue(obj, null);
                if (currentValue != null)
                {
                    stringProperty.SetValue(obj, currentValue.Trim(), null);
                }

            }

        }
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> whereClause)
        {
            if (condition)
            {
                return query.Where(whereClause);
            }
            return query;
        }
    }
}
