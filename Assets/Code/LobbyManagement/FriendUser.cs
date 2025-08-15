using System;
using UnityEngine;

namespace LobbyManagement
{
    public struct FriendUser : IEquatable<FriendUser>
    {
        public string Id {get; set;}
        public string Name {get; set;}
        public Texture2D Avatar {get; set;}

        public bool Equals(FriendUser other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return obj is FriendUser other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
