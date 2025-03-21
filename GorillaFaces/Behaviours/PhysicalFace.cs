using GorillaFaces.Models;
using UnityEngine;

namespace GorillaFaces.Behaviours;

public class PhysicalFace : MonoBehaviour
{
	public static GorillaFace Face;

	public static Material Material;

	private float _overrideDuration = 0f;

	private bool _currentEyeOverride;

	private int _currentFlipbookIndex;

	private Renderer _renderer;

	private GorillaSpeakerLoudness _speakerLoudness;

	 private ShaderHashId _eyeOverride = "_EyeOverride", _mouthUV = "_MouthUV";

	private readonly MouthCoordinate[] _mouthCoordinates = new MouthCoordinate[10]
	{
		new MouthCoordinate
		{
			Loudness = new Vector2(-1f, 0.001f),
			Location = new Vector2(0f, 1f / 3f),
			Duration = 1f
		},
		new MouthCoordinate
		{
			Loudness = new Vector2(0.001f, 0.02f),
			Location = new Vector2(0.25f, 1f / 3f),
			Duration = 1f
		},
		new MouthCoordinate
		{
			Loudness = new Vector2(0.02f, 0.0325f),
			Location = new Vector2(0.5f, 1f / 3f),
			Duration = 1f
		},
		new MouthCoordinate
		{
			Loudness = new Vector2(0.0325f, 0.0425f),
			Location = new Vector2(0.75f, 1f / 3f),
			Duration = 1f
		},
		new MouthCoordinate
		{
			Loudness = new Vector2(0.0425f, 0.055f),
			Location = new Vector2(0f, 0f),
			Duration = 1f
		},
		new MouthCoordinate
		{
			Loudness = new Vector2(0.055f, 0.075f),
			Location = new Vector2(0.25f, 0f),
			Duration = 1f
		},
		new MouthCoordinate
		{
			Loudness = new Vector2(0.075f, 0.09f),
			Location = new Vector2(0.5f, 0f),
			Duration = 1f
		},
		new MouthCoordinate
		{
			Loudness = new Vector2(0.09f, 0.1125f),
			Location = new Vector2(0.75f, 0f),
			Duration = 1f
		},
		new MouthCoordinate
		{
			Loudness = new Vector2(0.1125f, 0.175f),
			Location = new Vector2(0f, -1f / 3f),
			Duration = 1f
		},
		new MouthCoordinate
		{
			Loudness = new Vector2(0.175f, 1f),
			Location = new Vector2(0.25f, -1f / 3f),
			Duration = 1f
		}
	};

	private readonly MouthCoordinate _mutedCoordinate = new MouthCoordinate
	{
		Loudness = Vector2.zero,
		Location = new Vector2(0.5f, -1f / 3f),
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
		if (Face != null)
		{
			CheckEyes();
			UpdateEyes();
			float loudness = _speakerLoudness.Loudness;
			CheckMouth(loudness);
			MouthCoordinate coordinate = (_speakerLoudness.IsMicEnabled ? _mouthCoordinates[_currentFlipbookIndex] : _mutedCoordinate);
			UpdateMouth(coordinate);
		}
	}

	public void UpdateFace(GorillaFace face)
	{
		_renderer.material.SetTexture("_Base", (Texture)(object)face.Base);
		_renderer.material.SetTexture("_Eye", (Texture)(object)face.EyeSheet);
		_renderer.material.SetTexture("_Mouth", (Texture)(object)face.MouthSheet);
	}

	public void CheckEyes()
	{
		if (_speakerLoudness.IsSpeaking && _speakerLoudness.Loudness > 0.2f)
		{
			_currentEyeOverride = true;
			_overrideDuration = 0.5f;
		}
		else if (_currentEyeOverride)
		{
			_overrideDuration -= Time.deltaTime;
			if (_overrideDuration < 0f)
			{
				_currentEyeOverride = false;
			}
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
			num--;
		}
	}

	public void UpdateMouth(MouthCoordinate coordinate)
	{
		Vector2 currentLocation = coordinate.Location;
        _renderer.material.SetVector(_mouthUV, new Vector4(1f / 2f, 2f / 3f, currentLocation.x, currentLocation.y));
	}
}
