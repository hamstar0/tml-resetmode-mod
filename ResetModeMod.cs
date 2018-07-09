using HamstarHelpers.Components.Config;
using HamstarHelpers.DebugHelpers;
using HamstarHelpers.Services.Promises;
using ResetMode.Data;
using ResetMode.Logic;
using System;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode {
	partial class ResetModeMod : Mod {
		public static ResetModeMod Instance { get; private set; }



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
			Promises.AddCustomPromise( "ResetModeWorldExited", () => {
				this.CurrentNetMode = -1;
				return true;
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
