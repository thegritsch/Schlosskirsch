using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperG
{
    public enum ColliderType
    {
        player,
        playerBullet,
        enemy,
        enemyBullet,
        tree
    }

    public interface ICollider
    {
        bool CheckCollision(ICollider collider);

        Rectangle GetBoundingBox();

        ColliderType GetColliderType();
    }
}
