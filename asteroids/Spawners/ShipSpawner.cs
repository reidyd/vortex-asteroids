﻿using asteroids.Components;
using SlimMath;
using Vortex.Core.Assets;
using Vortex.Core.Collision;
using Vortex.Graphics;
using Vortex.Graphics.Enums;
using Vortex.Scenegraph;
using Vortex.Scenegraph.Components;
using Vortex.Scenegraph.Components.Collision;
using Vortex.Scenegraph.Utility;

namespace asteroids.Spawners
{
    public static class ShipSpawner
    {
        public static Entity SpawnIn(Scene scene, Vector3 position)
        {
            var ship = ColladaUtils.CreateEntity(scene, @"Models\asteroid-spaceship.dae");
            ship.Name = "ship";

            ship.TransformComponent.LocalPosition = position;

            ship.CreateComponent<RigidbodyComponent>(component =>
            {
                component.Mass = 4000;
                component.Drag = 0.01f;
                component.PositionConstraint = new Vector3Constraints {X = false, Y = false, Z = true};
            });

            // todo: this stuff shouldn't be done outside the scenegraph construction.
            var mesh = ship.GetComponentInSelfOrChildren<MeshComponent>();
            mesh.Material = StaticAssetLoader.GetInstance<Material>("Materials/ship.material");

            var radius = mesh.WorldBoundingSphere().Radius;

            var weaponPort = scene.CreateEntity();
            weaponPort.Parent = mesh.Entity;
            weaponPort.TransformComponent.LocalPosition = new Vector3(-radius, 0, 0);
            weaponPort.CreateComponent<WeaponPort>();

            var lightAttachPoint = scene.CreateEntity("light", ship);
            lightAttachPoint.CreateComponent<LightComponent>(component =>
            {
                component.Colour = new Color4(1.0f, 0.8f, 1.0f, 0.5f);
                component.Intensity = 2f;
                component.Range = 64.0f;
                component.LightType = LightType.Point;
            });
            lightAttachPoint.TransformComponent.LocalPosition = new Vector3(radius, 0, 0);

            ship.CreateComponent<ShipMovement>();
            ship.CreateComponent<ShipFiring>();
            ship.CreateComponent<ShipDefence>();
            ship.CreateComponent<SphereColliderComponent>(component => component.Radius = 1);

            ship.CreateComponent<JsScriptComponent>(component =>
            {
                component.Source = StaticAssetLoader.GetString("screenConstrainer.js");
                component.Properties.Extents = new Vector3(40, 30, 0);
            });

            return ship;

        }
    }
}