using System;
using UnityEngine;

namespace LobbyManagement
{
    public struct LobbyUser : IEquatable<LobbyUser>
    {
        public string Id;
        public string DisplayName;
        public bool IsReady;
        public Texture2D Avatar;

        public bool Equals(LobbyUser other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return obj is LobbyUser other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }
    }
}