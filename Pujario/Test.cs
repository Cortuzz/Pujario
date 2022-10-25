using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pujario.Core;
using Pujario.Utils;
using Pujario.Components;
using Pujario.Core.WorldPresentation;

namespace Pujario
{
    internal class Test
    {
        public static void InitializeTestGround()
        {
            var t = Transform2D.Base;
            /*t.Position = new Vector2(
                Engine.Instance.WorldMapping.Grid.GetLength(0) / 2 * Engine.Instance.WorldMapping.ChunkSize,
                Engine.Instance.WorldMapping.Grid.GetLength(1) / 2 * Engine.Instance.WorldMapping.ChunkSize);*/
            t.Position = new Vector2(1000, 1000);
            Engine.Instance.SpawnActor(
                () =>
                {
                    var actor = new BaseActor()
                    {
                        RootComponent = new CBeacon
                        {
                            Beacon = new SquareBeacon() { R = 1, Mode = TickBeaconMode.Any }
                        },
                        Enabled = true,
                        Visible = true,
                    };
                    var chad = new CTexture("gigachad");
                    var arm = new CSpringArm() { Enabled = true };
                    var _camera = new CCamera();

                    actor.RootComponent.Attach(arm);
                    actor.RootComponent.Attach(chad);
                    arm.Attach(_camera);

                    // actor.RootComponent.Attach(_camera);
                    Engine.Instance.Camera = _camera;

                    /*Engine.Instance.InputManager.Subscribe(MouseMove.Any, point =>
                    {
                        var t = Transform2D.Base;
                        t.Position = Engine.Instance.Camera?.ToWorldPosition(point.ToVector2()) ?? Vector2.Zero;
                        actor.Transform = t;
                    });*/

                    Engine.Instance.InputManager.Subscribe("Forward", pressed =>
                    {
                        var t = actor.Transform;
                        t.Position += new Vector2(0, -50);
                        actor.Transform = t;
                    });

                    Engine.Instance.InputManager.Subscribe(Keys.S, pressed =>
                    {
                        var t = actor.Transform;
                        t.Position += new Vector2(0, 50);
                        actor.Transform = t;
                    });

                    Engine.Instance.InputManager.Subscribe(Keys.D, pressed =>
                    {
                        var t = actor.Transform;
                        t.Position += new Vector2(50, 0);
                        actor.Transform = t;
                    });

                    Engine.Instance.InputManager.Subscribe(Keys.A, pressed =>
                    {
                        var t = actor.Transform;
                        t.Position += new Vector2(-50, 0);
                        actor.Transform = t;
                    });

                    return actor;
                },
                t
            );
        }
    }
}
