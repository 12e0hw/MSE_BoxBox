package com.boxbox.server.dto.weather;

// Store basic city information used for weather API requests.
public record CityInfo(
        String cityId,
        String cityName,
        String countryCode
) {
}