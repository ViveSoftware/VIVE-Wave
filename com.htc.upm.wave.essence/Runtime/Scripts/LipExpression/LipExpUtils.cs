// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC\u2019s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using Wave.Native;
using Wave.OpenXR;

namespace Wave.Essence.LipExpression
{
    public enum LipExp
    {
        Jaw_Right = WVR_LipExpression.WVR_LipExpression_Jaw_Right,
        Jaw_Left = WVR_LipExpression.WVR_LipExpression_Jaw_Left,
        Jaw_Forward = WVR_LipExpression.WVR_LipExpression_Jaw_Forward,
        Jaw_Open = WVR_LipExpression.WVR_LipExpression_Jaw_Open,
        Mouth_Ape_Shape = WVR_LipExpression.WVR_LipExpression_Mouth_Ape_Shape,
        Mouth_Upper_Right = WVR_LipExpression.WVR_LipExpression_Mouth_Upper_Right,      // 5
        Mouth_Upper_Left = WVR_LipExpression.WVR_LipExpression_Mouth_Upper_Left,
        Mouth_Lower_Right = WVR_LipExpression.WVR_LipExpression_Mouth_Lower_Right,
        Mouth_Lower_Left = WVR_LipExpression.WVR_LipExpression_Mouth_Lower_Left,
        Mouth_Upper_Overturn = WVR_LipExpression.WVR_LipExpression_Mouth_Upper_Overturn,
        Mouth_Lower_Overturn = WVR_LipExpression.WVR_LipExpression_Mouth_Lower_Overturn,   // 10
        Mouth_Pout = WVR_LipExpression.WVR_LipExpression_Mouth_Pout,
        Mouth_Smile_Right = WVR_LipExpression.WVR_LipExpression_Mouth_Smile_Right,
        Mouth_Smile_Left = WVR_LipExpression.WVR_LipExpression_Mouth_Smile_Left,
        Mouth_Sad_Right = WVR_LipExpression.WVR_LipExpression_Mouth_Sad_Right,
        Mouth_Sad_Left = WVR_LipExpression.WVR_LipExpression_Mouth_Sad_Left,         // 15
        Cheek_Puff_Right = WVR_LipExpression.WVR_LipExpression_Cheek_Puff_Right,
        Cheek_Puff_Left = WVR_LipExpression.WVR_LipExpression_Cheek_Puff_Left,
        Cheek_Suck = WVR_LipExpression.WVR_LipExpression_Cheek_Suck,
        Mouth_Upper_UpRight = WVR_LipExpression.WVR_LipExpression_Mouth_Upper_Upright,
        Mouth_Upper_UpLeft = WVR_LipExpression.WVR_LipExpression_Mouth_Upper_Upleft,     // 20
        Mouth_Lower_DownRight = WVR_LipExpression.WVR_LipExpression_Mouth_Lower_Downright,
        Mouth_Lower_DownLeft = WVR_LipExpression.WVR_LipExpression_Mouth_Lower_Downleft,
        Mouth_Upper_Inside = WVR_LipExpression.WVR_LipExpression_Mouth_Upper_Inside,
        Mouth_Lower_Inside = WVR_LipExpression.WVR_LipExpression_Mouth_Lower_Inside,
        Mouth_Lower_Overlay = WVR_LipExpression.WVR_LipExpression_Mouth_Lower_Overlay,    // 25
        Tongue_Longstep1 = WVR_LipExpression.WVR_LipExpression_Tongue_Longstep1,
        Tongue_Left = WVR_LipExpression.WVR_LipExpression_Tongue_Left,
        Tongue_Right = WVR_LipExpression.WVR_LipExpression_Tongue_Right,
        Tongue_Up = WVR_LipExpression.WVR_LipExpression_Tongue_Up,
        Tongue_Down = WVR_LipExpression.WVR_LipExpression_Tongue_Down,            // 30
        Tongue_Roll = WVR_LipExpression.WVR_LipExpression_Tongue_Roll,
        Tongue_Longstep2 = WVR_LipExpression.WVR_LipExpression_Tongue_Longstep2,
        Tongue_UpRight_Morph = WVR_LipExpression.WVR_LipExpression_Tongue_Upright_Morph,
        Tongue_UpLeft_Morph = WVR_LipExpression.WVR_LipExpression_Tongue_Upleft_Morph,
        Tongue_DownRight_Morph = WVR_LipExpression.WVR_LipExpression_Tongue_Downright_Morph, // 35
        Tongue_DownLeft_Morph = WVR_LipExpression.WVR_LipExpression_Tongue_Downleft_Morph,
        Max = WVR_LipExpression.WVR_LipExpression_Max,
    }
    public static class LipExpUtils
    {
        public static readonly LipExp[] s_LipExps = new LipExp[(int)LipExp.Max]
        {
        LipExp.Jaw_Right,
        LipExp.Jaw_Left,
        LipExp.Jaw_Forward,
        LipExp.Jaw_Open,
        LipExp.Mouth_Ape_Shape,
        LipExp.Mouth_Upper_Right,       // 5
        LipExp.Mouth_Upper_Left,
        LipExp.Mouth_Lower_Right,
        LipExp.Mouth_Lower_Left,
        LipExp.Mouth_Upper_Overturn,
        LipExp.Mouth_Lower_Overturn,    // 10
        LipExp.Mouth_Pout,
        LipExp.Mouth_Smile_Right,
        LipExp.Mouth_Smile_Left,
        LipExp.Mouth_Sad_Right,
        LipExp.Mouth_Sad_Left,          // 15
        LipExp.Cheek_Puff_Right,
        LipExp.Cheek_Puff_Left,
        LipExp.Cheek_Suck,
        LipExp.Mouth_Upper_UpRight,
        LipExp.Mouth_Upper_UpLeft,      // 20
        LipExp.Mouth_Lower_DownRight,
        LipExp.Mouth_Lower_DownLeft,
        LipExp.Mouth_Upper_Inside,
        LipExp.Mouth_Lower_Inside,
        LipExp.Mouth_Lower_Overlay,     // 25
        LipExp.Tongue_Longstep1,
        LipExp.Tongue_Left,
        LipExp.Tongue_Right,
        LipExp.Tongue_Up,
        LipExp.Tongue_Down,             // 30
        LipExp.Tongue_Roll,
        LipExp.Tongue_Longstep2,
        LipExp.Tongue_UpRight_Morph,
        LipExp.Tongue_UpLeft_Morph,
        LipExp.Tongue_DownRight_Morph,  // 35
        LipExp.Tongue_DownLeft_Morph,
        };

        public static LipExp Exp(this WVR_LipExpression exp)
        {
            if (exp == WVR_LipExpression.WVR_LipExpression_Max) { return LipExp.Max; }

            return s_LipExps[(int)exp];
        }

        public static InputDeviceLip.Expressions Expression(this LipExp exp)
        {
            if (exp == LipExp.Max) { return InputDeviceLip.Expressions.Max; }

            return InputDeviceLip.s_LipExps[(int)exp];
        }
    }
}