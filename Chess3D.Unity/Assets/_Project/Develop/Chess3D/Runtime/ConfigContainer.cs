using System;
using System.Threading;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Chess3D.Runtime
{
    public sealed class ConfigContainer : ILoadUnit
    {
        public ChessConfig ChessConfig;

        public UniTask Load(CancellationToken cancellationToken = default)
        {
            var asset = AssetService.R.Load<TextAsset>(RuntimeConstants.Configs.FileName);
            JsonConvert.PopulateObject(asset.text, this);
            return UniTask.CompletedTask;
        }
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