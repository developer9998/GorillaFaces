using GorillaFaces.Models;
using UnityEngine;

namespace GorillaFaces.Behaviours
{
    public class PhysicalFace : MonoBehaviour
    {
        public static GorillaFace Face;
        public static Material Material;

        private float _overrideDuration = 0;
        private bool _currentEyeOverride;
        private int _currentFlipbookIndex;

        private Renderer _renderer;
        private GorillaSpeakerLoudness _speakerLoudness;

        private ShaderHashId _eyeOverride = "_EyeOverride", _mouthUV = "_MouthUV";

        private readonly MouthCoordinate[] _mouthCoordinates = 
        [
            new()
            {
                Loudness = new Vector2(-1f, 0.001f),
                Location = new Vector2(Constants.FirstColumn, Constants.TopRow),
                Duration = 1f
            },
            new()
            {
                Loudness = new Vector2(0.001f, 0.02f),
                Location = new Vector2(Constants.SecondColumn, Constants.TopRow),
                Duration = 1f
            },
            new()
            {
                Loudness = new Vector2(0.02f, 0.0325f),
                Location = new Vector2(Constants.ThirdColumn, Constants.TopRow),
                Duration = 1f
            },
            new()
            {
                Loudness = new Vector2(0.0325f, 0.0425f),
                Location = new Vector2(Constants.FourthColumn, Constants.TopRow),
                Duration = 1f
            },
            new()
            {
                Loudness = new Vector2(0.0425f, 0.055f),
                Location = new Vector2(Constants.FirstColumn, Constants.MiddleRow),
                Duration = 1f
            },
            new()
            {
                Loudness = new Vector2(0.055f, 0.075f),
                Location = new Vector2(Constants.SecondColumn, Constants.MiddleRow),
                Duration = 1f
            },
            new()
            {
                Loudness = new Vector2(0.075f, 0.09f),
                Location = new Vector2(Constants.ThirdColumn, Constants.MiddleRow),
                Duration = 1f
            },
            new()
            {
                Loudness = new Vector2(0.09f, 0.1125f),
                Location = new Vector2(Constants.FourthColumn, Constants.MiddleRow),
                Duration = 1f
            },
            new()
            {
                Loudness = new Vector2(0.1125f, 0.175f),
                Location = new Vector2(Constants.FirstColumn, Constants.BottomRow),
                Duration = 1f
            },
            new()
            {
                Loudness = new Vector2(0.175f, 1f),
                Location = new Vector2(Constants.SecondColumn, Constants.BottomRow),
                Duration = 1f
            }
        ];

        private readonly MouthCoordinate _mutedCoordinate = new()
        {
            Loudness = Vector2.zero,
            Location = new Vector2(Constants.ThirdColumn, Constants.BottomRow),
            Duration = 1f
        };

        public void Start()
        {
            _renderer = GetComponent<VRRig>().headMesh.transform.Find("gorillaface").GetComponent<Renderer>();
            _speakerLoudness = GetComponent<GorillaSpeakerLoudness>();

            Events.ApplyFace += UpdateFace;

            if (Material) _renderer.material = new Material(Material);
        }

        public void Update()
        {
            if (Face == null) return;

            CheckEyes();
            UpdateEyes();

            float volume = _speakerLoudness.Loudness;
            CheckMouth(volume);

            MouthCoordinate activeCoordinate = _speakerLoudness.IsMicEnabled ? _mouthCoordinates[_currentFlipbookIndex] : _mutedCoordinate;
            UpdateMouth(activeCoordinate);
        }

        public void UpdateFace(GorillaFace face)
        {
            _renderer.material.SetTexture("_Base", face.Base);
            _renderer.material.SetTexture("_Eye", face.EyeSheet);
            _renderer.material.SetTexture("_Mouth", face.MouthSheet);
        }

        public void CheckEyes()
        {
            if (_speakerLoudness.IsSpeaking && _speakerLoudness.Loudness > Constants.ScreamVolume)
            {
                _currentEyeOverride = true;
                _overrideDuration = Constants.ScreamDuration;
                return;
            }
            if (_currentEyeOverride)
            {
                _overrideDuration -= Time.deltaTime;
                if (_overrideDuration < 0f) _currentEyeOverride = false;
            }
        }

        public void UpdateEyes()
        {
            _renderer.material.SetFloat(_eyeOverride, _currentEyeOverride ? 1f : 0f);
        }

        public void CheckMouth(float volume)
        {
            int num = _mouthCoordinates.Length - 1;
            while (num >= 0 && volume < _mouthCoordinates[num].Loudness.y)
            {
                if (volume > _mouthCoordinates[num].Loudness.x)
                {
                    if (_currentFlipbookIndex != num)
                    {
                        _currentFlipbookIndex = num;
                        break;
                    }
                    break;
                }
                else
                {
                    num--;
                }
            }
        }

        public void UpdateMouth(MouthCoordinate coordinate)
        {
            Vector2 currentLocation = coordinate.Location;
            _renderer.material.SetVector(_mouthUV, new Vector4(1f / 2f, 2f / 3f, currentLocation.x, currentLocation.y));
        }
    }
}
