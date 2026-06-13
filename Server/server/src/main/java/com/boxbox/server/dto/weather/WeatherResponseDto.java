package com.boxbox.server.dto.weather;

// Response DTO for general weather data.
public record WeatherResponseDto(
        String cityId,
        String cityName,
        String weather,
        Double temperature
) {
}