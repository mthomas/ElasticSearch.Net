﻿using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace ElasticSearch.Client.QueryDSL
{
	/// <summary>
	/// a queryDSL wrapper 
	/// </summary>
	[JsonConverter(typeof(ElasticQueryConverterer))]
	public class ElasticQuery
	{
		public ElasticQuery(int from, int size, bool explatin = false)
		{
			From = from;
			Size = size;
			Explain = explatin;
		}

		public ElasticQuery(IQuery query, SortItem sortItem, int from, int size)
		{
			Query = query;
			SortItems = new List<SortItem>() { sortItem };
			From = from;
			Size = size;
		}

		[DefaultValue(0)]
		public int From { get; private set; }
		[DefaultValue(10)]
		public int Size { get; private set; }
		[DefaultValue(false)]
		public bool Explain { set; get; }

		public List<string> Fields;
		public List<SortItem> SortItems;

		public IQuery Query;

        public IFacet Facets;

		public ElasticQuery SetQuery(IQuery query)
		{
			Query = query;
			return this;
		}

		public ElasticQuery SetSortItems(List<SortItem> sortItems)
		{
			SortItems = sortItems;
			return this;
		}

		public ElasticQuery AddField(string field)
		{
			if (Fields == null) { Fields = new List<string>(); }
			Fields.Add(field);
			return this;
		}

		public ElasticQuery AddSortItem(SortItem sortItem)
		{
			if (SortItems == null) { SortItems = new List<SortItem>(); }
			SortItems.Add(sortItem);
			return this;
		}

		public ElasticQuery AddSortItem(string field, SortType type = SortType.Desc)
		{
			if (SortItems == null) { SortItems = new List<SortItem>(); }
			SortItems.Add(new SortItem(field, type));
			return this;
		}

		public void SetFacets()
		{

		}
        
          public ElasticQuery SetFacets(IFacet facet)
        {
            Facets = facet;
            return this;
		}

	    public ElasticQuery AddFields(string[] fields)
	    {
            if (Fields == null) { Fields = new List<string>(); }
            Fields.AddRange(fields);
	    	return this;
	    }
	}

    public interface IFacet
    {
        
    }

    [JsonConverter(typeof(TermsFacetConverterer))]
    public class TermsFacet : IFacet
    {
        internal List<TermsFacetItem> facetItems;
        public TermsFacet(string facetName, string field, int size = 10)
        {
            facetItems = new List<TermsFacetItem>();
            var item = new TermsFacetItem();
            item.FacetName = facetName;
            item.Field = field;
            item.Size = size;
            facetItems.Add(item);
        }

        public TermsFacet AddTermFacet(string facetName, string field, int size = 10)
        {
            var item = new TermsFacetItem();
            item.FacetName = facetName;
            item.Field = field;
            item.Size = size;
            facetItems.Add(item);
            return this;
        }

        public class TermsFacetItem
        {
            public string Field;
            public int Size;
            public string FacetName { get; set; }
        }
    }


	public class Filter
	{
		public IQuery Query;
	}
}
