using System;
using Chess3D.Runtime.Online;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Chess3D.Runtime
{
    public sealed class ConfigContainer : ILoadUnit
    {
        public LobbyConfig LobbyConfig;
        public ChessConfig ChessConfig;
        public OnlineConfig OnlineConfig;

        public UniTask Load()
        {
            var asset = AssetService.R.Load<TextAsset>(RuntimeConstants.Configs.FileName);
            JsonConvert.PopulateObject(asset.text, this);
            return UniTask.CompletedTask;
        }
    }

    [Serializable]
    public sealed class LobbyConfig
    {
        public float UpdateIntervalSec;
        public int PlayersToStart;
    }

    [Serializable]
    public sealed class OnlineConfig
    {
        public ConnectionType ConnectionType;
        public AuthorityType AuthorityType;
        public bool IsLocal;
    }

    [Serializable]
    public sealed class ChessConfig
    {
        public float WarmupTimeSec;
        public float UndoMoveTimeSec;
        public float RedoMoveTimeSec;
        public float MoveTimeSec;
    }
}