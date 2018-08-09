using HamstarHelpers.Components.Config;
using HamstarHelpers.Helpers.DebugHelpers;
using ResetMode.Data;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode {
	partial class ResetModeMod : Mod {
		public static string GithubUserName { get { return "hamstar0"; } }
		public static string GithubProjectName { get { return "tml-resetmode-mod"; } }

		public static string ConfigFileRelativePath {
			get { return JsonConfig<ResetModeConfigData>.ConfigSubfolder + Path.DirectorySeparatorChar + ResetModeConfigData.ConfigFileName; }
		}
		public static void ReloadConfigFromFile() {
			if( Main.netMode != 0 ) {
				throw new Exception( "Cannot reload configs outside of single player." );
			}
			if( ResetModeMod.Instance != null ) {
				if( !ResetModeMod.Instance.ConfigJson.LoadFile() ) {
					ResetModeMod.Instance.ConfigJson.SaveFile();
				}
			}
		}
		public static void ResetConfigFromDefaults() {
			if( Main.netMode != 0 ) {
				throw new Exception( "Cannot reset to default configs outside of single player." );
			}

			var new_config = new ResetModeConfigData();
			//new_config.SetDefaults();

			ResetModeMod.Instance.ConfigJson.SetData( new_config );
			ResetModeMod.Instance.ConfigJson.SaveFile();
		}
	}
}
