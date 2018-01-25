using HamstarHelpers.Utilities.Config;
using ResetMode.NetProtocol;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using TimeLimit;


namespace ResetMode {
	class ResetModeMod : Mod {
		public static ResetModeMod Instance { get; private set; }

		public static string GithubUserName { get { return "hamstar0"; } }
		public static string GithubProjectName { get { return "tml-resetmode-mod"; } }

		public static string ConfigFileRelativePath {
			get { return JsonConfig<ResetModeConfigData>.RelativePath + Path.DirectorySeparatorChar + ResetModeConfigData.ConfigFileName; }
		}
		public static void ReloadConfigFromFile() {
			if( Main.netMode != 0 ) {
				throw new Exception( "Cannot reload configs outside of single player." );
			}
			if( ResetModeMod.Instance != null ) {
				if( !ResetModeMod.Instance.JsonConfig.LoadFile() ) {
					ResetModeMod.Instance.JsonConfig.SaveFile();
				}
			}
		}



		////////////////

		public bool IsContentSetup { get; private set; }
		internal JsonConfig<ResetModeConfigData> JsonConfig;
		public ResetModeConfigData Config { get { return JsonConfig.Data; } }


		////////////////

		public ResetModeMod() {
			this.IsContentSetup = false;
			this.Properties = new ModProperties() {
				Autoload = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};

			this.JsonConfig = new JsonConfig<ResetModeConfigData>( ResetModeConfigData.ConfigFileName,
				ConfigurationDataBase.RelativePath, new ResetModeConfigData() );
		}

		public override void Load() {
			ResetModeMod.Instance = this;

			this.LoadConfigs();
		}

		private void LoadConfigs() {
			if( !this.JsonConfig.LoadFile() ) {
				this.JsonConfig.SaveFile();
			}

			if( this.Config.UpdateToLatestVersion() ) {
				ErrorLogger.Log( "Reset Mode updated to " + ResetModeConfigData.ConfigVersion.ToString() );
				this.JsonConfig.SaveFile();
			}
		}
		
		public override void Unload() {
			ResetModeMod.Instance = null;
		}

		////////////////

		public override void PostSetupContent() {
			this.IsContentSetup = true;

			var hook = new CustomTimerAction( delegate () {
				var mymod = ResetModeMod.Instance;
				var myworld = mymod.GetModWorld<ResetModeWorld>();
				myworld.Logic.ResetToNextWorld( mymod );
			} );

			TimeLimitAPI.AddCustomAction( "reset", hook );
		}


		////////////////

		public override void HandlePacket( BinaryReader reader, int player_who ) {
			if( Main.netMode == 1 ) {   // Client
				ClientPackets.HandlePacket( this, reader );
			} else if( Main.netMode == 2 ) {    // Server
				ServerPackets.HandlePacket( this, reader, player_who );
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
