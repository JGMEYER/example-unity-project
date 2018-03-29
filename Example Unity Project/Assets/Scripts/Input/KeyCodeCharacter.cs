using System.Collections.Generic;
using UnityEngine;

public static class KeyCodeCharacter {

    private static Dictionary<KeyCode, char> keyCodeToChar = new Dictionary<KeyCode, char>()
    {
        // Lower Case Letters
        {KeyCode.A, 'A'},
        {KeyCode.B, 'B'},
        {KeyCode.C, 'C'},
        {KeyCode.D, 'D'},
        {KeyCode.E, 'E'},
        {KeyCode.F, 'F'},
        {KeyCode.G, 'G'},
        {KeyCode.H, 'H'},
        {KeyCode.I, 'I'},
        {KeyCode.J, 'J'},
        {KeyCode.K, 'K'},
        {KeyCode.L, 'L'},
        {KeyCode.M, 'M'},
        {KeyCode.N, 'N'},
        {KeyCode.O, 'O'},
        {KeyCode.P, 'P'},
        {KeyCode.Q, 'Q'},
        {KeyCode.R, 'R'},
        {KeyCode.S, 'S'},
        {KeyCode.T, 'T'},
        {KeyCode.U, 'U'},
        {KeyCode.V, 'V'},
        {KeyCode.W, 'W'},
        {KeyCode.X, 'X'},
        {KeyCode.Y, 'Y'},
        {KeyCode.Z, 'Z'},

        // KeyPad Numbers
        {KeyCode.Keypad1, '1'},
        {KeyCode.Keypad2, '2'},
        {KeyCode.Keypad3, '3'},
        {KeyCode.Keypad4, '4'},
        {KeyCode.Keypad5, '5'},
        {KeyCode.Keypad6, '6'},
        {KeyCode.Keypad7, '7'},
        {KeyCode.Keypad8, '8'},
        {KeyCode.Keypad9, '9'},
        {KeyCode.Keypad0, '0'},

        // Other Symbols
        {KeyCode.Ampersand, '&'},  // 7
        {KeyCode.AltGr, '~'},
        {KeyCode.Asterisk, '*'},  // 8
        {KeyCode.At, '@'},  // 2
        {KeyCode.BackQuote, '`'},
        {KeyCode.Backslash, '\\'},
        {KeyCode.Caret, '^'},  // 6
        {KeyCode.Colon, ':'},
        {KeyCode.Comma, ','},
        {KeyCode.Dollar, '$'},  // 4
        {KeyCode.DoubleQuote, '"'},
        {KeyCode.Equals, '='},
        {KeyCode.Exclaim, '!'},  // 1
        {KeyCode.Greater, '>'},
        {KeyCode.Hash, '#'},  // 3
        {KeyCode.LeftBracket, '['},
        {KeyCode.LeftParen, '('},  // 9
        {KeyCode.Less, '<'},
        {KeyCode.Minus, '-'},
        {KeyCode.Period, '.'},
        {KeyCode.Plus, '+'},
        {KeyCode.Question, '?'},
        {KeyCode.Quote, '\''},
        {KeyCode.RightBracket, ']'},
        {KeyCode.RightParen, ')'},  // 0
        {KeyCode.Semicolon, ';'},
        {KeyCode.Slash, '/'},
        {KeyCode.Underscore, '_'},

        // =========
        // Unicode
        // =========

        // Directional keys
        {KeyCode.UpArrow, '↑'},
        {KeyCode.LeftArrow, '←'},
        {KeyCode.DownArrow, '↓'},
        {KeyCode.RightArrow, '→'},

        // Modifier Keys
        {KeyCode.Backspace, '⌫'},
        {KeyCode.Delete, '⌦'},
        {KeyCode.CapsLock, '⇪' },
        {KeyCode.Return, '⏎'},
        {KeyCode.Space, '␣'},
        {KeyCode.Tab, '⇥'},

        // Misc
        {KeyCode.Escape, '␛'},
    };

    public static char For(KeyCode keyCode)
    {
        char keyChar;
        if (!keyCodeToChar.TryGetValue(keyCode, out keyChar)) {
            string errorMsg = string.Format("KeyCode \"{0}\" does not have " +
                " a registered character representation.", keyCode.ToString());
            throw new KeyNotFoundException(errorMsg);
        }
        return keyChar;
    }

}
