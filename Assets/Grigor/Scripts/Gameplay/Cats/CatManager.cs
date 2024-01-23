using System.Collections.Generic;
using CardboardCore.DI;
using Grigor.Data;
using Grigor.Gameplay.Interacting;
using Grigor.Gameplay.World.Components;
using Grigor.Utils;
using UnityEngine;

namespace Grigor.Gameplay.Cats
{
    [Injectable]
    public class CatManager : CardboardCoreBehaviour
    {
        private readonly List<CatInteractable> allCats = new();
        private bool isCatnipPlaced;
        private GameConfig gameConfig;
        private Catnip currentCatnip;

        public bool IsCatnipPlaced => isCatnipPlaced;

        protected override void OnInjected()
        {
            gameConfig = GameConfig.Instance;
        }

        protected override void OnReleased()
        {

        }

        public void RegisterCat(CatInteractable catInteractable)
        {
            if (allCats.Contains(catInteractable))
            {
                return;
            }

            allCats.Add(catInteractable);
        }

        public void UnregisterCat(CatInteractable catInteractable)
        {
            if (!allCats.Contains(catInteractable))
            {
                return;
            }

            allCats.Remove(catInteractable);
        }

        public void OnCatnipPlaced(Vector3 position, Vector3 lookDirection)
        {
            if (isCatnipPlaced)
            {
                return;
            }

            isCatnipPlaced = true;

            currentCatnip = PlaceCatnip(position, lookDirection);

            Helper.Delay(gameConfig.CatnipReactionDelay, delegate { AlertCatsWithCatnip(position, lookDirection); });
        }

        private void AlertCatsWithCatnip(Vector3 position, Vector3 lookDirection)
        {
            if (currentCatnip == null)
            {
                return;
            }

            TrySpawnNewCatBehindPlayer(position, lookDirection);

            allCats.ForEach(cat => cat.OnCatnipPlaced(currentCatnip.transform));
        }

        private Catnip PlaceCatnip(Vector3 position, Vector3 lookDirection)
        {
            Catnip catnip = Instantiate(gameConfig.CatnipPrefab, position, Quaternion.identity);

            catnip.transform.SetParent(transform);

            catnip.ThrowCatnip(lookDirection, gameConfig.CatnipUpwardsForceWhenThrown);

            return catnip;
        }

        private void TrySpawnNewCatBehindPlayer(Vector3 position, Vector3 lookDirection)
        {
            float random = Random.Range(0f, 1f);

            if (random > gameConfig.ChanceForCatToSpawnNearCatnip)
            {
                return;
            }

            Vector3 catPosition = position - ((gameConfig.CatnipRadius - 1f) * lookDirection);

            Interactable cat = Instantiate(gameConfig.CatPrefab, catPosition, Quaternion.identity);

            cat.transform.SetParent(transform);

            cat.EnableInteractable();
        }

        public void OnCatnipRemoved()
        {
            if (!isCatnipPlaced)
            {
                return;
            }

            isCatnipPlaced = false;

            currentCatnip.DestroyCatnip();

            allCats.ForEach(cat => cat.OnCatnipRemoved());
        }
    }
}
