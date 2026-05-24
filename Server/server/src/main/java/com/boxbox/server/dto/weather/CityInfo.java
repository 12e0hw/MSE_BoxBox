package com.boxbox.server.dto.weather;

public record CityInfo(
        String cityId,
        String cityName,
        String countryCode
) {
}