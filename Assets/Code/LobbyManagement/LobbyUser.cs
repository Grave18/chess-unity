using System;
using UnityEngine;

namespace LobbyManagement
{
    public struct LobbyUser : IEquatable<LobbyUser>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Texture2D Image { get; set; }
        public bool IsReady { get; set; }

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