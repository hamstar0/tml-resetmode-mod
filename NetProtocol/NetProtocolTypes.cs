namespace ResetMode.NetProtocol {
	public enum ResetModeProtocolTypes : byte {
		RequestModSettings,
		ModSettings,
		RequestWorldData,
		WorldData,
		PlayerData,
		PromptForReset,
		RequestPlayerSessionJoinAck,
		PlayerSessionJoinAck,
		RequestSessionData,
		SessionData
	}
}
