namespace Sitecore.Support.Mvc.Presentation
{

using Sitecore.Data.Items;
using Sitecore.Layouts;
using Sitecore.Mvc.Extensions;
using Sitecore.Mvc.Presentation;
using Sitecore.Text;
using Sitecore.Web;
using System.Linq;


    public class NewXmlBasedRenderingParser: XmlBasedRenderingParser
    {
        protected override void AddRenderingItemProperties(Rendering rendering)
        {
            RenderingItem renderingItem = rendering.RenderingItem;
            if (renderingItem != null)
            {
                AddRenderingItemProperty(rendering, "DataSource", renderingItem.DataSource);
                AddRenderingItemProperty(rendering, "Placeholder", renderingItem.Placeholder);
                AddRenderingItemProperty(rendering, "Parameters", renderingItem.Parameters);
                if (!rendering.Caching.Cacheable)
                {
                    RenderingCaching caching = renderingItem.Caching;
                    rendering["Cacheable"] = caching.Cacheable.ToBoolString();
                    rendering["Cache_VaryByData"] = caching.VaryByData.ToBoolString();
                    rendering["Cache_VaryByDevice"] = caching.VaryByDevice.ToBoolString();
                    rendering["Cache_VaryByLogin"] = caching.VaryByLogin.ToBoolString();
                    rendering["Cache_VaryByParameters"] = caching.VaryByParm.ToBoolString();
                    rendering["Cache_VaryByQueryString"] = caching.VaryByQueryString.ToBoolString();
                    rendering["Cache_VaryByUser"] = caching.VaryByUser.ToBoolString();
                }
            }
        }

        protected new void AddRenderingItemProperty(Rendering rendering, string propertyName, string value)
        {
            #region patch 414532
           
            if (rendering.RenderingItem.Database.Name!="core" && rendering.RenderingItem.InnerItem.Template.InnerItem.Paths.Path.StartsWith("/sitecore/templates/System/Layout/Renderings/"))
            {
                if (propertyName == "Parameters" && value.ContainsText() && rendering.Properties["Parameters"].ContainsText())
                {
                    var standardValues = WebUtil.ParseUrlParameters(value);
                    var overridenValues = WebUtil.ParseUrlParameters(rendering["Parameters"]);
                    if (overridenValues.Count != standardValues.Count)
                    {
                        foreach (string key in standardValues.Keys)
                        {
                            if (overridenValues.AllKeys.Contains(key))
                                continue;
                            if (standardValues[key] != string.Empty)
                                overridenValues.Add(key, standardValues[key]);
                        }
                        rendering["Parameters"] = new UrlString(overridenValues).ToString();
                        return;
                    }
                }
            }
            #endregion
            if (!rendering.Properties[propertyName].ContainsText())
            {
                rendering[propertyName] = value;
            }
        }
    }
}