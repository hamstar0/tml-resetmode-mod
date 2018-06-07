using HamstarHelpers.DebugHelpers;
using HamstarHelpers.TmlHelpers;
using HamstarHelpers.Utilities.Config;
using ResetMode.Data;
using ResetMode.Logic;
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



		////////////////
		
		internal JsonConfig<ResetModeConfigData> ConfigJson;
		public ResetModeConfigData Config { get { return ConfigJson.Data; } }
		
		public SessionLogic Session { get; internal set; }


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

			if( this.Config.ResetAllWorldsBetweenGames ) {
				WorldLogic.ClearAllWorlds();
			}

			TmlLoadHelpers.AddWorldUnloadEachPromise( () => {
				if( this.Config.ResetAllWorldsBetweenGames ) {
					WorldLogic.ClearAllWorlds();
				}
			} );

			TmlLoadHelpers.AddWorldLoadEachPromise( delegate {
				if( this.Config.AutoStart && Main.netMode != 1 ) {
					this.Session.Start( this );
				}
			} );
		}

		public override void PostAddRecipes() {
			var hook = new CustomTimerAction( delegate () {
				if( Main.netMode == 1 ) { return; }

				var mymod = ResetModeMod.Instance;
				var myworld = mymod.GetModWorld<ResetModeWorld>();

				myworld.Logic.ExpireForCurrentSession( mymod );
			} );

			TimeLimitAPI.AddCustomAction( "reset", hook );

			this.LoadRewards();
		}


		private void LoadConfigs() {
			if( !this.ConfigJson.LoadFile() ) {
				this.ConfigJson.SaveFile();
			}
			
			if( this.Config.UpdateToLatestVersion() ) {
				ErrorLogger.Log( "Reset Mode updated to " + ResetModeConfigData.ConfigVersion.ToString() );
				this.ConfigJson.SaveFile();
			}
		}

		private void LoadRewards() {
			Mod rewards_mod = ModLoader.GetMod( "Rewards" );
			if( rewards_mod == null || rewards_mod.Version < new Version( 1, 4, 12 ) ) {
				if( this.Config.DebugModeInfo ) {
					LogHelpers.Log( "Reset Mode - Mod.LoadRewards - No Rewards mod found." );
				}
				return;
			}
			
			Action<Player, float> func = ( plr, rewards ) => {
				if( rewards == 0 ) { return; }

				var mymod = ResetModeMod.Instance;

				mymod.Session.AddRewards( plr, rewards );

				if( this.Config.DebugModeInfo ) {
					LogHelpers.Log( "Reset Mode - Mod.LoadRewards - Points added: "+rewards );
				}
			};

			try {
				rewards_mod.Call( "OnPointsGained", func );

				if( this.Config.DebugModeInfo ) {
					LogHelpers.Log( "Reset Mode - Mod.LoadRewards - Success." );
				}
			} catch( Exception e ) {
				LogHelpers.Log( "Reset Mode - Mod.LoadRewards - Could not hook Rewards: " + e.ToString() );
			}
		}

		
		public override void Unload() {
			ResetModeMod.Instance = null;
		}


		////////////////

		public override void PreSaveAndQuit() {
			if( Main.netMode != 1 ) {
				this.Session.Save( this );	// This probably does nothing...
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
