using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarForTheThrone.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _music;
        [SerializeField] private AudioSource _sound;

        [SerializeField] private AudioClip[] _clips;

        public enum Sounds
        {
            Attack,
            Arrow,
            Inspiration,
            Block,
            Damaged
        }

        public void PlaySound(Sounds sound)
        {
            _sound.PlayOneShot(_clips[(int)sound]);
        }

    }
}