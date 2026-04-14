package com.boxbox.server.dto.weather;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

import java.util.List;

// OpenWeather 응답에 정의하지 않은 필드가 더 있어도 무시
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
            // JSON의 "1h" 값을 oneHour 필드에 매핑
            @JsonProperty("1h")
            Double oneHour  
    ) {}
}