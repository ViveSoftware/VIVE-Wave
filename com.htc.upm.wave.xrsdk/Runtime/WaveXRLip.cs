// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System.Diagnostics;
using UnityEngine;
using Wave.XR.Settings;

namespace Wave.OpenXR
{
	public static class InputDeviceLip
	{
		const string LOG_TAG = "Wave.OpenXR.InputDeviceLip";
		static void DEBUG(string msg) { UnityEngine.Debug.Log(LOG_TAG + " " + msg); }

		public enum Expressions
		{
			Jaw_Right = 0,
			Jaw_Left = 1,
			Jaw_Forward = 2,
			Jaw_Open = 3,
			Mouth_Ape_Shape = 4,
			Mouth_Upper_Right = 5,
			Mouth_Upper_Left = 6,
			Mouth_Lower_Right = 7,
			Mouth_Lower_Left = 8,
			Mouth_Upper_Overturn = 9,
			Mouth_Lower_Overturn = 10,
			Mouth_Pout = 11,
			Mouth_Smile_Right = 12,
			Mouth_Smile_Left = 13,
			Mouth_Sad_Right = 14,
			Mouth_Sad_Left = 15,
			Cheek_Puff_Right = 16,
			Cheek_Puff_Left = 17,
			Cheek_Suck = 18,
			Mouth_Upper_UpRight = 19,
			Mouth_Upper_UpLeft = 20,
			Mouth_Lower_DownRight = 21,
			Mouth_Lower_DownLeft = 22,
			Mouth_Upper_Inside = 23,
			Mouth_Lower_Inside = 24,
			Mouth_Lower_Overlay = 25,
			Tongue_Longstep1 = 26,
			Tongue_Left = 27,
			Tongue_Right = 28,
			Tongue_Up = 29,
			Tongue_Down = 30,
			Tongue_Roll = 31,
			Tongue_Longstep2 = 32,
			Tongue_UpRight_Morph = 33,
			Tongue_UpLeft_Morph = 34,
			Tongue_DownRight_Morph = 35,
			Tongue_DownLeft_Morph = 36,
			Max,
		}
		public static readonly Expressions[] s_LipExps = new Expressions[(int)Expressions.Max]
		{
		Expressions.Jaw_Right,               // 0
		Expressions.Jaw_Left,
		Expressions.Jaw_Forward,
		Expressions.Jaw_Open,
		Expressions.Mouth_Ape_Shape,
		Expressions.Mouth_Upper_Right,       // 5
        Expressions.Mouth_Upper_Left,
		Expressions.Mouth_Lower_Right,
		Expressions.Mouth_Lower_Left,
		Expressions.Mouth_Upper_Overturn,
		Expressions.Mouth_Lower_Overturn,    // 10
        Expressions.Mouth_Pout,
		Expressions.Mouth_Smile_Right,
		Expressions.Mouth_Smile_Left,
		Expressions.Mouth_Sad_Right,
		Expressions.Mouth_Sad_Left,          // 15
        Expressions.Cheek_Puff_Right,
		Expressions.Cheek_Puff_Left,
		Expressions.Cheek_Suck,
		Expressions.Mouth_Upper_UpRight,
		Expressions.Mouth_Upper_UpLeft,      // 20
        Expressions.Mouth_Lower_DownRight,
		Expressions.Mouth_Lower_DownLeft,
		Expressions.Mouth_Upper_Inside,
		Expressions.Mouth_Lower_Inside,
		Expressions.Mouth_Lower_Overlay,     // 25
        Expressions.Tongue_Longstep1,
		Expressions.Tongue_Left,
		Expressions.Tongue_Right,
		Expressions.Tongue_Up,
		Expressions.Tongue_Down,             // 30
        Expressions.Tongue_Roll,
		Expressions.Tongue_Longstep2,
		Expressions.Tongue_UpRight_Morph,
		Expressions.Tongue_UpLeft_Morph,
		Expressions.Tongue_DownRight_Morph,  // 35
        Expressions.Tongue_DownLeft_Morph,
		};

