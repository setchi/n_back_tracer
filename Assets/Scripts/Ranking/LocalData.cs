using System;

public class LocalData {

	public static string BestScore {
		get { return Read().bestScore ?? "0"; }
		set { Rewrite(localData => {
				localData.bestScore = value;
				return localData;
			}); }
	}
	
	public static JsonModel.PlayerInfo PlayerInfo {
		get { return Read().playerInfo; }
		set { Rewrite(localData => {
				localData.playerInfo = value;
				return localData;
			}); }
	}

	static JsonModel.LocalData Read() {
		return LocalStorage.Read<JsonModel.LocalData>() ?? new JsonModel.LocalData();
	}

	static void Write(JsonModel.LocalData data) {
		LocalStorage.Write<JsonModel.LocalData>(data);
	}

	static void Rewrite(Func<JsonModel.LocalData, JsonModel.LocalData> update) {
		Write(update(Read()));
	}
}
