﻿using EliteMMO.API;
using MemoryAPI;
using MemoryAPI.Navigation;

namespace MemoryAPI
{
    public class Helpers
    {
        public static Position ToPosition(float x, float y, float z, float h)
        {
            var position = new Position
            {
                X = x,
                Y = y,
                Z = z,
                H = h
            };

            return position;
        }

        public static Status ToStatus(EntityStatus status)
        {
            switch (status)
            {
                case EntityStatus.Idle:
                    return MemoryAPI.Status.Standing;
                case EntityStatus.Engaged:
                    return MemoryAPI.Status.Fighting;
                case EntityStatus.Dead:
                    return MemoryAPI.Status.Dead1;
                case EntityStatus.DeadEngaged:
                    return MemoryAPI.Status.Dead2;
                case EntityStatus.Event:
                    return MemoryAPI.Status.Event;
                case EntityStatus.Healing:
                    return MemoryAPI.Status.Healing;
                default:
                    return MemoryAPI.Status.Unknown;
            }
        }
    }
}
