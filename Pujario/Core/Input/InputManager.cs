using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Pujario.Core.Input
{
    /// <summary>
    /// An huge inefficient event dispatcher for input events, so refactoring is welcome   
    /// </summary>
    public partial class InputManager :
        IEventDispatcher<Action<bool>, Keys>,
        IEventDispatcher<Action<bool>, Buttons>,
        IEventDispatcher<Action<bool>, MouseButtons>,
        IEventDispatcher<Action<bool>, string>,
        IEventDispatcher<Action<float>, GamePadTriggersInput>,
        IEventDispatcher<Action<Point>, MouseMove>,
        IEventDispatcher<Action<int>, MouseScrolls>,
        IEventDispatcher<Action<Vector2>, GamePadSticksInput>
    {
        private readonly partial struct TypeOrderedHelper
        {
        }

        private partial class SingleInputEvent<TState>
        {
        }

        private readonly struct DiscreteEventHelper<TKey> : IEventDispatcher<Action<bool>, TKey>
            where TKey : notnull
        {
            public readonly Dictionary<TKey, SingleInputEvent<bool>> Dictionary;
            private readonly Func<TKey, SingleInputEvent<bool>> _create;

            public DiscreteEventHelper(Func<TKey, SingleInputEvent<bool>> fabricMethod)
            {
                _create = fabricMethod;
                Dictionary = new Dictionary<TKey, SingleInputEvent<bool>>();
            }

            public void Subscribe(TKey on, Action<bool> handler)
            {
                if (!Dictionary.TryGetValue(on, out var @event))
                {
                    @event = _create(on);
                    Dictionary.Add(on, @event);
                }

                @event.Add(handler);
            }

            public void Unsubscribe(TKey on, Action<bool> handler)
            {
                if (Dictionary.ContainsKey(on)) Dictionary[on].Remove(handler);
            }
        }

        public static Func<bool, bool, bool> BoolEquality { get; set; } = EqualityComparer<bool>.Default.Equals;
        public static Func<float, float, bool> FloatEquality{ get; set; } = EqualityComparer<float>.Default.Equals;
        public static Func<Vector2, Vector2, bool> Vector2Equality{ get; set; } = EqualityComparer<Vector2>.Default.Equals;
        public static Func<int, int, bool> IntEquality{ get; set; } = EqualityComparer<int>.Default.Equals;

        private readonly Dictionary<string, InputCombination> _keyMapping;

        private readonly DiscreteEventHelper<Keys> _keyEvents;
        private readonly DiscreteEventHelper<Buttons> _buttonEvents;
        private readonly DiscreteEventHelper<MouseButtons> _mouseButtonsEvents;

        private readonly SingleInputEvent<Point> _mouseMoveEvent;
        private readonly SingleInputEvent<Vector2>[] _stickMoveEvent;
        private readonly SingleInputEvent<float>[] _triggerMoveEvent;
        private readonly SingleInputEvent<int>[] _scrollEvents;

        public KeyboardState KeyboardState { get; private set; }
        public GamePadState GamePadState { get; private set; }
        public MouseStateWrap MouseState { get; private set; }

        private void _updateInputStates()
        {
            KeyboardState = Keyboard.GetState();
            GamePadState = GamePad.GetState(0); // todo: resolve multiple gamepad case
            MouseState = new MouseStateWrap(Mouse.GetState());
        }

        public InputManager(IDictionary<string, InputCombination> keyMapping, Type[] typeOrder)
        {
            var typeOrder1 = new Type[typeOrder.Length + 1];
            typeOrder1[0] = GetType();
            typeOrder.CopyTo(typeOrder1, 1);
            _keyMapping = new Dictionary<string, InputCombination>(keyMapping);

            _updateInputStates();

            _keyEvents = new DiscreteEventHelper<Keys>(
                key => new SingleInputEvent<bool>(KeyboardState.IsKeyDown(key), BoolEquality, typeOrder1));
            _buttonEvents = new DiscreteEventHelper<Buttons>(
                key => new SingleInputEvent<bool>(GamePadState.IsButtonDown(key), BoolEquality, typeOrder1));
            _mouseButtonsEvents = new DiscreteEventHelper<MouseButtons>(
                key => new SingleInputEvent<bool>(MouseState.IsButtonPressed(key), BoolEquality, typeOrder1));
            _mouseMoveEvent = new SingleInputEvent<Point>(
                MouseState.State.Position, EqualityComparer<Point>.Default.Equals, typeOrder1);
            _stickMoveEvent = new[]
            {
                new SingleInputEvent<Vector2>(GamePadState.ThumbSticks.Left, Vector2Equality, typeOrder1),
                new SingleInputEvent<Vector2>(GamePadState.ThumbSticks.Right, Vector2Equality, typeOrder1)
            };
            _triggerMoveEvent = new[]
            {
                new SingleInputEvent<float>(GamePadState.Triggers.Left, FloatEquality, typeOrder1),
                new SingleInputEvent<float>(GamePadState.Triggers.Right, FloatEquality, typeOrder1)
            };
            _scrollEvents = new[]
            {
                new SingleInputEvent<int>(MouseState.State.ScrollWheelValue, IntEquality, typeOrder1),
                new SingleInputEvent<int>(MouseState.State.HorizontalScrollWheelValue, IntEquality, typeOrder1)
            };
        }

        public virtual void RaiseEvents()
        {
            _updateInputStates();

            foreach (var (key, value) in _keyEvents.Dictionary) value.HandleInput(KeyboardState.IsKeyDown(key));
            foreach (var (key, value) in _buttonEvents.Dictionary) value.HandleInput(GamePadState.IsButtonDown(key));
            foreach (var (key, value) in _mouseButtonsEvents.Dictionary) value.HandleInput(MouseState.IsButtonPressed(key));
            _mouseMoveEvent.HandleInput(MouseState.State.Position);
            _stickMoveEvent[0].HandleInput(GamePadState.ThumbSticks.Left);
            _stickMoveEvent[1].HandleInput(GamePadState.ThumbSticks.Right);
            _triggerMoveEvent[0].HandleInput(GamePadState.Triggers.Left);
            _triggerMoveEvent[1].HandleInput(GamePadState.Triggers.Right);
            _scrollEvents[0].HandleInput(MouseState.State.ScrollWheelValue);
            _scrollEvents[1].HandleInput(MouseState.State.HorizontalScrollWheelValue);
        }

        public void Subscribe(string on, Action<bool> handler)
        {
            foreach (var key in _keyMapping[on].KeyboardInput) Subscribe(key, handler);
            foreach (var button in _keyMapping[on].GamepadInput) Subscribe(button, handler);
            foreach (var alias in _keyMapping[on].KeyMappingAliases) Subscribe(alias, handler);
        }

        public void Unsubscribe(string on, Action<bool> handler)
        {
            foreach (var key in _keyMapping[on].KeyboardInput) Unsubscribe(key, handler);
            foreach (var button in _keyMapping[on].GamepadInput) Unsubscribe(button, handler);
            foreach (var alias in _keyMapping[on].KeyMappingAliases) Unsubscribe(alias, handler);
        }

        public void Subscribe(Keys on, Action<bool> handler) => _keyEvents.Subscribe(on, handler);
        public void Unsubscribe(Keys on, Action<bool> handler) => _keyEvents.Unsubscribe(on, handler);
        public void Subscribe(Buttons on, Action<bool> handler) => _buttonEvents.Subscribe(on, handler);
        public void Unsubscribe(Buttons on, Action<bool> handler) => _buttonEvents.Unsubscribe(on, handler);
        public void Subscribe(MouseButtons on, Action<bool> handler) => _mouseButtonsEvents.Subscribe(on, handler);
        public void Unsubscribe(MouseButtons on, Action<bool> handler) => _mouseButtonsEvents.Unsubscribe(on, handler);
        public void Subscribe(GamePadTriggersInput on, Action<float> handler) => _triggerMoveEvent[(int)on].Add(handler);
        public void Unsubscribe(GamePadTriggersInput on, Action<float> handler) => _triggerMoveEvent[(int)on].Remove(handler);
        public void Subscribe(MouseMove on, Action<Point> handler) => _mouseMoveEvent.Add(handler);
        public void Unsubscribe(MouseMove on, Action<Point> handler) => _mouseMoveEvent.Remove(handler);
        public void Subscribe(GamePadSticksInput on, Action<Vector2> handler) => _stickMoveEvent[(int)on].Add(handler);
        public void Unsubscribe(GamePadSticksInput on, Action<Vector2> handler) => _stickMoveEvent[(int)on].Remove(handler);
        public void Subscribe(MouseScrolls on, Action<int> handler) => _scrollEvents[(int)on].Add(handler);
        public void Unsubscribe(MouseScrolls on, Action<int> handler) => _scrollEvents[(int)on].Remove(handler);
    }
}