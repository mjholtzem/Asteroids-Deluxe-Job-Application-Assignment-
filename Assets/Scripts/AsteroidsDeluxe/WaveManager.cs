using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;
using System;
using System.Collections;

namespace AsteroidsDeluxe
{
    public class WaveManager : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private float _waveStartDelay;
		[SerializeField] private bool _requireEnemyClear = true;

        [Header("Config - Asteroids")]
        [SerializeField] private int _minAsteroidCount = 4;
        [SerializeField] private int _maxAsteroidCount = 8;
        [SerializeField] private int _waveNumberForMaxAsteroidCount;

		[Header("Config - Death Star")]
		[SerializeField] private int _maxDeathStarDelay = 15;
		[SerializeField] private int _minDeathStarDelay = 5;
		[SerializeField] private int _maxDeathStarInterval = 30;
		[SerializeField] private int _minDeathStarInterval = 10;
		[SerializeField] private int _waveNumberForMinDeathStarTime = 30;

		[Header("Config - Saucer")]
		[SerializeField] private int _maxSaucerDelay = 18;
		[SerializeField] private int _minSaucerDelay = 1;
		[SerializeField] private int _maxSaucerInterval = 20;
		[SerializeField] private int _minSaucerInterval = 2;
		[SerializeField] private int _waveNumberForMinSaucerTime = 30;
		[SerializeField][Range(0, 1)] private float _minSmallSaucerChance = .2f;
		[SerializeField] [Range(0, 1)] private float _maxSmallSaucerChance = 1f;
		[SerializeField] private int _waveNumberForMaxSaucerChance = 30;

		[Header("References")]
        [SerializeField] private Asteroid _asteroidPrefabLarge;
		[SerializeField] private DeathStar _deathStarPrefab;
		[SerializeField] private Saucer _saucerLargePrefab;
		[SerializeField] private Saucer _saucerSmallPrefab;

		private List<AsteroidsBehaviour> _asteroids = new();
		public List<AsteroidsBehaviour> Asteroids => _asteroids;
        private List<AsteroidsBehaviour> _enemies = new();
		public List<AsteroidsBehaviour> Enemies => _enemies;
        private int _waveCount = 0;
		private bool _isStartingWave = false;

        private float _deathStarSpawnTime = Mathf.Infinity;
		private float _saucerSpawnTime = Mathf.Infinity;

		private AsteroidsBehaviour _currentSaucer = null;

		private void Start()
		{
            Dispatch.Listen<ObjectDestroyedMessage>(OnObjectDestroyed);
		}

		private void OnDestroy()
		{
			Dispatch.Unlisten<ObjectDestroyedMessage>(OnObjectDestroyed);
		}

		private void Update()
		{
			if(_waveCount == 0) return;
			UpdateDeathStarSpawner();
			UpdateSaucerSpawner();
		}

