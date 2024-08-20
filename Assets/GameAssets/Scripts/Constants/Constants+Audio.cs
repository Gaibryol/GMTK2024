using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Constants
{
	public class Audio
	{
		public const string MusicVolumePP = "Music";
		public const string SFXVolumePP = "SFX";

		public const float DefaultMusicVolume = 0.25f;
		public const float DefaultSFXVolume = 0.25f;

		public const float MusicFadeSpeed = 0.1f;

		public class Music
		{
            public const string MainMenuTheme = "MainMenu";
            public const string Level1Theme = "Level1";
			public const string Level2Theme = "Level2";
			public const string Level3Theme = "Level3";
			public const string Level4Theme = "Level4";
			public const string Level5Theme = "Level5";
        }

		public class SFX
		{
            public const string ButtonPress = "ButtonPress";
            public const string Dandelion = "Dandelion";
            public const string Invalid = "Invalid";
            public const string MushroomJump = "MushroomJump";
            public const string PlatformDoorMove = "PlatformDoorMove";
            public const string UIClick = "UIClick";
            public const string WormFall = "WormFall";
            public const string WormJoin = "WormJoin";
            public const string WormSplit = "WormSplit";

        }
    }
}