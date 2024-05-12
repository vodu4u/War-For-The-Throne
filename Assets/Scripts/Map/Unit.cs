using UnityEngine;
using TMPro;
using DG.Tweening;
using System;
using UnityEngine.UI;
using WarForTheThrone.Info;
using System.Collections;

namespace WarForTheThrone.Map
{
    public class Unit : MonoBehaviour
    {
        private float _heightOffset = 0;
        private bool _direction = true;
        private int _health;
        private bool _inspiration;

        private UnitController _controller;
        [SerializeField] private Transform _unit;
        [SerializeField] private TextMeshProUGUI _txtHealth;
        [SerializeField] private Animator _animator;
        [SerializeField] private Image[] _paintElements;
        [SerializeField] private Image _weapon;

        [field: SerializeField] public UnitConfigSO Config { get; set; }
        public int Priority { get; set; }
        public int Owner { get; set; }
        public bool Inspiration 
        { 
            get => _inspiration; 
            set
            {
                _inspiration = value;
                if (value)
                {
                    _weapon.DOColor(Config.InspirationWeaponColor, 1f);
                }
                else
                {
                    _weapon.DOColor(Color.white, 1f);
                }
            }
        }

        public void Initialize(int owner, int priority, float heightOffset, UnitController controller, Color color)
        {
            Owner = owner;
            _controller = controller;
            Priority = priority;
            _heightOffset = heightOffset;
            foreach(var e in _paintElements)
            {
                e.color = color;
            }
        }

        public int Health
        {
            get => _health;
            set
            {
                _health = Mathf.Clamp(value, 0, Config.Health);
                _txtHealth.text = _health.ToString();
            }
        }
        public bool Direction
        {
            get => _direction;
            set
            {
                _direction = value;
                _unit.localScale = _direction ? Vector3.one : new Vector3(-1,1,1);
            }
        }

        public enum AnimationState
        {
            idle,
            moveing,
            dead
        }

        public enum AnimationClip
        {
            attack,
            takingDamage,
            inspiration
        }

        public enum DamageSource
        {
            melee,
            arrow
        }

        private void Start()
        {
            Health = Config.Health;
        }

        private IEnumerator DestroyAfterDelay()
        {
            yield return new WaitForSeconds(2f);
            Destroy(gameObject);
        }

        public void FacePosition(Vector3 position)
        {
            if (transform.position.x < position.x)
                Direction = true;
            else if (transform.position.x > position.x)
                Direction = false;
        }

        public void MoveToCell(Cell field, Action onEnd)
        {
            SetAnimation(AnimationState.moveing);
            onEnd += () => SetAnimation(AnimationState.idle);
            Vector3 position = new Vector3(field.transform.position.x,
                field.transform.position.y + _heightOffset, field.transform.position.z);
            transform.DOMove(position, 1).OnComplete(() => onEnd());
        }

        public int GetDamage()
        {
            int damage = Config.Damage;
            if (Inspiration)
                damage += 2;
            return damage;
        }

        public int Damage(int damage, DamageSource source)
        {
            if (Config.Type == UnitConfigSO.UnitType.Protector && source == DamageSource.arrow) damage = 1;
            Health -= damage;
            if (Health <= 0)
            {
                SetAnimation(AnimationState.dead);
                StartCoroutine(nameof(DestroyAfterDelay));
                _controller.UnitDied(this);
            }
            else
            {
                PlayAnimationOnce(AnimationClip.takingDamage);
            }
            return damage;
        }

        public void SetAnimation(AnimationState state)
        {
            if (state == AnimationState.moveing)
            {
                _animator.SetBool("moveing", true);
            }
            else if (state == AnimationState.idle)
            {
                _animator.SetBool("moveing", false);
            }
            else if (state == AnimationState.dead)
            {
                _animator.SetBool("dead", true);
            }
        }

        public void PlayAnimationOnce(AnimationClip clip)
        {
            string name = Config.Type + "_" + clip.ToString();
            _animator.Play(name);
        }
    }
}