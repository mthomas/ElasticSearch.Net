using System.Collections.Generic;
using System.Diagnostics.Contracts;
using ElasticSearch.Client.Domain;
using Newtonsoft.Json.Linq;

namespace ElasticSearch.Client.Utils
{
	public static class ElasticSearchExtensions
	{
		public static string Fill(this string formatString, params object[] args)
		{
			Contract.Assert(formatString != null);
			Contract.Assert(args != null);
			return string.Format(formatString, args);
		}
		
		/// <summary>
		///  Resolve Lucene Keyword
		/// </summary>
		/// <param name="searchKeyword"></param>
		/// <returns></returns>
		public static string ReplaceLuceneKeywordChar(this string searchKeyword)
		{
			if(searchKeyword.Length==1)
			{
				if (searchKeyword.StartsWith("!"))
				{
					searchKeyword = "*";
				}
			}

			searchKeyword = searchKeyword.Replace(@"\", @"\\");
			string strFilter = ":*?~!@^-+'\"\\{}[]()";
			char[] arrFilterChar = strFilter.ToCharArray();
			foreach (char c in arrFilterChar)
			{
				searchKeyword = searchKeyword.Replace("" + c, @"\" + c);
			}
			return searchKeyword;
		}

        public static void InitOrGetFacets(this SearchResult result)
        {
            if (result != null && !string.IsNullOrEmpty(result.Response))
            {
                if (result._facets == null) { result._facets = new Dictionary<string, Dictionary<string, int>>(); }
                var jobject = JObject.Parse(result.Response);
                var facets = jobject["facets"];
                foreach (JToken jToken in facets)
                {

                    var facetkey = jToken.Value<JProperty>();
                    var fkey = facetkey.Name;
                    var fvalue = facetkey.Value;

                    #region trash

                    //                    var missing= fvalue["missing"];
                    //                    var type= fvalue["_type"];
                    //                    var total= fvalue["total"];
                    //                    var other= fvalue["other"];

                    #endregion

                    var terms = fvalue["terms"];

                    var dict = new Dictionary<string, int>();
                    foreach (JToken term in terms)
                    {
                        try
                        {
                            var tm = term["term"];
                            var count = term["count"];
                            dict[tm.Value<string>().ToString()] = count.Value<int>();
                        }
                        catch (System.Exception e)
                        {
//                            logger.HandleException(e, "xxx_search_facets_json_parse_failure");
                        }
                    }

                    if (dict.Count > 0)
                    {
                        result._facets[fkey] = dict;
                    }
                }
            }
        }
    
    }
}