package com.boxbox.server.controller;

import com.boxbox.server.dto.weather.GameWeatherResponseDto;
import com.boxbox.server.dto.weather.WeatherResponseDto;
import com.boxbox.server.global.ApiResponse;
import com.boxbox.server.service.WeatherService;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/external")
public class WeatherController {

    private final WeatherService weatherService;

    public WeatherController(WeatherService weatherService) {
        this.weatherService = weatherService;
    }

    @GetMapping("/weather")
    public ApiResponse<WeatherResponseDto> getCurrentWeather(
            @RequestParam(defaultValue = "SUWON") String cityId
    ) {
        WeatherResponseDto response = weatherService.getCurrentWeather(cityId);
        return ApiResponse.success("날씨 조회 성공", response);
    }

    @GetMapping("/weather/game")
    public ApiResponse<GameWeatherResponseDto> getGameWeather(
            @RequestParam(defaultValue = "SUWON") String cityId
    ) {
        GameWeatherResponseDto response = weatherService.getGameWeather(cityId);
        return ApiResponse.success("게임용 날씨 조회 성공", response);
    }
}