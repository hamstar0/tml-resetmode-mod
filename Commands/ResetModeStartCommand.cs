using HamstarHelpers.DebugHelpers;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode.Commands {
	class ResetModeStartCommand : ModCommand {
		public override CommandType Type {
			get {
				if( Main.netMode == 0 ) {
					return CommandType.World;
				}
				return CommandType.Console;
			}
		}
		public override string Command { get { return "resetmodestart"; } }
		public override string Usage { get { return "/resetmodestart"; } }
		public override string Description { get { return "Begins reset mode."; } }
		

		////////////////

		public override void Action( CommandCaller caller, string input, string[] args ) {
			var mymod = (ResetModeMod)this.mod;

			try {
				if( mymod.Session.StartSession( mymod ) ) {
					caller.Reply( "Reset mode begun! This will continue until for each new world /resetmodeend is called.", Color.YellowGreen );
				} else {
					caller.Reply( "Reset mode is already in session.", Color.Red );
				}
			} catch( Exception e ) {
				LogHelpers.Log( e.ToString() );
				caller.Reply( "Reset mode could not be started.", Color.Red );
			}
		}
	}
}
