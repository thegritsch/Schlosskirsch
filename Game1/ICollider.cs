using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperG
{
    public interface ICollider
    {
        bool CheckCollision(ICollider collider);

        Rectangle GetBoundingBox();
    }
}
