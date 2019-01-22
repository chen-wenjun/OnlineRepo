using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Raymark.Entities;
using Raymark.Caching;

public partial class MonitorCachePage : Page
{
	protected int Index = 1;

	protected void Page_Load(object sender, EventArgs e)
	{
		// Setup title and header text.
		headerLtl.Text = String.Format("POS service cache monitor page - [{0}]", Request.Url.Host);
		Title = headerLtl.Text;

        // Cache

        {
            // Count literal
            Literal cacheCountLtl = new Literal();
            cacheHolder.Controls.Add(cacheCountLtl);
            int cacheRegionCount = 0;
            long cacheTotalCount = Raymark.Caching.Cache.Current.GetAllCount();
            long cacheKeyNumber = 0;

            List<string> cacheRegions = Raymark.Caching.Cache.Current.GetRegions();
            cacheRegionCount = cacheRegions.Count;

            if (cacheRegions.Count > 0)
            {
                foreach (string cacheRegion in cacheRegions)
                {
                    Literal rowLtl = new Literal();

                    rowLtl.Text = $@"
                                <div class=""row"">
                                    <div class=""col-xs-2 table-bordered"">{cacheRegion}</div>
                                    <div class=""col-xs-2 table-bordered"">{Raymark.Caching.Cache.Current.GetCount(cacheRegion)}</div>
                                </div>
                        ";

                    cacheHolder.Controls.Add(rowLtl);

                    List<CacheTypeEnum> cacheTypes = Raymark.Caching.Cache.Current.GetTypes(cacheRegion);

                    foreach (CacheTypeEnum cacheType in cacheTypes)
                    {
                        rowLtl = new Literal();

                        rowLtl.Text = $@"
                                <div class=""row"">
                                    <div class=""col-xs-2 col-xs-offset-2 table-bordered"">{cacheType}</div>
                                    <div class=""col-xs-4 table-bordered"">{Raymark.Caching.Cache.Current.GetCount(cacheRegion, cacheType)}</div>
                                </div>
                            ";

                        cacheHolder.Controls.Add(rowLtl);

                        List<string> cacheKeys = Raymark.Caching.Cache.Current.GetKeys(cacheRegion, cacheType);

                        foreach (string cacheKey in cacheKeys)
                        {
                            object cacheValue = Raymark.Caching.Cache.Current.GetByCacheKey(cacheKey);
                            string cacheValueString = "null";
                            if (cacheValue is Store)
                                cacheValueString = ((Store)cacheValue).Name;
                            else if (cacheValue != null)
                                cacheValueString = cacheValue.ToString();

                            rowLtl = new Literal();

                            rowLtl.Text = $@"
                                <div class=""row"">
                                    <div class=""col-xs-2 col-xs-offset-2 table-bordered text-right"">{++cacheKeyNumber}</div>
                                    <div class=""col-xs-4 table-bordered"">{cacheKey}</div>
                                    <div class=""col-xs-4 table-bordered"">{cacheValueString}</div>
                                </div>
                                ";

                            cacheHolder.Controls.Add(rowLtl);
                        }

                    }

                }
            }

            cacheCountLtl.Text = string.Format(@"<div style='color:blue;'>Region Count: {0}<br />Total Cache Count: {1}</div>", cacheRegionCount, cacheTotalCount);

        }









        //{
        //    // Count literal
        //    Literal cacheCountLtl = new Literal();
        //    cacheHolder.Controls.Add(cacheCountLtl);
        //    int cacheRegionCount = 0;
        //    long cacheTotalCount = Raymark.Caching.Cache.Current.GetAllCount();
        //    //List<string> cacheAllKeys = Raymark.Caching.Cache.Current.GetKeys();

        //    List<string> cacheRegions = Raymark.Caching.Cache.Current.GetRegions();
        //    cacheRegionCount = cacheRegions.Count;

        //    if (cacheRegions.Count > 0)
        //    {
        //        // CacheRegion table
        //        Table cacheRegiontable = new Table();
        //        cacheRegiontable.CellSpacing = 0;
        //        cacheRegiontable.CellPadding = 3;
        //        cacheRegiontable.BorderWidth = 1;

        //        //TableHeaderRow cacheRegionHeaderRow = new TableHeaderRow();

        //        //TableHeaderCell cacheRegionHeaderCell = new TableHeaderCell();
        //        //cacheRegionHeaderCell.Text = "Region";
        //        //cacheRegionHeaderRow.Cells.Add(cacheRegionHeaderCell);

        //        //cacheRegiontable.Rows.Add(cacheRegionHeaderRow);

        //        foreach (string cacheRegion in cacheRegions)
        //        {
        //            TableCell cacheRegionValueCell = new TableCell();
        //            TableCell cacheTypeCell = new TableCell();
        //            cacheRegionValueCell.BorderWidth = new Unit(1);
        //            cacheTypeCell.BorderWidth = new Unit(1);

        //            cacheRegionValueCell.Text = cacheRegion;

        //            TableRow cacheRegionRow = new TableRow();
        //            cacheRegionRow.Cells.Add(cacheRegionValueCell);
        //            cacheRegionRow.Cells.Add(cacheTypeCell);
        //            cacheRegiontable.Rows.Add(cacheRegionRow);


        //            // CacheType table
        //            Table cacheTypetable = new Table();
        //            cacheTypetable.CellSpacing = 0;
        //            cacheTypetable.CellPadding = 3;
        //            cacheTypetable.BorderWidth = 1;
        //            cacheTypeCell.Controls.Add(cacheTypetable);

        //            //TableHeaderRow cacheTypeHeaderRow = new TableHeaderRow();
        //            //TableHeaderCell cacheTypeHeaderCell = new TableHeaderCell();
        //            //cacheTypeHeaderCell.Text = "Type";
        //            //cacheTypeHeaderRow.Cells.Add(cacheTypeHeaderCell);
        //            //cacheTypetable.Rows.Add(cacheTypeHeaderRow);

        //            List<CacheTypeEnum> cacheTypes = Raymark.Caching.Cache.Current.GetTypes(cacheRegion);

        //            foreach (CacheTypeEnum cacheType in cacheTypes)
        //            {
        //                TableCell cacheTypeValueCell = new TableCell();
        //                TableCell cacheItemCell = new TableCell();
        //                cacheTypeValueCell.BorderWidth = new Unit(1);
        //                cacheItemCell.BorderWidth = new Unit(1);

        //                cacheTypeValueCell.Text = cacheType.ToString();

        //                TableRow cacheTypeRow = new TableRow();
        //                cacheTypeRow.Cells.Add(cacheTypeValueCell);
        //                cacheTypeRow.Cells.Add(cacheItemCell);
        //                cacheTypetable.Rows.Add(cacheTypeRow);


        //                // CacheItem table
        //                Table cacheItemtable = new Table();
        //                cacheItemtable.CellSpacing = 0;
        //                cacheItemtable.CellPadding = 3;
        //                cacheItemtable.BorderWidth = 1;
        //                cacheItemCell.Controls.Add(cacheItemtable);

        //                //TableHeaderRow cacheItemHeaderRow = new TableHeaderRow();
        //                //TableHeaderCell cacheKeyHeaderCell = new TableHeaderCell();
        //                //TableHeaderCell cacheValueHeaderCell = new TableHeaderCell();
        //                //cacheKeyHeaderCell.Text = "Key";
        //                //cacheValueHeaderCell.Text = "Value";
        //                //cacheItemHeaderRow.Cells.Add(cacheKeyHeaderCell);
        //                //cacheItemHeaderRow.Cells.Add(cacheValueHeaderCell);
        //                //cacheItemtable.Rows.Add(cacheItemHeaderRow);

        //                List<string> cacheKeys = Raymark.Caching.Cache.Current.GetKeys(cacheRegion, cacheType);

        //                foreach (string cacheKey in cacheKeys)
        //                {
        //                    TableCell cacheItemKeyCell = new TableCell();
        //                    TableCell cacheItemValueCell = new TableCell();
        //                    cacheItemKeyCell.BorderWidth = new Unit(1);
        //                    cacheItemValueCell.BorderWidth = new Unit(1);

        //                    cacheItemKeyCell.Text = cacheKey;
        //                    object cacheValue = Raymark.Caching.Cache.Current.GetByCacheKey(cacheKey);
        //                    string cacheValueString = "null";
        //                    if (cacheValue is Store)
        //                        cacheValueString = ((Store)cacheValue).Name;
        //                    else if (cacheValue != null)
        //                        cacheValueString = cacheValue.ToString();

        //                    cacheItemValueCell.Text = cacheValueString;

        //                    TableRow cacheItemRow = new TableRow();
        //                    cacheItemRow.Cells.Add(cacheItemKeyCell);
        //                    cacheItemRow.Cells.Add(cacheItemValueCell);
        //                    cacheItemtable.Rows.Add(cacheItemRow);
        //                }

        //            }

        //        }

        //        cacheHolder.Controls.Add(cacheRegiontable);
        //    }

        //    cacheCountLtl.Text = string.Format(@"<div style='color:blue;'>Region Count: {0}<br />Total Count: {1}</div>", cacheRegionCount, cacheTotalCount);

        //}



  //      // Service Cache

  //      // Count literal
  //      Literal serviceCacheCountLtl = new Literal();
		//serviceCacheHolder.Controls.Add(serviceCacheCountLtl);
		//int serviceCacheTypeCount = 0;
		//int serviceCacheItemCount = 0;
  //      long totalCount = Raymark.Caching.Cache.Current.GetAllCount();

		//List<CacheTypeEnum> serviceCacheTypes = Raymark.Caching.Cache.Current.GetTypesOfServiceCache();
		//serviceCacheTypeCount = serviceCacheTypes.Count;

		//if (serviceCacheTypes.Count > 0)
		//{


		//	// CacheType table
		//	Table cacheTypetable = new Table();
		//	cacheTypetable.CellSpacing = 0;
		//	cacheTypetable.CellPadding = 3;
		//	cacheTypetable.BorderWidth = 1;

		//	TableHeaderRow cacheTypeHeaderRow = new TableHeaderRow();
		//	TableHeaderCell cacheTypeHeaderCell = new TableHeaderCell();
		//	cacheTypeHeaderCell.Text = "Type";
		//	cacheTypeHeaderRow.Cells.Add(cacheTypeHeaderCell);
		//	cacheTypetable.Rows.Add(cacheTypeHeaderRow);

		//	foreach (CacheTypeEnum cacheType in serviceCacheTypes)
		//	{
		//		TableCell cacheTypeValueCell = new TableCell();
		//		TableCell cacheItemCell = new TableCell();
		//		cacheTypeValueCell.BorderWidth = new Unit(1);
		//		cacheItemCell.BorderWidth = new Unit(1);

		//		cacheTypeValueCell.Text = cacheType.ToString();

		//		TableRow cacheTypeRow = new TableRow();
		//		cacheTypeRow.Cells.Add(cacheTypeValueCell);
		//		cacheTypeRow.Cells.Add(cacheItemCell);
		//		cacheTypetable.Rows.Add(cacheTypeRow);


		//		// CacheItem table
		//		Table cacheItemtable = new Table();
		//		cacheItemtable.CellSpacing = 0;
		//		cacheItemtable.CellPadding = 3;
		//		cacheItemtable.BorderWidth = 1;
		//		cacheItemCell.Controls.Add(cacheItemtable);

		//		TableHeaderRow cacheItemHeaderRow = new TableHeaderRow();
		//		TableHeaderCell cacheKeyHeaderCell = new TableHeaderCell();
		//		TableHeaderCell cacheValueHeaderCell = new TableHeaderCell();
		//		cacheKeyHeaderCell.Text = "Key";
		//		cacheValueHeaderCell.Text = "Value";
		//		cacheItemHeaderRow.Cells.Add(cacheKeyHeaderCell);
		//		cacheItemHeaderRow.Cells.Add(cacheValueHeaderCell);
		//		cacheItemtable.Rows.Add(cacheItemHeaderRow);

		//		List<string> cacheKeys = Raymark.Caching.Cache.Current.GetKeysOfServiceCache(cacheType);
		//		serviceCacheItemCount += cacheKeys.Count;

		//		foreach (string cacheKey in cacheKeys)
		//		{
		//			TableCell cacheItemKeyCell = new TableCell();
		//			TableCell cacheItemValueCell = new TableCell();
		//			cacheItemKeyCell.BorderWidth = new Unit(1);
		//			cacheItemValueCell.BorderWidth = new Unit(1);

		//			cacheItemKeyCell.Text = cacheKey;
		//			object cacheValue = Raymark.Caching.Cache.Current.Get(cacheType, cacheKey);
		//			string cacheValueString = "null";
		//			if (cacheValue is Store)
		//				cacheValueString = ((Store)cacheValue).Name;
		//			else if (cacheValue != null)
		//				cacheValueString = cacheValue.ToString();

		//			cacheItemValueCell.Text = cacheValueString;

		//			TableRow cacheItemRow = new TableRow();
		//			cacheItemRow.Cells.Add(cacheItemKeyCell);
		//			cacheItemRow.Cells.Add(cacheItemValueCell);
		//			cacheItemtable.Rows.Add(cacheItemRow);
		//		}

		//	}

		//	serviceCacheHolder.Controls.Add(cacheTypetable);
		//}

		//serviceCacheCountLtl.Text = string.Format(@"<div style='color:blue;'>Type Count: {0}<br />Item Count: {1}<br />Total Count: {2}</div>", serviceCacheTypeCount, serviceCacheItemCount, totalCount);


		//// Store Cache

		//// Count literal
		//Literal storeCacheCountLtl = new Literal();
		//storeCacheHolder.Controls.Add(storeCacheCountLtl);
		//int storeCacheStoreCount = 0;
		//int storeCacheTypeCount = 0;
		//int storeCacheItemCount = 0;


		//List<string> stores = Raymark.Caching.Cache.Current.GetStores();
		//storeCacheStoreCount = stores.Count;

		//if (stores.Count > 0)
		//{
		//	Table storetable = new Table();
		//	storetable.CellSpacing = 0;
		//	storetable.CellPadding = 3;
		//	storetable.BorderWidth = 1;

		//	TableHeaderRow storeHeaderRow = new TableHeaderRow();
		//	TableHeaderCell storeCodeHeaderCell = new TableHeaderCell();
		//	storeCodeHeaderCell.Text = "Store Code";
		//	storeHeaderRow.Cells.Add(storeCodeHeaderCell);
		//	storetable.Rows.Add(storeHeaderRow);

		//	foreach (string storeCode in stores)
		//	{
		//		TableCell storeCell = new TableCell();
		//		TableCell cacheTypeCell = new TableCell();
		//		storeCell.BorderWidth = new Unit(1);
		//		cacheTypeCell.BorderWidth = new Unit(1);

		//		storeCell.Text = storeCode;

		//		TableRow storeRow = new TableRow();
		//		storeRow.Cells.Add(storeCell);
		//		storeRow.Cells.Add(cacheTypeCell);
		//		storetable.Rows.Add(storeRow);

		//		List<CacheTypeEnum> cacheTypes = Raymark.Caching.Cache.Current.GetTypesOfStoreCache(storeCode);
		//		storeCacheTypeCount += cacheTypes.Count;

		//		// CacheType table
		//		Table cacheTypetable = new Table();
		//		cacheTypetable.CellSpacing = 0;
		//		cacheTypetable.CellPadding = 3;
		//		cacheTypetable.BorderWidth = 1;
		//		cacheTypeCell.Controls.Add(cacheTypetable);

		//		TableHeaderRow cacheTypeHeaderRow = new TableHeaderRow();
		//		TableHeaderCell cacheTypeHeaderCell = new TableHeaderCell();
		//		cacheTypeHeaderCell.Text = "Type";
		//		cacheTypeHeaderRow.Cells.Add(cacheTypeHeaderCell);
		//		cacheTypetable.Rows.Add(cacheTypeHeaderRow);

		//		foreach (CacheTypeEnum cacheType in cacheTypes)
		//		{
		//			TableCell cacheTypeValueCell = new TableCell();
		//			TableCell cacheItemCell = new TableCell();
		//			cacheTypeValueCell.BorderWidth = new Unit(1);
		//			cacheItemCell.BorderWidth = new Unit(1);

		//			cacheTypeValueCell.Text = cacheType.ToString();

		//			TableRow cacheTypeRow = new TableRow();
		//			cacheTypeRow.Cells.Add(cacheTypeValueCell);
		//			cacheTypeRow.Cells.Add(cacheItemCell);
		//			cacheTypetable.Rows.Add(cacheTypeRow);


		//			// CacheItem table
		//			Table cacheItemtable = new Table();
		//			cacheItemtable.CellSpacing = 0;
		//			cacheItemtable.CellPadding = 3;
		//			cacheItemtable.BorderWidth = 1;
		//			cacheItemCell.Controls.Add(cacheItemtable);

		//			TableHeaderRow cacheItemHeaderRow = new TableHeaderRow();
		//			TableHeaderCell cacheKeyHeaderCell = new TableHeaderCell();
		//			TableHeaderCell cacheValueHeaderCell = new TableHeaderCell();
		//			cacheKeyHeaderCell.Text = "Key";
		//			cacheValueHeaderCell.Text = "Value";
		//			cacheItemHeaderRow.Cells.Add(cacheKeyHeaderCell);
		//			cacheItemHeaderRow.Cells.Add(cacheValueHeaderCell);
		//			cacheItemtable.Rows.Add(cacheItemHeaderRow);

		//			List<string> cacheKeys = Raymark.Caching.Cache.Current.GetKeysOfStoreCache(storeCode, cacheType);
		//			storeCacheItemCount += cacheKeys.Count;

		//			foreach (string cacheKey in cacheKeys)
		//			{
		//				TableCell cacheItemKeyCell = new TableCell();
		//				TableCell cacheItemValueCell = new TableCell();
		//				cacheItemKeyCell.BorderWidth = new Unit(1);
		//				cacheItemValueCell.BorderWidth = new Unit(1);

		//				cacheItemKeyCell.Text = cacheKey;
		//				object cacheValue = Raymark.Caching.Cache.Current.Get(storeCode, cacheType, cacheKey);
		//				cacheItemValueCell.Text = cacheValue == null ? "null" : cacheValue.ToString();

		//				TableRow cacheItemRow = new TableRow();
		//				cacheItemRow.Cells.Add(cacheItemKeyCell);
		//				cacheItemRow.Cells.Add(cacheItemValueCell);
		//				cacheItemtable.Rows.Add(cacheItemRow);
		//			}

		//		}

		//	}

		//	storeCacheHolder.Controls.Add(storetable);

		//}

		//storeCacheCountLtl.Text = string.Format(@"<div style='color:blue;'>Store Count: {0}<br />Type Count: {1}<br />Item Count: {2}</div>", storeCacheStoreCount, storeCacheTypeCount, storeCacheItemCount);


		//// Terminal Cache

		//// Count literal
		//Literal terminalCacheCountLtl = new Literal();
		//terminalCacheHolder.Controls.Add(terminalCacheCountLtl);
		//int terminalCacheStoreCount = 0;
		//int terminalCacheTerminalCount = 0;
		//int terminalCacheTypeCount = 0;
		//int terminalCacheItemCount = 0;

		//Dictionary<string, List<int>> terminals = Raymark.Caching.Cache.Current.GetTerminals();
		//terminalCacheStoreCount = terminals.Keys.Count;

		//if (terminals.Keys.Count > 0)
		//{
		//	Table storetable = new Table();
		//	storetable.CellSpacing = 0;
		//	storetable.CellPadding = 3;
		//	storetable.BorderWidth = 1;

		//	TableHeaderRow storeHeaderRow = new TableHeaderRow();
		//	TableHeaderCell storeCodeHeaderCell = new TableHeaderCell();
		//	storeCodeHeaderCell.Text = "Store Code";
		//	storeHeaderRow.Cells.Add(storeCodeHeaderCell);
		//	storetable.Rows.Add(storeHeaderRow);

		//	foreach (KeyValuePair<string, List<int>> pair in terminals)
		//	{
		//		terminalCacheTerminalCount += pair.Value.Count;

		//		string storeCode = pair.Key;

		//		TableCell storeCell = new TableCell();
		//		TableCell registerCell = new TableCell();
		//		storeCell.BorderWidth = new Unit(1);
		//		registerCell.BorderWidth = new Unit(1);

		//		storeCell.Text = storeCode;

		//		TableRow storeRow = new TableRow();
		//		storeRow.Cells.Add(storeCell);
		//		storeRow.Cells.Add(registerCell);
		//		storetable.Rows.Add(storeRow);

		//		// Register table
		//		Table registertable = new Table();
		//		registertable.CellSpacing = 0;
		//		registertable.CellPadding = 3;
		//		registertable.BorderWidth = 1;
		//		registerCell.Controls.Add(registertable);

		//		TableHeaderRow registerHeaderRow = new TableHeaderRow();
		//		TableHeaderCell registerHeaderCell = new TableHeaderCell();
		//		registerHeaderCell.Text = "Reg No";
		//		registerHeaderRow.Cells.Add(registerHeaderCell);
		//		registertable.Rows.Add(registerHeaderRow);

		//		foreach (int registerNumber in pair.Value)
		//		{
		//			TableCell registerValueCell = new TableCell();
		//			TableCell cacheTypeCell = new TableCell();
		//			registerValueCell.BorderWidth = new Unit(1);
		//			cacheTypeCell.BorderWidth = new Unit(1);

		//			registerValueCell.Text = registerNumber.ToString();

		//			TableRow registerRow = new TableRow();
		//			registerRow.Cells.Add(registerValueCell);
		//			registerRow.Cells.Add(cacheTypeCell);
		//			registertable.Rows.Add(registerRow);

		//			List<CacheTypeEnum> cacheTypes = Raymark.Caching.Cache.Current.GetTypesOfTerminalCache(storeCode, registerNumber);
		//			terminalCacheTypeCount += cacheTypes.Count;

		//			// CacheType table
		//			Table cacheTypetable = new Table();
		//			cacheTypetable.CellSpacing = 0;
		//			cacheTypetable.CellPadding = 3;
		//			cacheTypetable.BorderWidth = 1;
		//			cacheTypeCell.Controls.Add(cacheTypetable);

		//			TableHeaderRow cacheTypeHeaderRow = new TableHeaderRow();
		//			TableHeaderCell cacheTypeHeaderCell = new TableHeaderCell();
		//			cacheTypeHeaderCell.Text = "Type";
		//			cacheTypeHeaderRow.Cells.Add(cacheTypeHeaderCell);
		//			cacheTypetable.Rows.Add(cacheTypeHeaderRow);

		//			foreach (CacheTypeEnum cacheType in cacheTypes)
		//			{
		//				TableCell cacheTypeValueCell = new TableCell();
		//				TableCell cacheItemCell = new TableCell();
		//				cacheTypeValueCell.BorderWidth = new Unit(1);
		//				cacheItemCell.BorderWidth = new Unit(1);

		//				cacheTypeValueCell.Text = cacheType.ToString();

		//				TableRow cacheTypeRow = new TableRow();
		//				cacheTypeRow.Cells.Add(cacheTypeValueCell);
		//				cacheTypeRow.Cells.Add(cacheItemCell);
		//				cacheTypetable.Rows.Add(cacheTypeRow);


		//				// CacheItem table
		//				Table cacheItemtable = new Table();
		//				cacheItemtable.CellSpacing = 0;
		//				cacheItemtable.CellPadding = 3;
		//				cacheItemtable.BorderWidth = 1;
		//				cacheItemCell.Controls.Add(cacheItemtable);

		//				TableHeaderRow cacheItemHeaderRow = new TableHeaderRow();
		//				TableHeaderCell cacheKeyHeaderCell = new TableHeaderCell();
		//				TableHeaderCell cacheValueHeaderCell = new TableHeaderCell();
		//				cacheKeyHeaderCell.Text = "Key";
		//				cacheValueHeaderCell.Text = "Value";
		//				cacheItemHeaderRow.Cells.Add(cacheKeyHeaderCell);
		//				cacheItemHeaderRow.Cells.Add(cacheValueHeaderCell);
		//				cacheItemtable.Rows.Add(cacheItemHeaderRow);

		//				List<string> cacheKeys = Raymark.Caching.Cache.Current.GetKeysOfTerminalCache(storeCode, registerNumber, cacheType);
		//				terminalCacheItemCount += cacheKeys.Count;

		//				foreach (string cacheKey in cacheKeys)
		//				{
		//					TableCell cacheItemKeyCell = new TableCell();
		//					TableCell cacheItemValueCell = new TableCell();
		//					cacheItemKeyCell.BorderWidth = new Unit(1);
		//					cacheItemValueCell.BorderWidth = new Unit(1);

		//					cacheItemKeyCell.Text = cacheKey;
		//					object cacheValue = Raymark.Caching.Cache.Current.Get(storeCode, registerNumber, cacheType, cacheKey);
		//					cacheItemValueCell.Text = cacheValue == null ? "null" : cacheValue.ToString();

		//					TableRow cacheItemRow = new TableRow();
		//					cacheItemRow.Cells.Add(cacheItemKeyCell);
		//					cacheItemRow.Cells.Add(cacheItemValueCell);
		//					cacheItemtable.Rows.Add(cacheItemRow);
		//				}

		//			}
		//		}
		//	}

		//	terminalCacheHolder.Controls.Add(storetable);

		//}

		//terminalCacheCountLtl.Text = string.Format(@"<div style='color:blue;'>Store Count: {0}<br />Terminal Count: {1}<br />Type Count: {2}<br />Item Count: {3}</div>", terminalCacheStoreCount, terminalCacheTerminalCount, terminalCacheTypeCount, terminalCacheItemCount);


	}


}