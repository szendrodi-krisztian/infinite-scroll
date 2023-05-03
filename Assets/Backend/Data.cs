using System;
using System.Collections.Generic;
using System.Linq;

namespace Knife
{
    public struct LevelID
    {
        private readonly short uuid; // 16
    }

    public struct Level
    {
        private LevelID id;

        private string title;
        private bool progress;
        private bool runMode;
        private string author;
        private int votes;
        private float rating;
        private string playerCount;
        private string completionRate;
        private string firstTryRate;
        private string creationDate;

        private string comment;
        private string recordHolder;
        private int recordScore;
        private string firstTryHolderName;
        private int firstTryRecordScore;
        private byte[] thumbnail;
    }

    public class LevelPage
    {
        private LevelID[] levels;
        public string PreparedJSON { get; }
    }

    public class LevelCollectionPaged
    {
        private LevelPage[] pages;

        public void SelectPagesFromParent(LevelCollectionPaged parentPages, Filter filter) { }
        public void OrderPagesFromParent(LevelCollectionPaged parentPages, Sorter sorter) { }
        public string HandlePagedQuery(LevelQuery query)
        {
            var pageIndex = query.PageIndex >= pages.Length ? 0 : query.PageIndex;
            return pages[pageIndex].PreparedJSON;
        }
    }

    [Serializable]
    public class LevelQuery
    {
        private const string QueryBase = "SELECT * FROM knife_dev.user_created_level_data WHERE {0} ORDER BY {1};";

        private List<Filter> filters = new List<Filter>();
        private int pageIndex;
        private List<Sorter> sorters = new List<Sorter>();

        public LevelQuery() { }

        public LevelQuery(List<Filter> newFilters, List<Sorter> newSorters) : this()
        {
            filters = newFilters;
            sorters = newSorters;
        }

        public List<Filter> Filters => filters;
        public List<Sorter> Sorters => sorters;

        public bool HasFilters => filters.Count > 0;
        public bool HasSorters => sorters.Count > 0;
        public int PageIndex => pageIndex;

        public Filter Where(string propertyName)
        {
            var filter = new Filter(this, propertyName);
            filters.Add(filter);
            return filter;
        }

        public Sorter OrderBy(string propertyName)
        {
            var sorter = new Sorter(this, propertyName);
            sorters.Add(sorter);
            return sorter;
        }

        public override string ToString()
        {
            var whereClause = string.Join(" AND ", filters.Select(x => x.ToString()));
            var orderByClause = string.Join(", ", sorters.Select(x => x.ToString()));

            return string.Format(LevelQuery.QueryBase, whereClause, orderByClause);
        }
    }

    [Serializable]
    public class Sorter
    {
        private readonly object orderingProperty;
        private readonly LevelQuery query;
        private string action;

        public Sorter(LevelQuery query, object newOrderingProperty)
        {
            this.query = query;
            orderingProperty = newOrderingProperty;
        }

        public LevelQuery Ascending()
        {
            action = "{0} ASC";
            return query;
        }

        public LevelQuery Descending()
        {
            action = "{0} DESC";
            return query;
        }

        public override string ToString() => string.Format(action, orderingProperty);
    }

    [Serializable]
    public class Filter
    {
        private readonly object a;
        private readonly LevelQuery query;
        private string action;
        private object b;

        public Filter(LevelQuery query, object newA)
        {
            this.query = query;
            a = newA;
        }

        public LevelQuery BiggerThan(object newB)
        {
            action = "({0} > {1})";
            b = newB;
            return query;
        }

        public LevelQuery SmallerThan(object newB)
        {
            action = "({0} < {1})";
            b = newB;
            return query;
        }

        public LevelQuery EqualsTo(object newB)
        {
            action = "({0} > {1})";
            b = newB;
            return query;
        }

        public override string ToString() => string.Format(action, a, b);
    }

    public class RootLevelCollection
    {
        private Dictionary<LevelID, Level> levels;
        private LevelCollection root;

        public RootLevelCollection()
        {
            // TODO: select from db
            levels = new Dictionary<LevelID, Level>();
            root = new LevelCollection();
        }
    }

    public class LevelCollection
    {
        private readonly LevelCollectionPaged pages;

        private readonly Dictionary<Sorter, LevelCollection> sortedResults;
        private readonly Dictionary<Filter, LevelCollection> subQueries;

        public LevelCollection()
        {
            sortedResults = new Dictionary<Sorter, LevelCollection>();
            subQueries = new Dictionary<Filter, LevelCollection>();
            pages = new LevelCollectionPaged();
        }

        private LevelCollection(LevelCollection parent, Filter filter)
        {
            sortedResults = new Dictionary<Sorter, LevelCollection>();
            subQueries = new Dictionary<Filter, LevelCollection>();
            pages = new LevelCollectionPaged();

            pages.SelectPagesFromParent(parent.pages, filter);
        }

        private LevelCollection(LevelCollection parent, Sorter sorter)
        {
            sortedResults = new Dictionary<Sorter, LevelCollection>();
            subQueries = new Dictionary<Filter, LevelCollection>();
            pages = new LevelCollectionPaged();

            pages.OrderPagesFromParent(parent.pages, sorter);
        }

        public string Query(LevelQuery query)
        {
            if (query.HasFilters)
            {
                var currentFilter = query.Filters[0];

                var newFilters = query.Filters;
                newFilters.RemoveAt(0);
                var subQuery = new LevelQuery(newFilters, query.Sorters);

                if (subQueries.TryGetValue(currentFilter, out var cached))
                {
                    return cached.Query(subQuery);
                }

                var newCollection = new LevelCollection(this, currentFilter);
                subQueries[currentFilter] = newCollection;
                return newCollection.Query(subQuery);
            }

            if (query.HasSorters)
            {
                var currentSorter = query.Sorters[0];

                var newSorters = query.Sorters;
                newSorters.RemoveAt(0);
                var subQuery = new LevelQuery(query.Filters, newSorters);

                if (sortedResults.TryGetValue(currentSorter, out var cached))
                {
                    return cached.Query(subQuery);
                }

                var newCollection = new LevelCollection(this, currentSorter);
                sortedResults[currentSorter] = newCollection;
                return newCollection.Query(subQuery);
            }

            return pages.HandlePagedQuery(query);
        }
    }
}