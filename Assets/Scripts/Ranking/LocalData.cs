using System;

public class LocalData {

	public static JsonModel.LocalData Read() {
		return LocalStorage.Read<JsonModel.LocalData>() ?? new JsonModel.LocalData();
	}

	public static void Write(JsonModel.LocalData data) {
		LocalStorage.Write<JsonModel.LocalData>(data);
	}

	public static void Rewrite(Func<JsonModel.LocalData, JsonModel.LocalData> update) {
		Write(update(Read()));
	}
}
