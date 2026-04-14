package com.boxbox.server.dto.weather;

public record GameWeatherResponseDto(
        String weather,
        Double temperature,
        String gameEffect
) {
}