		#region Wave XR Constants
		const string kIsLipExpressionAvailable = "IsLipExpressionAvailable";
		const string kLipExpressionStatus = "LipExpressionStatus";
		const string kHasLipExpressionValue = "HasLipExpressionValue";
		const string kLipExpression_Jaw_Right = "LipExpression_Jaw_Right"; // 0
		const string kLipExpression_Jaw_Left = "LipExpression_Jaw_Left";
		const string kLipExpression_Jaw_Forward = "LipExpression_Jaw_Forward";
		const string kLipExpression_Jaw_Open = "LipExpression_Jaw_Open";
		const string kLipExpression_Mouth_Ape_Shape = "LipExpression_Mouth_Ape_Shape";
		const string kLipExpression_Mouth_Upper_Right = "LipExpression_Mouth_Upper_Right"; // 5
		const string kLipExpression_Mouth_Upper_Left = "LipExpression_Mouth_Upper_Left";
		const string kLipExpression_Mouth_Lower_Right = "LipExpression_Mouth_Lower_Right";
		const string kLipExpression_Mouth_Lower_Left = "LipExpression_Mouth_Lower_Left";
		const string kLipExpression_Mouth_Upper_Overturn = "LipExpression_Mouth_Upper_Overturn";
		const string kLipExpression_Mouth_Lower_Overturn = "LipExpression_Mouth_Lower_Overturn"; // 10
		const string kLipExpression_Mouth_Pout = "LipExpression_Mouth_Pout";
		const string kLipExpression_Mouth_Smile_Right = "LipExpression_Mouth_Smile_Right";
		const string kLipExpression_Mouth_Smile_Left = "LipExpression_Mouth_Smile_Left";
		const string kLipExpression_Mouth_Sad_Right = "LipExpression_Mouth_Sad_Right";
		const string kLipExpression_Mouth_Sad_Left = "LipExpression_Mouth_Sad_Left"; // 15
		const string kLipExpression_Cheek_Puff_Right = "LipExpression_Cheek_Puff_Right";
		const string kLipExpression_Cheek_Puff_Left = "LipExpression_Cheek_Puff_Left";
		const string kLipExpression_Cheek_Suck = "LipExpression_Cheek_Suck";
		const string kLipExpression_Mouth_Upper_Upright = "LipExpression_Mouth_Upper_Upright";
		const string kLipExpression_Mouth_Upper_Upleft = "LipExpression_Mouth_Upper_Upleft"; // 20
		const string kLipExpression_Mouth_Lower_Downright = "LipExpression_Mouth_Lower_Downright";
		const string kLipExpression_Mouth_Lower_Downleft = "LipExpression_Mouth_Lower_Downleft";
		const string kLipExpression_Mouth_Upper_Inside = "LipExpression_Mouth_Upper_Inside";
		const string kLipExpression_Mouth_Lower_Inside = "LipExpression_Mouth_Lower_Inside";
		const string kLipExpression_Mouth_Lower_Overlay = "LipExpression_Mouth_Lower_Overlay"; // 25
		const string kLipExpression_Tongue_Longstep1 = "LipExpression_Tongue_Longstep1";
		const string kLipExpression_Tongue_Left = "LipExpression_Tongue_Left";
		const string kLipExpression_Tongue_Right = "LipExpression_Tongue_Right";
		const string kLipExpression_Tongue_Up = "LipExpression_Tongue_Up";
		const string kLipExpression_Tongue_Down = "LipExpression_Tongue_Down"; // 30
		const string kLipExpression_Tongue_Roll = "LipExpression_Tongue_Roll";
		const string kLipExpression_Tongue_Longstep2 = "LipExpression_Tongue_Longstep2";
		const string kLipExpression_Tongue_Upright_Morph = "LipExpression_Tongue_Upright_Morph";
		const string kLipExpression_Tongue_Upleft_Morph = "LipExpression_Tongue_Upleft_Morph";
		const string kLipExpression_Tongue_Downright_Morph = "LipExpression_Tongue_Downright_Morph"; // 35
		const string kLipExpression_Tongue_Downleft_Morph = "LipExpression_Tongue_Downleft_Morph";
		#endregion

