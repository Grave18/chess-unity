using System.Collections;
using ChessGame.Logic;
using TMPro;
using UnityEngine;

namespace Ui.Game
{
    public class ClockPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Initialization.Initialization initialization;

        [Header("Ui")]
        [SerializeField] private TMP_Text whiteText;
        [SerializeField] private TMP_Text blackText;

        private IEnumerator Start()
        {
            var wait = new WaitForSeconds(0.2f);
            IClock clock = initialization.Clock;

            yield return new WaitWhile (() => clock == null);

            while (Application.isPlaying)
            {
                whiteText.text = clock.WhiteTime.ToString(@"mm\:ss");
                blackText.text = clock.BlackTime.ToString(@"mm\:ss");

                yield return wait;
            }
        }
    }
}