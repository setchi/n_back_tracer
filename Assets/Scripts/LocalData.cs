
public class LocalData {

	public static JsonModel.LocalData Read() {
		return LocalStorage.Read<JsonModel.LocalData>() ?? new JsonModel.LocalData();
	}

	public static void Write(JsonModel.LocalData data) {
		LocalStorage.Write<JsonModel.LocalData>(data);
	}
}
