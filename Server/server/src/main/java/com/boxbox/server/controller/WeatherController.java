package com.boxbox.server.controller;

import com.boxbox.server.dto.weather.GameWeatherResponseDto;
import com.boxbox.server.dto.weather.WeatherResponseDto;
import com.boxbox.server.global.ApiResponse;
import com.boxbox.server.service.WeatherService;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/external")
public class WeatherController {

    private final WeatherService weatherService;

    public WeatherController(WeatherService weatherService) {
        this.weatherService = weatherService;
    }

    @GetMapping("/weather")
    public ApiResponse<WeatherResponseDto> getWeather() {
        WeatherResponseDto data = weatherService.getCurrentWeather();
        return ApiResponse.success("날씨 조회 성공", data);
    }

    @GetMapping("/weather/game")
    public ApiResponse<GameWeatherResponseDto> getGameWeather() {
        GameWeatherResponseDto data = weatherService.getGameWeather();
        return ApiResponse.success("게임용 날씨 조회 성공", data);
    }
}