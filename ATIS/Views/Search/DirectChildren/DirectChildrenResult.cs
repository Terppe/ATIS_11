using System;
using System.Collections.Generic;
using System.Text;
using ATIS.Core.Taxonomy;

namespace ATIS.Views.Search.DirectChildren;

public sealed class DirectChildrenResult
{
    public TaxonLevel Level { get; set; }
    public IList<object> Items { get; set; } = new List<object>();

    public bool HasItems => Items.Count > 0;
}
