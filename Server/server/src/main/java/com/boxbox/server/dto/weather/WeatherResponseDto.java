package com.boxbox.server.dto.weather;

public record WeatherResponseDto(
        String weather,
        Double temperature
) {
}