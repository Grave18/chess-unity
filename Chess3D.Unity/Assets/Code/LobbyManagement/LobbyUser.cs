using UnityEngine;

namespace LobbyManagement
{
    public class LobbyUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Texture2D Image { get; set; }
        public bool IsReady { get; set; }
    }
}