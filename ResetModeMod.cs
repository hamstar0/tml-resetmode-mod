using HamstarHelpers.Components.Config;
using HamstarHelpers.DebugHelpers;
using HamstarHelpers.Services.Promises;
using ResetMode.Data;
using ResetMode.Logic;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode {
	partial class ResetModeMod : Mod {
		public static ResetModeMod Instance { get; private set; }

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



		////////////////

		internal JsonConfig<ResetModeConfigData> ConfigJson;
		public ResetModeConfigData Config { get { return ConfigJson.Data; } }
		
		public SessionLogic Session { get; internal set; }

		internal int CurrentNetMode = -1;


		////////////////

		public ResetModeMod() {
			this.Properties = new ModProperties() {
				Autoload = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};
			
			this.ConfigJson = new JsonConfig<ResetModeConfigData>( ResetModeConfigData.ConfigFileName,
				ConfigurationDataBase.RelativePath, new ResetModeConfigData() );

			this.Session = new SessionLogic();
		}


		public override void Load() {
			ResetModeMod.Instance = this;

			this.LoadConfigs();

			Promises.AddWorldLoadEachPromise( delegate {
				this.CurrentNetMode = Main.netMode;
			} );

			this.Session.OnModLoad();
		}

		
		public override void Unload() {
			ResetModeMod.Instance = null;
		}
		
		////////////////

		private void LoadConfigs() {
			if( !this.ConfigJson.LoadFile() ) {
				LogHelpers.Log( "ResetMode.ResetModeMod.LoadConfigs - Reset Mode missing/invalid config created anew." );
				this.ConfigJson.SaveFile();
			}

			if( this.Config.UpdateToLatestVersion() ) {
				ErrorLogger.Log( "Reset Mode updated to " + ResetModeConfigData.ConfigVersion.ToString() );
				this.ConfigJson.SaveFile();
			}
		}


		////////////////

		public override object Call( params object[] args ) {
			if( args.Length == 0 ) { throw new Exception( "Undefined call type." ); }

			string call_type = args[0] as string;
			if( args == null ) { throw new Exception( "Invalid call type." ); }

			var new_args = new object[args.Length - 1];
			Array.Copy( args, 1, new_args, 0, args.Length - 1 );

			return ResetModeAPI.Call( call_type, new_args );
		}
	}
}
