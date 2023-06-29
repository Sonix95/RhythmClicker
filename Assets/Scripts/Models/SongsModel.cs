using System;
using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    [Serializable]
    public class SongsModel
    {
        [SerializeField] private string _name;
        [SerializeField] private string _songResourcePath;
        [SerializeField] private List<float> _bits;

        public string Name => _name;
        public string SongResourcePath => _songResourcePath;
        public List<float> Bits => _bits;
    }
}
