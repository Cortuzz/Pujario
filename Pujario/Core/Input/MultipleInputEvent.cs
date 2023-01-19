// developing InputCombination stuff was too unpleasant so left if 

// using System;
// using System.Collections.Generic;
// using System.Collections.Immutable;
// using System.Linq;
// using Microsoft.Xna.Framework.Input;
//
// namespace Pujario.Core.Input
// {
//     public partial class InputManager
//     {
//         // a bit clunky and (maybe) low performance class, so refactoring is welcome   
//         private partial class MultipleInputEvent<TState> 
//         {
//             private readonly InputManager _inputManager;
//
//             private readonly KeyValuePair<Keys, Action<bool>>[] _keyBindings;
//             private readonly KeyValuePair<Buttons, Action<bool>>[] _buttonBindings;
//             private readonly KeyValuePair<string, Action<bool>>[] _aliasBindings;
//
//             // private readonly bool[] _aliasState;
//             // private readonly bool[] _keyState;
//             // private readonly bool[] _buttonState;
//
//             private readonly bool[][] _boolStates = new bool[3][]; 
//
//             private bool _isChanged;
//
//             private void _initListeners<TOn>(
//                 ImmutableArray<TOn> src,
//                 out KeyValuePair<TOn, Action<bool>>[] bindings,
//                 out bool[] state,
//                 Action<KeyValuePair<TOn, Action<bool>>> subscribe)
//             {
//                 var size = src.Length;
//                 bindings = new KeyValuePair<TOn, Action<bool>>[size];
//                 state = new bool[size];
//                 var stateA = state;
//                 for (int i = 0; i < size; ++i)
//                 {
//                     var index = i;
//                     var pair = new KeyValuePair<TOn, Action<bool>>(
//                         src[i],
//                         begin =>
//                         {
//                             stateA[index] = begin;
//                             _isChanged = true;
//                         });
//                     bindings[i] = pair;
//                     subscribe(pair);
//                 }
//             }
//
//             public MultipleInputEvent(
//                 in InputCombination combination,
//                 InputManager inputManager,
//                 TState initState,
//                 IEqualityComparer<TState> equality,
//                 Type[] typeOrder)
//                 : base(initState, equality, typeOrder)
//             {
//                 _inputManager = inputManager;
//                 _isChanged = false;
//                 
//                 _initListeners(combination.KeyMappingAliases, out _aliasBindings, out _boolStates[0],
//                     pair => inputManager.Subscribe(pair.Key, pair.Value));
//                 _initListeners(combination.KeyboardInput, out _keyBindings, out _boolStates[1],
//                     pair => inputManager.Subscribe(pair.Key, pair.Value));
//                 _initListeners(combination.GamepadInput, out _buttonBindings, out _boolStates[2],
//                     pair => inputManager.Subscribe(pair.Key, pair.Value));
//             }
//
//             ~MultipleInputEvent()
//             {
//                 foreach (var pair in _aliasBindings) _inputManager.Unsubscribe(pair.Key, pair.Value);
//                 foreach (var pair in _buttonBindings) _inputManager.Unsubscribe(pair.Key, pair.Value);
//                 foreach (var pair in _keyBindings) _inputManager.Unsubscribe(pair.Key, pair.Value);
//             }
//
//             public void HandleInput()
//             {
//                 if (!_isChanged) return;
//                 _isChanged = false;
//                 var state = true;
//                 foreach (var boolState in _boolStates)
//                 {
//                     if (boolState.Any(b => !b))
//                     {
//                         state = false;
//                         break;
//                     }
//                 }
//                 base.HandleInput(state);
//             }
//         }
//     }
// }