		//Asteroid Spawning
		public Asteroid SpawnAsteroid(Asteroid asteroidPrefab, Vector2 position)
        {
            var asteroid = Instantiate(asteroidPrefab, position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
            _asteroids.Add(asteroid);
            return asteroid;
        }

        private void OnObjectDestroyed(ObjectDestroyedMessage message)
        {
			if(_waveCount == 0) return;

            if(message.DestroyedType == ObjectType.AsteroidLarge
                || message.DestroyedType == ObjectType.AsteroidMedium
                || message.DestroyedType == ObjectType.AsteroidSmall)
            {
				_asteroids.Remove(message.destroyedObject);
			}
            else if(message.DestroyedType == ObjectType.DeathStar
                || message.DestroyedType == ObjectType.ChaserLarge
                || message.DestroyedType == ObjectType.ChaserSmall)
            {
                _enemies.Remove(message.destroyedObject);
            }
			else if(message.DestroyedType == ObjectType.SaucerLarge
				|| message.DestroyedType == ObjectType.SaucerSmall)
            {
				_currentSaucer = null;
				SetSaucerSpawnTime(false);
            }

            if(_asteroids.Count == 0 && (_enemies.Count == 0 || _requireEnemyClear == false))
            {
				Debug.Log($"Wave {_waveCount} is COMPLETE!!!");
				StartCoroutine(SpawnWave());
            }
        }

        public IEnumerator SpawnWave()
        {
			if(_isStartingWave) yield break;
			_isStartingWave = true;

			_waveCount++;
			Debug.Log($"Wave {_waveCount} is starting!!!");
			Dispatch.Fire(new WaveStartedMessage { waveCount = _waveCount });

			yield return new WaitForSeconds(_waveStartDelay);

			var t = Mathf.InverseLerp(1, _waveNumberForMaxAsteroidCount, _waveCount);
            var asteroidCount = Mathf.RoundToInt(Mathf.Lerp(_minAsteroidCount, _maxAsteroidCount, t));

            //Create Spawn Zones
            var spawnZones = CalculateSpawnZones();

            for(int i = 0; i < asteroidCount; i++)
            {
                var spawnZone = spawnZones[i % spawnZones.Count];
                var asteroid = SpawnAsteroid(
                    _asteroidPrefabLarge, 
                    new Vector2(Random.Range(spawnZone.min.x, spawnZone.max.x), Random.Range(spawnZone.min.y, spawnZone.max.y)));

                //for these starter asteroids spawning in on the edges. We want to make sure they are more or less drifting towards the center of the screen
                //within reason. This is not the most elegant solution but we will just keep re-rolling velocity until we get a good trajectory

                var directionToCenter = (Vector2.zero - (Vector2)asteroid.transform.localPosition).normalized;
                while((Vector2.Dot(asteroid.Velocity.normalized, directionToCenter) < .7f) && Application.isPlaying)
                {
                    asteroid.RandomMovementController.RandomizeVelocity();
                }
            }

            SetDeathStarSpawnTime(true);
			SetSaucerSpawnTime(true);
			_isStartingWave = false;
		}

		public void DeInit()
        {
			_waveCount = 0;

			foreach(var asteroid in _asteroids) Destroy(asteroid.gameObject);
			_asteroids.Clear();

			foreach(var enemy in _enemies) Destroy(enemy.gameObject);
			_enemies.Clear();

			if(_currentSaucer) Destroy(_currentSaucer.gameObject);
        }
		
		//Death Star Spawning
		private void UpdateDeathStarSpawner()
        {
            if(_asteroids.Count == 0) return;

            if(Time.time < _deathStarSpawnTime) return;

            //Spawn Death Star
            var spawnZones = CalculateSpawnZones();
			var spawnZone = spawnZones[Random.Range(0, spawnZones.Count)];
			SpawnDeathStar(new Vector2(Random.Range(spawnZone.min.x, spawnZone.max.x), Random.Range(spawnZone.min.y, spawnZone.max.y)));
			SetDeathStarSpawnTime(false);
		}

        private void SetDeathStarSpawnTime(bool newWave)
        {
            var t = Mathf.InverseLerp(1, _waveNumberForMinDeathStarTime, _waveCount);
            var delay = newWave 
                ? Mathf.Lerp(_maxDeathStarDelay, _minDeathStarDelay, t) 
                : Mathf.Lerp(_maxDeathStarInterval, _minDeathStarInterval, t);

            _deathStarSpawnTime = Time.time + delay;
		}

		private DeathStar SpawnDeathStar(Vector2 position)
		{
			var deathStar = Instantiate(_deathStarPrefab, position, Quaternion.Euler(0, 0, Random.Range(0, 360)));

			//register root and all children recursively
			_enemies.Add(deathStar);

			foreach(var child in deathStar.Children)
			{
				RegisterChaserChildren(child);
			}

			var directionToCenter = (Vector2.zero - (Vector2)deathStar.transform.localPosition).normalized;
			while(Vector2.Dot(deathStar.Velocity.normalized, directionToCenter) < .7f)
			{
				deathStar.RandomDrift.RandomizeVelocity();
			}

			return deathStar;
		}

		private void RegisterChaserChildren(Chaser chaser)
		{
			_enemies.Add(chaser);
			foreach(var child in chaser.Children)
			{
				RegisterChaserChildren(child);
			}
		}

		//Saucer Spawning
		private void UpdateSaucerSpawner()
		{
			if(_asteroids.Count == 0) return;

			if(Time.time < _saucerSpawnTime || _currentSaucer != null) return;

			//Spawn Death Star
			var spawnZones = CalculateSpawnZones();
			var spawnZone = spawnZones[Random.Range(0, spawnZones.Count)];
			SpawnSaucer(new Vector2(Random.Range(spawnZone.min.x, spawnZone.max.x), Random.Range(spawnZone.min.y, spawnZone.max.y)));
		}
		private void SetSaucerSpawnTime(bool newWave)
		{
			var t = Mathf.InverseLerp(1, _waveNumberForMinSaucerTime, _waveCount);
			var delay = newWave
				? Mathf.Lerp(_maxSaucerDelay, _minSaucerDelay, t)
				: Mathf.Lerp(_maxSaucerInterval, _minSaucerInterval, t);

			_saucerSpawnTime = Time.time + delay;
		}

		private Saucer SpawnSaucer(Vector2 position)
		{
			var t = Mathf.InverseLerp(1, _waveNumberForMaxSaucerChance, _waveCount);
			var smallSaucerChance = Mathf.Lerp(
				_minSmallSaucerChance, 
				_maxSmallSaucerChance,
				t);
			var saucerPrefab = Random.value < smallSaucerChance ? _saucerSmallPrefab : _saucerLargePrefab;
			var saucer = Instantiate(saucerPrefab, position, Quaternion.identity);

			//register root and all children recursively
			_currentSaucer = saucer;

			return saucer;
		}

		private List<(Vector2 min, Vector2 max)> CalculateSpawnZones()
		{
			var spawnZones = new List<(Vector2 min, Vector2 max)>();
			var cameraBounds = GameManager.Instance.ScreenWrapManager.CameraBounds;

			//we will to a round robin on all the edges and spawn asteroids in random postitions along them
			//spawn zones are min xy and max xy

			//left edge
			spawnZones.Add((
				new Vector2(cameraBounds.min.x, cameraBounds.min.y),
				new Vector2(cameraBounds.min.x, cameraBounds.max.y)));
			//top edge
			spawnZones.Add((
			   new Vector2(cameraBounds.min.x, cameraBounds.max.y),
			   new Vector2(cameraBounds.max.x, cameraBounds.max.y)));
			//right edge
			spawnZones.Add((
			   new Vector2(cameraBounds.max.x, cameraBounds.min.y),
			   new Vector2(cameraBounds.max.x, cameraBounds.max.y)));
			//bot edge
			spawnZones.Add((
			   new Vector2(cameraBounds.min.x, cameraBounds.min.y),
			   new Vector2(cameraBounds.max.x, cameraBounds.min.y)));

			return spawnZones;
		}
	}
}
