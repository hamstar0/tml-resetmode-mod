using HamstarHelpers.DebugHelpers;
using HamstarHelpers.UserHelpers;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode.Commands {
	class ResetModeStartCommand : ModCommand {
		public override CommandType Type {
			get {
				if( Main.netMode == 0 && !Main.dedServ ) {
					return CommandType.World;
				}
				return CommandType.Console | CommandType.World;
			}
		}
		public override string Command { get { return "resetmodestart"; } }
		public override string Usage { get { return "/resetmodestart"; } }
		public override string Description { get { return "Begins reset mode."; } }
		

		////////////////

		public override void Action( CommandCaller caller, string input, string[] args ) {
			if( Main.netMode == 1 ) {
				LogHelpers.Log( "ResetMode.Commands.ResetModeStartCommand - Not supposed to run on client." );
				return;
			}

			if( Main.netMode == 2 && caller.CommandType != CommandType.Console ) {
				bool success;
				bool has_priv = UserHelpers.HasBasicServerPrivilege( caller.Player, out success );

				if( !success ) {
					caller.Reply( "Could not validate.", Color.Yellow );
					return;
				} else if( !has_priv ) {
					caller.Reply( "Access denied.", Color.Red );
					return;
				}
			}

			var mymod = (ResetModeMod)this.mod;

			try {
				if( mymod.Session.StartSession( mymod ) ) {
					caller.Reply( "Reset mode begun! This will continue until for each new world /resetmodeend is called.", Color.YellowGreen );
				} else {
					caller.Reply( "Reset mode is already in session.", Color.Red );
				}
			} catch( Exception e ) {
				LogHelpers.Log( "ResetMode.Commands.ResetModeStart - Failed to start: " + e.ToString() );
				caller.Reply( "Reset mode could not be started.", Color.Red );
			}
		}
	}
}
