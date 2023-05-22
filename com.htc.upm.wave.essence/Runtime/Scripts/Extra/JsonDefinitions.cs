// "WaveXR SDK
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System;
using System.Text;
using UnityEngine;
using Wave.Native;

namespace Wave.Essence.Extra
{
	/*
	 * OEM Config
	 *	 { "show": 2 } */
	[System.Serializable]
	public class JSON_BatteryPolicy
	{
		public int show;
	}

	/*
	 ** OEM Config
	 * { "enable": true } */
	[System.Serializable]
	public class JSON_BeamPolicy
	{
		public bool enable;
	}

	/**
	 * OEM Config
	 * \"beam\": {
	 * \"start_width\": 0.000625,
	 * \"end_width\": 0.00125,
	 * \"start_offset\": 0.015,
	 * \"length\":  0.8,
	 * \"start_color\": \"#FFFFFFFF\",
	 * \"end_color\": \"#FFFFFF4D\"
	 * },
	 **/
	[System.Serializable]
	public class JSON_BeamDesc
	{
		public float start_width;
		public float end_width;
		public float start_offset;
		public float length;
		public string start_color;
		public string end_color;
	}

	/**
	 * OEM Config
	 * \"pointer\": {
	   \"diameter\": 0.01,
	   \"distance\": 1.3,
	   \"use_texture\": true,
	   \"color\": \"#FFFFFFFF\",
	   \"border_color\": \"#777777FF\",
	   \"focus_color\": \"#FFFFFFFF\",
	   \"focus_border_color\": \"#777777FF\",
	   \"texture_name\":  null,
	   \"Blink\": false
	   },
	 **/
	[System.Serializable]
	public class JSON_PointerDesc
	{
		public float diameter;
		public float distance;
		public bool use_texture;
		public string color;
		public string border_color;
		public string focus_color;
		public string focus_border_color;
		public string texture_name;
		public bool Blink;
	}

	/**
	* OEM Config
	*  "model": {
		"touchpad_dot_color":"#00B3E3FF",
		"touchpad_dot_use_texture":true,
		"touchpad_dot_texture_name":null
	},
	**/
	[System.Serializable]
	public class JSON_ModelDesc
	{
		public string touchpad_dot_color;
		public bool touchpad_dot_use_texture;
		public string touchpad_dot_texture_name;
	}

	/*
	"battery":{
	"battery_level_count":5,
	"battery_levels":[
		{ "level_max_value":1,
		"level_min_value":0.80,
		"level_texture_name":null },
		{ "level_max_value":0.79,
		"level_min_value":0.60,
		"level_texture_name":null },
		{ "level_max_value":0.59,
		"level_min_value":0.40,
		"level_texture_name":null },
		{ "level_max_value":0.39,
		"level_min_value":0.20,
		"level_texture_name":null },
		{ "level_max_value":0.19,
		"level_min_value":0,
		"level_texture_name":null } ]
	},
	not used
	**/
	[System.Serializable]
	public class JSON_BatteryLevel
	{
		public float level_max_value;
		public float level_min_value;
		public string level_texture_name;
	}

	[System.Serializable]
	public class JSON_BatteryDesc
	{
		public int battery_level_count;
		public JSON_BatteryLevel[] battery_levels;
	}

	/*
	"arm":{
	"head_to_elbow_offset":[0.2,-0.7,0],
	"elbow_to_wrist_offset":[0,0,0.15],
	"wrist_to_controller_offset":[0,0,0.05],
	"elbow_pitch_angle_offset":[-0.2,0.55,0.08],
	"elbow_pitch_angle_min":0,
	"elbow_pitch_angle_max":90,
	"elbow_yaxis_raise_height":0,
	"elbow_zaxis_raise_depth":0
	},
	**/
	[System.Serializable]
	public class JSON_ArmDesc
	{
		public Vector3 head_to_elbow_offset;
		public Vector3 elbow_to_wrist_offset;
		public Vector3 wrist_to_controller_offset;
		public Vector3 elbow_pitch_angle_offset;
		public int elbow_pitch_angle_min;
		public int elbow_pitch_angle_max;
		public int elbow_yaxis_raise_height;
		public int elbow_zaxis_raise_depth;
	}

	/*
	"reticle":{
	"inner_diameter":0,
	"outer_diameter":0.005236002,
	"interact_object_inner_diameter":0.02094472,
	"interact_object_outer_diameter":0.02618144,
	"distance":10,
	"segments":20,
	"color":"#FFFFFFFF",
	"rotation_speed":6,
	"colorFlicker":false,
	"deepening_color_rotation":false
	},
	**/
	[System.Serializable]
	public class JSON_ReticleDesc
	{
		public float inner_diameter;
		public float outer_diameter;
		public float interact_object_inner_diameter;
		public float interact_object_outer_diameter;
		public float distance;
		public float segments;
		public string color;
		public float rotation_speed;
		public bool colorFlicker;
		public bool deepening_color_rotation;
	}

	/*
	"particle":{
	"color":[1.0,1.0,1.0,1.0],
	"Width":[0.5,0.5,0.5]
	},
	**/
	[System.Serializable]
	public class JSON_ParticleDesc
	{
		public Color32 color;
		public Vector3 Width;
	}

