using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Quisy.WebScrapers.Helpers
{
    class HtmlNodeHelper
    {
        public static HtmlNode GetFirstByNameAndAttribute(IEnumerable<HtmlNode> nodes, string name, string attribute, string attributeValue)
        {
            return nodes.FirstOrDefault(d =>
                                    d.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) &&
                                    d.Attributes.Contains(attribute) &&
                                    d.Attributes[attribute].Value.Equals
                                                (attributeValue, StringComparison.InvariantCultureIgnoreCase));
        }

        public static HtmlNode GetFirstByNameAndClass(IEnumerable<HtmlNode> nodes, string name, string classValue)
        {
            return GetFirstByNameAndAttribute(nodes, name, "class", classValue);
        }

        public static HtmlNode GetFirstByName(IEnumerable<HtmlNode> nodes, string name)
        {
            return nodes.FirstOrDefault(d => d.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public static IEnumerable<HtmlNode> GetAllByNameAndAttribute(IEnumerable<HtmlNode> nodes, string name, string attribute, string attributeValue)
        {
            return nodes.Where(d =>
                                    d.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) &&
                                    d.Attributes.Contains(attribute) &&
                                    d.Attributes[attribute].Value.Equals
                                                (attributeValue, StringComparison.InvariantCultureIgnoreCase));
        }

        public static string GetFirstValueByNameAndAttribute(IEnumerable<HtmlNode> nodes, string name, string attribute)
        {
            var node = nodes.FirstOrDefault(d => d.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            return node?.Attributes.Contains(attribute) == true ? node.Attributes[attribute].Value : null;
        }
    }
}
