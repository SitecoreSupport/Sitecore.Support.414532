namespace Sitecore.Support
{

    using Sitecore.Events;
    using System;
    using Sitecore.Data.Items;
    using System.Collections.Generic;
    using Sitecore.Web;
    using Sitecore.Configuration;
    using System.Linq;

    public class ClearRenderingParametersCache
    {
        List<SiteInfo> list;
        public void ClearOnStandardValuesItemSaved(object sender, EventArgs args)
        {
            SitecoreEventArgs scArgs = args as SitecoreEventArgs;
            Item item = scArgs.Parameters[0] as Item;
            if (item != null && item.Paths.Path.StartsWith("/sitecore/templates") && item.ID == item.Template.StandardValues?.ID)
            {
                    list = (from si in Factory.GetSiteInfoList()
                            where si.RenderingParametersCache != null
                            select si).ToList();
                    if (list.Count > 0)
                    {
                        FindRendringAndClear(item);
                    }
              
            }

        }

        void FindRendringAndClear(Item item)
        {
            var stack = new Stack<Item>();
            stack.Push(item);
            while (stack.Any())
            {
                var next = stack.Pop();
                if (next.Template.InnerItem.Paths.Path.StartsWith("/sitecore/templates/System/Layout/Renderings/"))
                {
                    foreach (SiteInfo site in list)
                    {
                        site.RenderingParametersCache.RemoveKeysContaining(next.ID.ToString());
                    }
                    continue;
                }
                if (next.Paths.Path.StartsWith("/sitecore/templates"))
                foreach (var child in Globals.LinkDatabase.GetReferers(next).Select(x=>x.GetSourceItem()))
                {
                    if (child!=null && child.Template.ID != next.ID)
                        stack.Push(child);
                }
            }
        }
    }
}