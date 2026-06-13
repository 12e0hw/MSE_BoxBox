package com.boxbox.server.dto.weather;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

import java.util.List;

// Ignore unused fields in the OpenWeather API response.
@JsonIgnoreProperties(ignoreUnknown = true)
public record OpenWeatherResponse(
        List<WeatherInfo> weather,
        MainInfo main,
        RainInfo rain
) {
    @JsonIgnoreProperties(ignoreUnknown = true)
    public record WeatherInfo(
            Integer id,
            String main,
            String description
    ) {}

    @JsonIgnoreProperties(ignoreUnknown = true)
    public record MainInfo(
            Double temp
    ) {}

    @JsonIgnoreProperties(ignoreUnknown = true)
    public record RainInfo(
            // Map the JSON "1h" value to the oneHour field.
            @JsonProperty("1h")
            Double oneHour  
    ) {}
}