
using SolarStorm.UnityToolkit;
using System.Drawing;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace SolarStorm.Entities
{
    [RequireComponent(typeof(Tower))]
    public class TowerView : AbilityViewComponent
    {
        #region Variables

        [SerializeField] Animator _towerAnimator;
        [SerializeField] string[] _shootAnimationTriggerNames;

        private Renderer[] _emissiveRenderers;
        private Tower _tower;

        #endregion


        #region Unity Messages

        private void Awake()
        {
            if (_towerAnimator == null) if (!TryGetComponent(out _towerAnimator))
                {
                    throw new MissingComponentException($"TowerView found no animator on {name}");
                }

            _tower = GetComponent<Tower>();
            _emissiveRenderers = GetComponentsInChildren<Renderer>();
            _tower.OnLevelChange += OnLevelChange;
            _tower.GetComponent<ITurretWeapon>().OnShoot += OnShoot;
        }

        #endregion


        private void OnLevelChange(TurretLevelData level)
        {
            foreach (var rend in _emissiveRenderers)
            {
                foreach (var mat in rend.materials)
                {
                    mat.SetColor("_EmissionColor", level.emissionColor);
                }
            }
        }
        private void OnShoot(int index)
        {
            _towerAnimator.SetTrigger(_shootAnimationTriggerNames[index]);
        }


        #region IView Overrides

        public override void Show()
        {
            // Initialize level visuals
            OnLevelChange(_tower.CurrentLevelData);
            gameObject.SetActive(true);
        }

        #endregion
    }
}