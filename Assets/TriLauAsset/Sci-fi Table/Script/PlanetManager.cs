using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using MyRule.Audio;

namespace MyRule
{
    public class PlanetManager : Singleton<PlanetManager>
    {
        [SerializeField] private List<Planet> planets;
        [SerializeField] private CinemachineCamera planetCam;
        [SerializeField] private GameObject holotableSuggest;
        [SerializeField] private GameObject planetSugget;
        public Planet planetTargetd;

        public bool isPlanetShow;

        // ===================== INPUT =====================

        public void Interact()
        {
            if (!planetTargetd.canInteract)
            {
                AudioManager.Instance.PlaySound("PlannetError");
                return;
            }

            if (isPlanetShow) return;
            if (planetTargetd == null) return;
            if (!ScifiMouseController.instance.isOnPlanet) return;

            HidePlanets(planetTargetd);
            planetTargetd.ShowDetailPlanet();

            ScifiMouseController.instance?.LockMouse();

            ShowPlanetCinemachineCam();

            holotableSuggest.SetActive(false);
            planetSugget.SetActive(true);

            isPlanetShow = true;
        }

        public void Escape()
        {
            if (!isPlanetShow) return;

            ShowAllPlanets();

            if (planetTargetd != null)
                planetTargetd.HideDetailPlanet();

            ScifiMouseController.instance?.UnlockMouse();

            HideCinemachineCam();

            holotableSuggest?.SetActive(true);
            planetSugget?.SetActive(false);

            planetTargetd = null;
            isPlanetShow = false;
        }

        // ===================== PLANET =====================

        public void ShowAllPlanets()
        {
            foreach (var planet in planets)
            {
                if (planet != null)
                    planet.gameObject.SetActive(true);
            }
        }

        public void HidePlanets(Planet planet)
        {
            foreach (var p in planets)
            {
                if (p != null && p.planetSO.planetType != planet.planetSO.planetType)
                    p.gameObject.SetActive(false);
            }
        }

        // ===================== CAMERA =====================

        public void ShowPlanetCinemachineCam()
        {
            planetCam.gameObject.SetActive(true);

            planetCam.Follow = planetTargetd.transform;
            planetCam.LookAt = planetTargetd.transform;
        }

        public void HideCinemachineCam()
        {
            planetCam.gameObject.SetActive(false);
        }
    }
}
