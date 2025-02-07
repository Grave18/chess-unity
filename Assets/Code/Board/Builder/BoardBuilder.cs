﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Board.Pieces;
using EditorCools;
using Logic;
using Ui.Promotion;
using UnityEngine;

namespace Board.Builder
{
    public class BoardBuilder : MonoBehaviour
    {
        private const int Width = 8;
        private const int Height = 8;

        [Header("References")]
        [SerializeField] private GameManager gameManager;
        [SerializeField] private PromotionPanel promotionPanel;

        [SerializeField] private Transform whitePiecesParent;
        [SerializeField] private Transform blackPiecesParent;
        [SerializeField] private GameObject[] piecePrefabs;
        [SerializeField] private BoardPreset boardPreset;

        private readonly Dictionary<Square, GameObject> _whitePairs = new();
        private readonly Dictionary<Square, GameObject> _blackPairs = new();

        private PieceType _pieceType = PieceType.None;

        public GameObject[] PiecePrefabs => piecePrefabs;

        public void RequestPieceFromSelector(PieceColor pieceColor, Square square, Action<Piece> callback)
        {
            promotionPanel.Show(pieceColor);
            Time.timeScale = 0f;
            StartCoroutine(WaitForSelection());
            return;

            IEnumerator WaitForSelection()
            {
                yield return new WaitUntil(() => _pieceType != PieceType.None);

                Piece piece = GetPiece(_pieceType, pieceColor, square);
                callback?.Invoke(piece);

                // Reset state
                _pieceType = PieceType.None;
                Time.timeScale = 1f;
            }
        }

        public void Select(PieceType pieceType)
        {
            _pieceType = pieceType;
            promotionPanel.Hide();
        }

        public Piece GetPiece(PieceType pieceType, PieceColor pieceColor, Square square)
        {
            Transform piecesParent = pieceColor == PieceColor.White
                ? whitePiecesParent
                : blackPiecesParent;

            GameObject piecePrefab = pieceColor == PieceColor.White
                ? pieceType switch
                    {
                        // White
                        PieceType.Queen => piecePrefabs[10],
                        PieceType.Rook => piecePrefabs[11],
                        PieceType.Bishop => piecePrefabs[6],
                        PieceType.Knight => piecePrefabs[8],
                        PieceType.Pawn => piecePrefabs[9],
                        _ => null,
                    }
                : pieceType switch
                    {
                        // Black
                        PieceType.Queen => piecePrefabs[4],
                        PieceType.Rook => piecePrefabs[5],
                        PieceType.Bishop => piecePrefabs[0],
                        PieceType.Knight => piecePrefabs[2],
                        PieceType.Pawn => piecePrefabs[3],
                        _ => null,
                    };

            InstantiatePiece(piecesParent, piecePrefab, square, out Piece piece);
            return piece;
        }

        public void BuildBoard(out PieceColor turnColor)
        {
            string text = boardPreset.Preset;
            turnColor = boardPreset.TurnColor;

            text = ClearNewLineCharacters(text);

            if (IsPresetNotValid(text))
            {
                Debug.LogError("Invalid board preset");
                return;
            }

            DestroyAllPieces();

            for (int x = 0, y = 0; y < Height;)
            {
                int iText = x + y * Width;
                // Must invert y (Height - 1 - y) also x and y are swapped
                int iSquares = Height - 1 - y + x * Width;

                switch (text[iText])
                {
                    // Set prefabs for each square
                    case '*':
                    case ' ':
                        break;
                    case 'b':
                        _blackPairs[gameManager.Squares[iSquares]] = piecePrefabs[0]; // B_Bishop
                        break;
                    case 'k':
                        _blackPairs[gameManager.Squares[iSquares]] = piecePrefabs[1]; // B_King
                        break;
                    case 'n':
                        _blackPairs[gameManager.Squares[iSquares]] = piecePrefabs[2]; // B_Knight
                        break;
                    case 'p':
                        _blackPairs[gameManager.Squares[iSquares]] = piecePrefabs[3]; // B_Pawn
                        break;
                    case 'q':
                        _blackPairs[gameManager.Squares[iSquares]] = piecePrefabs[4]; // B_Queen
                        break;
                    case 'r':
                        _blackPairs[gameManager.Squares[iSquares]] = piecePrefabs[5]; // B_Rook
                        break;
                    case 'B':
                        _whitePairs[gameManager.Squares[iSquares]] = piecePrefabs[6]; // W_Bishop
                        break;
                    case 'K':
                        _whitePairs[gameManager.Squares[iSquares]] = piecePrefabs[7]; // W_King
                        break;
                    case 'N':
                        _whitePairs[gameManager.Squares[iSquares]] = piecePrefabs[8]; // W_Knight
                        break;
                    case 'P':
                        _whitePairs[gameManager.Squares[iSquares]] = piecePrefabs[9]; // W_Pawn
                        break;
                    case 'Q':
                        _whitePairs[gameManager.Squares[iSquares]] = piecePrefabs[10]; // W_Queen
                        break;
                    case 'R':
                        _whitePairs[gameManager.Squares[iSquares]] = piecePrefabs[11]; // W_Rook
                        break;
                    default:
                        Debug.LogError($"{text[iText]} is not a valid character");
                        return;
                }

                x += 1;

                if (x == Width)
                {
                    x = 0;
                    y += 1;
                }
            }

            InstantiatePieces();

            gameManager.FindAllPieces();
        }

        private static bool IsPresetNotValid(string text)
        {
            return text.Length != Width * Height;
        }

        private static string ClearNewLineCharacters(string text)
        {
            return text.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty);
        }

        private void InstantiatePieces()
        {
            InstantiatePieces(_whitePairs, whitePiecesParent);
            InstantiatePieces(_blackPairs, blackPiecesParent);
        }

        private void InstantiatePieces(Dictionary<Square, GameObject> pairs, Transform piecesParent)
        {
            foreach (var (square, piece) in pairs)
            {
                InstantiatePiece(piecesParent, piece, square, out _);
            }
        }

        private void InstantiatePiece(Transform piecesParent, GameObject piecePrefab, Square square, out Piece piece)
        {
            GameObject pieceInstance = Instantiate(piecePrefab, square.transform.position, piecePrefab.transform.rotation, piecesParent);
            piece = pieceInstance.GetComponent<Piece>();
            piece.GetSectionAndAlign(gameManager);
        }

        [Button(name: "Build Board", space: 10f)]
        [ContextMenu("Build Board")]
        private void GameManagerRestart()
        {
            gameManager.Restart();
        }

        [Button(space: 10f)]
        [ContextMenu("Destroy All Pieces")]
        private void DestroyAllPieces()
        {
            DestroyColorPieces(whitePiecesParent);
            DestroyColorPieces(blackPiecesParent);
            _whitePairs.Clear();
            _blackPairs.Clear();

            gameManager.ClearPieces();
        }

        private static void DestroyColorPieces(Transform parent)
        {
            Transform[] pieces = parent.Cast<Transform>().ToArray();
            foreach (Transform piece in pieces)
            {
                DestroyImmediate(piece.gameObject);
            }
        }
    }
}
