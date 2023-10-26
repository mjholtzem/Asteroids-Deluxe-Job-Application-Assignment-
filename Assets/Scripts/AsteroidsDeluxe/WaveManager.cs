using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;
using System;

namespace AsteroidsDeluxe
{
    public class WaveManager : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private float _waveStartDelay;
        [SerializeField] private int _minAsteroidCount = 4;
        [SerializeField] private int _maxAsteroidCount = 8;
        [SerializeField] private int _waveNumberForMaxAsteroidCount;

        [Header("References")]
        [SerializeField] private Asteroid _asteroidPrefabLarge;

        private Camera _camera;
        private List<Asteroid> _asteroids = new();
        private int _waveCount = 0;

		private void Start()
		{
            _camera = Camera.main;
            Dispatch.Listen<AsteroidDestroyedMessage>(OnAsteroidDestroyed);
            SpawnWave();
		}

		private void OnDestroy()
		{
			Dispatch.Unlisten<AsteroidDestroyedMessage>(OnAsteroidDestroyed);
		}

		public Asteroid SpawnAsteroid(Asteroid asteroidPrefab, Vector2 position)
        {
            var asteroid = Instantiate(asteroidPrefab, position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
            _asteroids.Add(asteroid);
            return asteroid;
        }

        private void OnAsteroidDestroyed(AsteroidDestroyedMessage message)
        {
            _asteroids.Remove(message.asteroid);

            if(_asteroids.Count == 0)
            {
				Debug.Log($"Wave {_waveCount} is COMPLETE!!!");
				SpawnWave();
            }
        }

        private async void SpawnWave()
        {
            await Task.Delay(TimeSpan.FromSeconds(_waveStartDelay));

            _waveCount++;

			Debug.Log($"Wave {_waveCount} is starting!!!");
            Dispatch.Fire(new WaveStartedMessage { waveCount = _waveCount });

			var t = Mathf.InverseLerp(1, _waveNumberForMaxAsteroidCount, _waveCount);
            var asteroidCount = Mathf.RoundToInt(Mathf.Lerp(_minAsteroidCount, _maxAsteroidCount, t));

            //Create Spawn Zones
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

            for(int i = 0; i < asteroidCount; i++)
            {
                var spawnZone = spawnZones[i % spawnZones.Count];
                var asteroid = SpawnAsteroid(
                    _asteroidPrefabLarge, 
                    new Vector2(Random.Range(spawnZone.min.x, spawnZone.max.x), Random.Range(spawnZone.min.y, spawnZone.max.y)));

                //for these starter asteroids spawning in on the edges. We want to make sure they are more or less drifting towards the center of the screen
                //within reason. This is not the most elegant solution but we will just keep re-rolling velocity until we get a good trajectory

                var directionToCenter = (Vector2.zero - (Vector2)asteroid.transform.localPosition).normalized;
                while(Vector2.Dot(asteroid.Velocity.normalized, directionToCenter) < .7f)
                {
                    asteroid.RandomizeVelocity();
                }
            }
		}
    }
}
