package com.boxbox.server.dto.weather;

// Response DTO for weather data used by the game.
public record GameWeatherResponseDto(
        String cityId,
        String cityName,
        String weather,
        Double temperature,
        String gameEffect
) {
}