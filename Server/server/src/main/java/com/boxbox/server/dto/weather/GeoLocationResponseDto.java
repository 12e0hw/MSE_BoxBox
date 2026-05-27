package com.boxbox.server.dto.weather;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;

@JsonIgnoreProperties(ignoreUnknown = true)
public record GeoLocationResponseDto(
        String name,
        Double lat,
        Double lon,
        String country
) {
}