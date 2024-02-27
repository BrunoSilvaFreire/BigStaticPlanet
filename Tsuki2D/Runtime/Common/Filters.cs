using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lunari.Tsuki2D.Common {
    [Serializable]
    public class Filters : Filter {
        [SerializeReference]
        public List<Filter> filters;

        public override bool Allowed(Collider2D collider) {
            if (filters == null || filters.Count == 0) {
                return true;
            }

            return filters.All(filter => filter.Allowed(collider));
        }
    }
}