		// Keys and Values size = 37
		static string[] s_ExpKeys = {
			kLipExpression_Jaw_Right, // 0
			kLipExpression_Jaw_Left,
			kLipExpression_Jaw_Forward,
			kLipExpression_Jaw_Open,
			kLipExpression_Mouth_Ape_Shape,
			kLipExpression_Mouth_Upper_Right, // 5
			kLipExpression_Mouth_Upper_Left,
			kLipExpression_Mouth_Lower_Right,
			kLipExpression_Mouth_Lower_Left,
			kLipExpression_Mouth_Upper_Overturn,
			kLipExpression_Mouth_Lower_Overturn, // 10
			kLipExpression_Mouth_Pout,
			kLipExpression_Mouth_Smile_Right,
			kLipExpression_Mouth_Smile_Left,
			kLipExpression_Mouth_Sad_Right,
			kLipExpression_Mouth_Sad_Left, // 15
			kLipExpression_Cheek_Puff_Right,
			kLipExpression_Cheek_Puff_Left,
			kLipExpression_Cheek_Suck,
			kLipExpression_Mouth_Upper_Upright,
			kLipExpression_Mouth_Upper_Upleft, // 20
			kLipExpression_Mouth_Lower_Downright,
			kLipExpression_Mouth_Lower_Downleft,
			kLipExpression_Mouth_Upper_Inside,
			kLipExpression_Mouth_Lower_Inside,
			kLipExpression_Mouth_Lower_Overlay, // 25
			kLipExpression_Tongue_Longstep1,
			kLipExpression_Tongue_Left,
			kLipExpression_Tongue_Right,
			kLipExpression_Tongue_Up,
			kLipExpression_Tongue_Down, // 30
			kLipExpression_Tongue_Roll,
			kLipExpression_Tongue_Longstep2,
			kLipExpression_Tongue_Upright_Morph,
			kLipExpression_Tongue_Upleft_Morph,
			kLipExpression_Tongue_Downright_Morph, // 35
			kLipExpression_Tongue_Downleft_Morph
		};
		static float[] s_ExpValues = {
			0, // Jaw_Right = 0
			0, // Jaw_Left
			0, // Jaw_Forward
			0, // Jaw_Open
			0, // Mouth_Ape_Shape
			0, // Mouth_Upper_Right = 5
			0, // Mouth_Upper_Left
			0, // Mouth_Lower_Right
			0, // Mouth_Lower_Left
			0, // Mouth_Upper_Overturn
			0, // Mouth_Lower_Overturn = 10
			0, // Mouth_Pout
			0, // Mouth_Smile_Right
			0, // Mouth_Smile_Left
			0, // Mouth_Sad_Right
			0, // Mouth_Sad_Left = 15
			0, // Cheek_Puff_Right
			0, // Cheek_Puff_Left
			0, // Cheek_Suck
			0, // Mouth_Upper_Upright
			0, // Mouth_Upper_Upleft = 20
			0, // Mouth_Lower_Downright
			0, // Mouth_Lower_Downleft
			0, // Mouth_Upper_Inside
			0, // Mouth_Lower_Inside
			0, // Mouth_Lower_Overlay = 25
			0, // Tongue_Longstep1
			0, // Tongue_Left
			0, // Tongue_Right
			0, // Tongue_Up
			0, // Tongue_Down = 30
			0, // Tongue_Roll
			0, // Tongue_Longstep2
			0, // Tongue_Upright_Morph
			0, // Tongue_Upleft_Morph
			0, // Tongue_Downright_Morph = 35
			0, // Tongue_Downleft_Morph
		};

