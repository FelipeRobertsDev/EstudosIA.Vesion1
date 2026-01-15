using System;
using System.Text.Json;
using System.Text.Json.Nodes;

public static class RouteJsonEnricher
{
    public static JsonDocument EnrichSpots(JsonDocument route)
    {
        var node = JsonNode.Parse(route.RootElement.GetRawText()) as JsonObject;
        if (node is null) return route;

        // suporta "spots" ou "Spots"
        var spotsNode = node["spots"] ?? node["Spots"];
        if (spotsNode is not JsonArray spotsArray) return route;

        foreach (var item in spotsArray)
        {
            if (item is not JsonObject spotObj) continue;

            // 🔑 ID único do spot
            if (spotObj["id"] is null)
                spotObj["id"] = Guid.NewGuid().ToString();

            // 🧭 Flag se está no roteiro do usuário
            if (spotObj["isInRoute"] is null)
                spotObj["isInRoute"] = false;
        }

        return JsonDocument.Parse(node.ToJsonString());
    }
}
