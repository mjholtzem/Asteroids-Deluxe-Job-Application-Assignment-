namespace AsteroidsDeluxe
{
    public class ObjectDestroyedMessage { public ObjectType type; };

    public class AsteroidDestroyedMessage { public Asteroid asteroid; }

    public class PlayerDestroyedMessage { }

    public class PointsAwardedMessage { public int totalPoints; public int pointsAwarded; }

    public class WaveStartedMessage { public int waveCount; }
}