		#region Wave XR Interface
		/// <summary>
		/// Enables or disables the Lip Expression feature.
		/// </summary>
		/// <param name="active">True for enable.</param>
		public static void ActivateLipExp(bool active)
		{
			WaveXRSettings settings = WaveXRSettings.GetInstance();
			if (settings != null)
			{
				// Check current Wave XR Lip Expression status before activation.
				settings.EnableLipExpression = IsLipExpAvailable();

				string caller = "TBD";
				var frame = new StackFrame(1, true);
				if (frame != null)
				{
					var method = frame.GetMethod();
					if (method != null)
						caller = method.Name;
					else
						caller = "No method.";
				}
				if (settings.EnableLipExpression != active)
				{
					settings.EnableLipExpression = active;
					DEBUG("ActivateLipExp() " + (settings.EnableLipExpression ? "Activate." : "Deactivate.") + " from " + caller);
					SettingsHelper.SetBool(WaveXRSettings.EnableLipExpressionText, settings.EnableLipExpression);
				}
				else
				{
					DEBUG("ActivateLipExp() Lip Expression is already " + (settings.EnableLipExpression ? "enabled." : "disabled.") + " from " + caller);
				}
			}
		}

		/// <summary>
		/// Retrieves current status of Lip Expression.
		/// </summary>
		/// <returns>
		/// 0: Not Start
		/// 1: Start Failure
		/// 2: Starting
		/// 3: Stopping
		/// 4: Available
		/// 5: Unsupported
		/// </returns>
		public static uint GetLipExpStatus()
		{
			uint status = 0;
			SettingsHelper.GetInt(kLipExpressionStatus, ref status);
			return status;
		}

		/// <summary>
		/// Checks if the Lip Expression feature is available currently.
		/// </summary>
		/// <returns>True for available.</returns>
		public static bool IsLipExpAvailable()
		{
			bool enabled = false;
			SettingsHelper.GetBool(kIsLipExpressionAvailable, ref enabled);
			return enabled;
		}

		static bool m_HasLipExpValue = false;
		/// <summary>
		/// Checks if the Lip Expression value is provided in runtime.
		/// </summary>
		/// <returns>True for value provided.</returns>
		public static bool HasLipExpValue()
		{
			UpdateExpressionValues();
			return m_HasLipExpValue;
		}

		/// <summary>
		/// Retrieves the value of an <see cref="Expressions">eye expression</see>.
		/// </summary>
		/// <param name="exp">An <see cref="Expressions">eye expression</see>.</param>
		/// <returns>A float value.</returns>
		public static float GetLipExpValue(Expressions exp)
		{
			UpdateExpressionValues();
			return s_ExpValues[(int)exp];
		}

		/// <summary>
		/// Retrieves all Lip Expression values in a float array. The values are sorted in the order of <see cref="Expressions">Eye Expressions</see>.
		/// </summary>
		/// <param name="values">All **Eye Expression** values in a float array</param>
		/// <returns>True for valid output value.</returns>
		public static bool GetLipExpValues(out float[] values)
		{
			UpdateExpressionValues();
			values = s_ExpValues;
			return HasLipExpValue();
		}

		static float m_LipExpFrame = 0;
		static void UpdateExpressionValues()
		{
			if (m_LipExpFrame == Time.frameCount) { return; }
			m_LipExpFrame = Time.frameCount;

			SettingsHelper.GetBool(kHasLipExpressionValue, ref m_HasLipExpValue);

			for (int i = 0; i < s_LipExps.Length; i++)
			{
				if (s_LipExps[i] == Expressions.Max) { continue; }

				if (m_HasLipExpValue)
					SettingsHelper.GetFloat(s_ExpKeys[i], ref s_ExpValues[i]);
				else
					s_ExpValues[i] = 0;
			}
		}
		#endregion
	}
}