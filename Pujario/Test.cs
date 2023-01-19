using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pujario.Core;
using Pujario.Utils;
using Pujario.Components;
using Pujario.Core.WorldPresentation;
using Pujario.Core.Input;
using Pujario.Core.Kernel;
using Pujario.Core.Diagnostics;
using Pujario.Core.Kernel.Collisions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Diagnostics;

namespace Pujario
{
    internal class Test
    {
        internal class TestActor : BaseActor
        {
            private CTexture _c_chad = new("gigachad");
            private CSpringArm _c_arm = new() { Enabled = true };
            private CCamera _c_camera = new();
            private CEPolygon2Visialiser _c_epolV = new() { CoordsScaleFactor = 1, Visible = true, Thickness = 2, Color = Color.Black };

            private EPolygon2Shape _actorShape;
            private bool _isLShiftPressed = false;
            private readonly ImmutableArray<Vector2> _somePoints = ImmutableArray.Create(new Vector2[]
            {
                new Vector2(0,0)   * 100,
                new Vector2(1,1)   * 100,
                new Vector2(2, 3)  * 100,
                new Vector2(-1, 2) * 100
            });

            private List<EPolygon2Shape> shapes = new();

            public TestActor()
            {
                RootComponent = _c_chad;
                RootComponent.Attach(_c_arm);
                _c_arm.Attach(_c_camera);

                Engine.Instance.Camera = _c_camera;

                var epol = new EPolygon2(new[]
                {
                    new Vector2(-.5f, -1)   * 100,
                    new Vector2(0, 1.5f)    * 100,
                    new Vector2(1, 1)       * 100,
                    new Vector2(1.5f, 0)    * 100
                }, Polygon2CreationOptions.Normalize);
                _actorShape = new(epol);
                _c_epolV.Add(_actorShape);
                _c_epolV.Add(new EPolygon2(_somePoints.ToArray()));
                RootComponent.Attach(_c_epolV);

                TransformChanged += (object sender, EventArgs args) =>
                {
                    _actorShape.Transform = Transform;
                };

                #region InputSubscribes 
                Engine.Instance.InputManager.Subscribe(Keys.LeftShift, pressed => _isLShiftPressed = pressed);

                Engine.Instance.InputManager.Subscribe(MouseMove.Any, point =>
                {
                    if (!_isLShiftPressed) return;
                    var t = Transform2D.Base;
                    t.Position = Engine.Instance.Camera?.ToWorldPosition(point.ToVector2()) ?? Vector2.Zero;
                    Transform = t;
                });

                Engine.Instance.InputManager.Subscribe("Forward", pressed =>
                {
                    var t = Transform;
                    t.Position += new Vector2(0, -50);
                    Transform = t;
                });

                Engine.Instance.InputManager.Subscribe(Keys.S, pressed =>
                {
                    var t = Transform;
                    t.Position += new Vector2(0, 50);
                    Transform = t;
                });

                Engine.Instance.InputManager.Subscribe(Keys.D, pressed =>
                {
                    var t = Transform;
                    t.Position += new Vector2(50, 0);
                    Transform = t;
                });

                Engine.Instance.InputManager.Subscribe(Keys.A, pressed =>
                {
                    var t = Transform;
                    t.Position += new Vector2(-50, 0);
                    Transform = t;
                });

                Engine.Instance.InputManager.Subscribe(MouseScrolls.Vertical, num =>
                {
                    shapes.Add(new(_somePoints.ToArray()));
                    Debug.WriteLine("Shapes Count = " + shapes.Count);
                });
                #endregion
            }

            public override void Update(GameTime gameTime)
            {
                for (int i = 0; i < shapes.Count; ++i)
                for (int j = 0; j < shapes.Count; ++j)
                {
                    if (shapes[i].CheckCollision(_actorShape))
                    {
                        _c_epolV.Color = Color.Red;
                    }
                    else
                    {
                        _c_epolV.Color = Color.Black;
                    }
                }
                base.Update(gameTime);
            }
        }


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
                    var actor = new TestActor() { Enabled = true, Visible = true };
                    actor.RootComponent.Attach(new CBeacon
                    {
                        Beacon = new SquareBeacon() { R = 1, Mode = TickBeaconMode.Any }
                    });

                    return actor;
                },
                t
            );
        }
    }
}
