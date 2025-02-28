using UnityEngine;

namespace Logic.Players
{
    public class Player : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private Game game;
        [SerializeField] private CommandInvoker commandInvoker;

        protected Game Game => game;
        protected CommandInvoker CommandInvoker => commandInvoker;

        public virtual void AllowMakeMove()
        {

        }

        public virtual void DisallowMakeMove()
        {

        }
    }
}