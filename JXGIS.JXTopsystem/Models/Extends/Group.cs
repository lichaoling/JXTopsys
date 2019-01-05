using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models
{
    public class Group
    {
        public string PId { get; set; }

        public List<Item> Children { get; set; }
    }

    public class Item
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PId { get; set; }
        public List<Item> Children { get; set; }

        public object Tag { get; set; }

        public static List<Item> GetTree(List<Item> items)
        {
            var group = (from x in items
                         group x by x.PId into g
                         select new Group
                         {
                             PId = g.Key,
                             Children = g.ToList()
                         }).ToList();

            var root = group.Where(p => string.IsNullOrEmpty(p.PId)).FirstOrDefault();

            if (root != null)
            {
                loop(root.Children, group);
            }
            return root != null ? root.Children : null;
        }

        private static void loop(List<Item> items, List<Group> groups)
        {
            if (items != null && items.Count != 0)
            {
                foreach (var item in items)
                {
                    var group = groups.Where(g => g.PId == item.Id).FirstOrDefault();
                    if (group != null)
                    {
                        item.Children = group.Children;
                        loop(item.Children, groups);
                    }
                }
            }
        }
    }
}