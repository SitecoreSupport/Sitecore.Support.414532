namespace Sitecore.Support.Mvc.Pipelines.Response.BuildPageDefinition
{

using Sitecore.Mvc.Configuration;
using Sitecore.Mvc.Extensions;
using Sitecore.Mvc.Pipelines.Response.BuildPageDefinition;
using Sitecore.Mvc.Presentation;
using Sitecore.Support.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.Xml.Linq;


    public class ProcessXmlBasedLayoutDefinition: Sitecore.Mvc.Pipelines.Response.BuildPageDefinition.ProcessXmlBasedLayoutDefinition
    {

        protected override IEnumerable<Rendering> GetRenderings(XElement layoutDefinition, BuildPageDefinitionArgs args)
        {
            XmlBasedRenderingParser parser = MvcSettings.GetRegisteredObject<XmlBasedRenderingParser>();
            foreach (XElement deviceNode in layoutDefinition.Elements("d"))
            {
                Guid deviceId = deviceNode.GetAttributeValueOrEmpty("id").ToGuid();
                Guid layoutId = deviceNode.GetAttributeValueOrEmpty("l").ToGuid();
                yield return GetRendering(deviceNode, deviceId, layoutId, "Layout", parser);
                foreach (XElement item in deviceNode.Elements("r"))
                {
                    yield return GetRendering(item, deviceId, layoutId, item.Name.LocalName, parser);
                }
            }
        }
    }
}