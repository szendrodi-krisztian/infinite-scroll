using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Knife
{
    [Serializable]
    public struct LevelID
    {
        [SerializeField] private string uuid; // 16
    }

    [Serializable]
    public class Level
    {
        [SerializeField] private LevelID id;
        [SerializeField] private string title;
        [SerializeField] private float rating;
        [SerializeField] private int runMode;
        [SerializeField] private string author;
        [SerializeField] private string authorID;
        [SerializeField] private int playerCount;
        [SerializeField] private int completionCount;
        [SerializeField] private float completionRate;
        [SerializeField] private string creationDate;
        [SerializeField] private string comment;
        [SerializeField] private int rateCount;
        [SerializeField] private float firstTryRate;
        [SerializeField] private string recordHolder;
        [SerializeField] private int recordScore;
        [SerializeField] private string firstTryHolderName;
        [SerializeField] private int firstTryRecordScore;
        [SerializeField] private string progress;
        [SerializeField] private string thumbnail;

        public string Thumbnail => thumbnail;
        public string BestFirstTry => $"{firstTryRecordScore} ({firstTryHolderName})";
        public string WorldRecord => $"{recordScore} ({recordHolder})";
        public string Comment => comment;
        public string Title => title;
        public string MyProgress => "?";
        public string RunMode => runMode == 1 ? "+" : "-";
        public string Author => author;
        public string Votes => rateCount.ToString();
        public string Rating => rating.ToString("F1");
        public string PlayersPlayed => playerCount.ToString();
        public string PlayersCompleted => completionCount.ToString();
        public string PlayersFirstTry => firstTryRate.ToString("F1");

        public override string ToString() => $"{title}({runMode}) - {author}";
    }

    [Serializable]
    public class LevelPage
    {
        [SerializeField] private Level[] levels;
        [SerializeField] private int totalCount;

        public int TotalCount => totalCount;
        public Level[] Levels => levels;
    }

    [Serializable]
    public class LevelPageQuery
    {
        [SerializeField] private LevelQuery query;
        [SerializeField] private int pageIndex;

        public LevelPageQuery Query(LevelQuery newQuery)
        {
            query = newQuery;
            return this;
        }

        public LevelPageQuery Page(int page)
        {
            pageIndex = page;
            return this;
        }
    }

    [Serializable]
    public class LevelQuery
    {
        private const string QueryBase = "SELECT * FROM knife_dev.user_created_level_data WHERE {0} ORDER BY {1};";

        [SerializeField] private List<Filter> filters = new();
        [SerializeField] private List<Sorter> sorters = new();

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

        public LevelQuery SetFilters(IEnumerable<Filter> newFilters)
        {
            filters = newFilters.Where(f => !string.IsNullOrWhiteSpace(f.Action)).ToList();
            return this;
        }

        public LevelQuery SetSorters(IEnumerable<Sorter> newSorters)
        {
            if (newSorters != null)
                sorters = newSorters.Where(s => s != null && !string.IsNullOrWhiteSpace(s.OrderingProperty)).ToList();

            return this;
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
        [SerializeField] private string orderingProperty;
        [SerializeField] private string action;

        [NonSerialized] private LevelQuery query;

        public Sorter(LevelQuery query, string newOrderingProperty)
        {
            this.query = query;
            orderingProperty = newOrderingProperty;
        }

        public string OrderingProperty => orderingProperty;

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
        [SerializeField] private string action;
        [SerializeField] private string a;
        [SerializeField] private string b;

        [NonSerialized] private LevelQuery query;

        public Filter(LevelQuery query, string newA)
        {
            this.query = query;
            a = newA;
        }

        public string A => a;
        public string Action => action;
        public string B => b;

        public LevelQuery BiggerThan(string newB)
        {
            action = "({0} > {1})";
            b = newB;
            return query;
        }

        public LevelQuery SmallerThan(string newB)
        {
            action = "({0} < {1})";
            b = newB;
            return query;
        }

        public LevelQuery EqualsTo(string newB)
        {
            action = "({0} = {1})";
            b = newB;
            return query;
        }

        public override string ToString() => string.Format(action, a, b);
    }
}