package com.boxbox.server.dto.weather;

public record GameWeatherResponseDto(
        // City API 사용을 위해 cityId, cityName 추가
        String cityId,
        String cityName,
        String weather,
        Double temperature,
        String gameEffect
) {
}