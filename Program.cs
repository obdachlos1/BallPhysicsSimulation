using System;
using Leopotam.EcsLite;

namespace EcsBallSimulation
{
    public struct Position
    {
        public float X;
        public float Y;
    }

    public struct Velocity
    {
        public float X;
        public float Y;
    }

    public class BallMovementSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var posPool = world.GetPool<Position>();
            var velPool = world.GetPool<Velocity>();
            var filter = world.Filter<Position>().Inc<Velocity>().End();

            foreach (var entity in filter)
            {
                ref var position = ref posPool.Get(entity);
                ref var velocity = ref velPool.Get(entity);
                position.X += velocity.X;
                position.Y += velocity.Y;
            }
        }
    }

    public class BallBounceSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var posPool = world.GetPool<Position>();
            var velPool = world.GetPool<Velocity>();
            var filter = world.Filter<Position>().Inc<Velocity>().End();

            foreach (var entity in filter)
            {
                ref var position = ref posPool.Get(entity);
                ref var velocity = ref velPool.Get(entity);

                if (position.X <= 0 || position.X >= 100)
                    velocity.X = -velocity.X;

                if (position.Y <= 0 || position.Y >= 100)
                    velocity.Y = -velocity.Y;
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var world = new EcsWorld();
            var systems = new EcsSystems(world);

            systems
                .Add(new BallMovementSystem())
                .Add(new BallBounceSystem());

            systems.Init();

            var posPool = world.GetPool<Position>();
            var velPool = world.GetPool<Velocity>();
            var ballEntity = world.NewEntity();
            posPool.Add(ballEntity) = new Position { X = 50, Y = 50 };
            velPool.Add(ballEntity) = new Velocity { X = 1, Y = 1 };

            for (int i = 0; i < 1000; i++)
            {
                systems.Run();
                ref var position = ref posPool.Get(ballEntity);
                Console.WriteLine($"Step {i + 1}: Ball Position = ({position.X}, {position.Y})");
            }

            systems.Destroy();
            world.Destroy();
        }
    }
}