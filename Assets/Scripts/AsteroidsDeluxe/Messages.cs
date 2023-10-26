namespace AsteroidsDeluxe
{
    public class ObjectDestroyedMessage
    {
        public AsteroidsBehaviour destroyedObject;
        public AsteroidsBehaviour destroyedByObject;
        public ObjectType DestroyedType => destroyedObject?.ObjectType ?? ObjectType.None;
        public ObjectType DestroyedByType => destroyedByObject?.ObjectType ?? ObjectType.None;
	};

    public class PointsAwardedMessage { public int totalPoints; public int pointsAwarded; }

    public class WaveStartedMessage { public int waveCount; }
}