	/*
	"reticle_pointer":{
	"color":[1.0,1.0,1.0,1.0],
	"size":1.0,
	"use_texture":true,
	"texture_name":null
	},
	not used
	**/
	[System.Serializable]
	public class JSON_ReticlePointerDesc
	{
		public Color32 color;
		public float size;
		public bool use_texture;
		public string texture_name;
	}

    /*
    "hand_styles" : {
      "default_style": {
        "thickness": 0.001,
        "contouringOpacity": 0.5,
        "fillingOpacity": 0.5,
        "graColorA": [ 1.0, 1.0, 1.0, 0 ]
        "graColorB": [ 1.0, 1.0, 1.0, 0 ],
        "conGraColorA": [ 1.0, 1.0, 1.0, 0 ],
        "conGraColorB": [ 1.0, 1.0, 1.0, 0 ]
      },
      "fusion_style": {
        "thickness": 0.002,
        "contouringOpacity": 0.5,
        "fillingOpacity": 0.5,
        "graColorA": [ 1.0, 1.0, 1.0, 0 ],
        "graColorB": [ 1.0, 1.0, 1.0, 0 ],
        "conGraColorA": [ 1.0, 1.0, 1.0, 0 ],
        "conGraColorB": [ 1.0, 1.0, 1.0, 0 ]
      }
    }
    **/
    [System.Serializable]
    public class JSON_HandStyleDesc
    {
        public float thickness;
        public float contouring_opacity;
        public float filling_opacity;
        public float[] gra_color_A;
        public float[] gra_color_B;
        public float[] con_gra_color_A;
        public float[] con_gra_color_B;
    }

    [System.Serializable]
    public class JSON_HandModelDesc
    {
        public JSON_HandStyleDesc default_style;
        public JSON_HandStyleDesc fusion_style;
    }

    public class JSON_HandStyleDesc_Ext
    {
        public JSON_HandStyleDesc_Ext()
        {
        }

        public JSON_HandStyleDesc_Ext(JSON_HandStyleDesc desc)
        {
            try
            {
                thickness = desc.thickness;
                contouring_opacity = desc.contouring_opacity;
                filling_opacity = desc.filling_opacity;
                float[] gca = desc.gra_color_A;
                float[] gcb = desc.gra_color_B;
                float[] cgca = desc.con_gra_color_A;
                float[] cgcb = desc.con_gra_color_B;
                gra_color_A = new Color(gca[0], gca[1], gca[2], gca[3]);
                gra_color_B = new Color(gcb[0], gcb[1], gcb[2], gcb[3]);
                con_gra_color_A = new Color(cgca[0], cgca[1], cgca[2], cgca[3]);
                con_gra_color_B = new Color(cgcb[0], cgcb[1], cgcb[2], cgcb[3]);
            }
            catch (Exception e)
            {
                Log.e("OEMConfig", "Please check float array size. (It may cause the exception below).");
                throw e;
            }
        }

        public void Dump(StringBuilder sb)
        {
            sb
                .Append("CA=").Append(gra_color_A)
                .Append(", ").Append("CB=").Append(gra_color_B)
                .Append(", ").Append("CCA=").Append(con_gra_color_A)
                .Append(", ").Append("CCB=").Append(con_gra_color_B)
                .Append(", ").Append("Op=").Append(filling_opacity)
                .Append(", ").Append("COp=").Append(contouring_opacity)
                .Append(", ").Append("Th=").Append(thickness);
        }

        public float thickness;
        public float contouring_opacity;
        public float filling_opacity;
        public Color gra_color_A;
        public Color gra_color_B;
        public Color con_gra_color_A;
        public Color con_gra_color_B;
    }

    public class JSON_HandModelDesc_Ext
    {
        public JSON_HandModelDesc_Ext()
        {
            default_style = null;
            fusion_style = null;
        }

        // Will throw exception if color array is not 4 element.
        public JSON_HandModelDesc_Ext(JSON_HandModelDesc desc)
        {
            default_style = new JSON_HandStyleDesc_Ext(desc.default_style);
            fusion_style = new JSON_HandStyleDesc_Ext(desc.fusion_style);
        }
        public JSON_HandStyleDesc_Ext default_style;
        public JSON_HandStyleDesc_Ext fusion_style;
    }


    /*
	 "controller_pointer":{
	 "color":[1.0,1.0,1.0,1.0],
	 "size":1.0 
	 }
	not used
	**/
    [System.Serializable]
	public class JSON_ControllerPointerDesc
	{
		public Color32 color;
		public float size;
	}

	[System.Serializable]
	public class JSON_ControllerDesc
	{
		public JSON_BeamDesc beam;
		public JSON_PointerDesc pointer;
		public JSON_ModelDesc model;
		public JSON_BatteryDesc battery;
		public JSON_ArmDesc armModel;
		public JSON_ReticleDesc reticle;
		public JSON_ParticleDesc particle;
		public JSON_ReticlePointerDesc rPointer;
		public JSON_ControllerPointerDesc cPointer;
	}
}
