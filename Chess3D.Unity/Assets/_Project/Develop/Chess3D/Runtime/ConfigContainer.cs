using System;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Chess3D.Runtime
{
    public sealed class ConfigContainer : ILoadUnit
    {
        public LobbyConfig LobbyConfig;

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
}