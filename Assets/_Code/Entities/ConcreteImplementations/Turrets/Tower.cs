
using SolarStorm.UnityToolkit;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Pool;

namespace SolarStorm.Entities
{
    // TODO Create attribute:
    //[RequireInterface(typeof(ITargetSensor))]
    //[RequireInterface(typeof(IWeapon))]
    public class Tower : MonoBehaviour
    {
        #region Variables

        // Inspector fields
        [Header("Turret Levels")]
        [field: SerializeField] public int Level { get; private set; }
        [SerializeField] private TurretLevelData[] levelData;

        [Header("Targeting")]
        [Tooltip("How many degrees away from the target a turret needs to be before it starts shooting")]
        [SerializeField] private FloatRef aimShootThreshold;
        [SerializeField] ITargetSensor enemySensor;
        [SerializeField] ITurretWeapon turretWeapon;

        [SerializeField] private Transform horizontalTR;
        [SerializeField] private Transform verticalTR;

        // C# fields
        public event Action<TurretLevelData> OnLevelChange;

        public TurretLevelData CurrentLevelData => levelData[Level];
        public GameObject Target { get; private set; }

        private float _angleFromTarget;
        private CancellationTokenSource _attackHandle;

        #endregion


        #region Unity Messages

        private void Awake()
        {
            if (!TryGetComponent(out enemySensor)) throw new MissingComponentException($"Turret {name} is missing a necessary {nameof(ITargetSensor)} component");
            if (!TryGetComponent(out turretWeapon)) throw new MissingComponentException($"Turret {name} is missing a necessary {nameof(ITurretWeapon)} component");
        }

        protected void Update()
        {
            // If the target goes out of range, unset it
            if (Target != null)
            {
                if (Vector3.Distance(Target.transform.position, transform.position) > CurrentLevelData.maxTargetingDistance)
                {
                    CancelAttack();
                    Target = null;
                }
            }

            // Then, if there is no target, get the closest one and set it as the target
            if (Target == null)
            {
                Target = enemySensor.GetTarget();
            }

            _angleFromTarget = Aim(CurrentLevelData.turnSpeed);
            if (_angleFromTarget < aimShootThreshold)
            {
                if (_attackHandle == null)
                {
                    _attackHandle = new CancellationTokenSource();
                    AttackTarget(_attackHandle.Token);
                }
            }
            else if (_attackHandle != null)
            {
                CancelAttack();
            }

            // TODO TEMP
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                LevelUp();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Level = Mathf.Max(Level - 1, 0);
                OnLevelChange?.Invoke(CurrentLevelData);
            }
            // END TEMP
        }

        #endregion


        public void LevelUp()
        {
            Level = Mathf.Min(Level + 1, levelData.Length - 1);
            OnLevelChange?.Invoke(levelData[Level]);
        }


        private float Aim(float turnSpeed)
        {
            if (Target != null)
            {
                //Horizontal
                Vector3 dirH = Target.transform.position - horizontalTR.position;
                Quaternion rotatingSideRot = Quaternion.RotateTowards(horizontalTR.localRotation, Quaternion.LookRotation(dirH), turnSpeed);
                horizontalTR.localRotation = Quaternion.Euler(0, rotatingSideRot.eulerAngles.y, 0);

                //Vertical
                Vector3 dirV = Target.transform.position - verticalTR.position;
                Quaternion pitchingSideRot = Quaternion.RotateTowards(verticalTR.localRotation, Quaternion.LookRotation(dirV), turnSpeed);
                verticalTR.localRotation = Quaternion.Euler(pitchingSideRot.eulerAngles.x, 0, 0);

                Quaternion barrelTargetRotation = Quaternion.LookRotation(Target.transform.position - verticalTR.position);

                return Quaternion.Angle(Quaternion.Euler(pitchingSideRot.eulerAngles.x, rotatingSideRot.eulerAngles.y, 0), barrelTargetRotation);
            }
            else
            {
                // Slow pan
                // TODO
                return 360;
            }
        }
        private async Awaitable AttackTarget(CancellationToken token)
        {
            while (Target != null && !token.IsCancellationRequested)
            {
                TurretLevelData data = levelData[Level];
                turretWeapon.Attack(Target, data);
                await Awaitable.WaitForSecondsAsync(60 / data.shotsPerMinute);
            }
        }
        private void CancelAttack()
        {
            if (_attackHandle != null)
            {
                _attackHandle.Cancel();
                _attackHandle.Dispose();
            }
            _attackHandle = null;
        }
    }
}