using System;
using UnityEngine;

namespace Lunari.Tsuki2D.Common {
    [Serializable]
    public abstract class Filter {
        public abstract bool Allowed(Collider2D collider);
    }
}