﻿using asteroids.Components;
using SlimMath;
using Vortex.Core.Collision;
using Vortex.Graphics;
using Vortex.Graphics.Enums;
using Vortex.Graphics.Shaders;
using Vortex.Scenegraph;
using Vortex.Scenegraph.Components;
using Vortex.Scenegraph.Components.Collision;
using Vortex.Scenegraph.Utility;

namespace asteroids.Spawners
{
    public static class ShipSpawner
    {
        public static void SpawnIn(Scene scene, Vector3 position)
        {
            var ship = ColladaUtils.CreateEntity(scene, @"Models\asteroid-spaceship.dae");
            ship.Name = "ship";

            position = new Vector3(-30, 0, 0);

            ship.LocalPosition = position;

            

            ship.CreateComponent<RigidbodyComponent>(component =>
            {
                component.Mass = 4000;
                component.Drag = 0.01f;
                component.PositionConstraint = new Vector3Constraints {X = false, Y = false, Z = true};
            });

            ship.CreateComponent<ShipMovement>();
            ship.CreateComponent<ShipFiring>();

            // todo: this stuff shouldn't be done outside the scenegraph construction.
            var mesh = ship.GetComponentInSelfOrChildren<MeshComponent>();
            mesh.Material = GetShipMaterial();

            var radius = mesh.WorldBoundingSphere().Radius;

            var weaponPort = scene.CreateEntity();
            weaponPort.Parent = mesh.Entity;
            weaponPort.LocalPosition = new Vector3(-radius, 0, 0);
            weaponPort.CreateComponent<WeaponPort>();

            var lightAttachPoint = scene.CreateEntity();
            lightAttachPoint.CreateComponent<LightComponent>(component =>
            {
                component.Colour = new Color4(1.0f, 0.8f, 0.5f, 0.5f);
                component.Intensity = 0.8f;
                component.Range = 196.0f;
                component.LightType = LightType.Point;
            });
            lightAttachPoint.LocalPosition = new Vector3(radius, 0, 0);
            ship.AddChild(lightAttachPoint);

            ship.CreateComponent<ShipDefence>();
            ship.CreateComponent<SphereColliderComponent>(component => component.Radius = 1);
        }

        private static Material GetShipMaterial()
        {
            var shipMaterial = Material.Get("shipMaterial");

            if (shipMaterial != null)
                return shipMaterial;

            shipMaterial = Material.Create("shipMaterial", Shader.Get("sdk_shaders/defaultAmbient.shader"));
            shipMaterial.SetColor4("matDiffuse", new Color4(1.0f, 1.0f, 1.0f, 1.0f));
            shipMaterial.SetColor4("matAmbient", new Color4(1.0f, 1.0f, 1.0f, 1.0f));

            return shipMaterial;
        }
    }
}