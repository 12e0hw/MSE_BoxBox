package com.boxbox.server.dto.weather;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;

// Response DTO for OpenWeather geocoding results.
@JsonIgnoreProperties(ignoreUnknown = true)
public record GeoLocationResponseDto(
        String name,
        Double lat,
        Double lon,
        String country
) {
}