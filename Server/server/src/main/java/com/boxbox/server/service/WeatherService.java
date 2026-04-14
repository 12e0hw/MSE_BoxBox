package com.boxbox.server.service;

import com.boxbox.server.dto.weather.GameWeatherResponseDto;
import com.boxbox.server.dto.weather.OpenWeatherResponse;
import com.boxbox.server.dto.weather.WeatherResponseDto;
import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.http.MediaType;
import org.springframework.stereotype.Service;
import org.springframework.web.client.RestClient;

import java.util.List;

@Service
public class WeatherService {

    // application.properties에서 읽어오는 설정값들
    private final RestClient openWeatherRestClient;
    private final String apiKey;
    private final double lat;
    private final double lon;
    private final String units;
    private final String lang;

    /*
      일반 날씨 조회용 메서드
     
      외부 API에서 현재 날씨를 받아온 뒤, BoxBox에서 사용할 수 있게 weather + temperature만 담아서 반환한다.
     
      예:
      {
        "weather": "RAIN",
        "temperature": 12.3
      }
     */
    public WeatherService(
            @Qualifier("openWeatherRestClient") RestClient openWeatherRestClient,
            @Value("${weather.api.key}") String apiKey,
            @Value("${weather.location.lat}") double lat,
            @Value("${weather.location.lon}") double lon,
            @Value("${weather.api.units}") String units,
            @Value("${weather.api.lang}") String lang
    ) {
        this.openWeatherRestClient = openWeatherRestClient;
        this.apiKey = apiKey;
        this.lat = lat;
        this.lon = lon;
        this.units = units;
        this.lang = lang;
    }

    public WeatherResponseDto getCurrentWeather() {
        OpenWeatherResponse response = fetchCurrentWeather();

        String weather = classifyWeather(response);
        Double temperature = response.main().temp();

        return new WeatherResponseDto(weather, temperature);
    }

    /*
      게임용 날씨 조회 메서드
     
      외부 API에서 현재 날씨를 받아온 뒤, 게임 로직에서 직접 쓸 수 있도록 effect와 background까지 가공해서 반환한다.
     
     예:
      {
        "weather": "RAIN",
        "temperature": 12.3,
        "gameEffect": "SLIPPERY_FLOOR",
        "recommendedBackground": "RAINY_STAGE"
      }
     */
    public GameWeatherResponseDto getGameWeather() {
        OpenWeatherResponse response = fetchCurrentWeather();

        String weather = classifyWeather(response);
        Double temperature = response.main().temp();

        if ("RAIN".equals(weather)) {
            return new GameWeatherResponseDto(
                    weather,
                    temperature,
                    "SLIPPERY_FLOOR"
            );
        }

        return new GameWeatherResponseDto(
                weather,
                temperature,
                "NORMAL"
        );
    }

    /*
      OpenWeather 현재 날씨 API 실제 호출용 메서드
     
      여기서 하는 일:
      1. 고정 좌표와 API 키로 외부 API 호출
      2. JSON 응답을 OpenWeatherResponse DTO로 변환
      3. 응답이 정상인지 검증
     */
    private OpenWeatherResponse fetchCurrentWeather() {
        OpenWeatherResponse response = openWeatherRestClient.get()
                .uri(uriBuilder -> uriBuilder
                        .path("/data/2.5/weather")
                        .queryParam("lat", lat)
                        .queryParam("lon", lon)
                        .queryParam("appid", apiKey)
                        .queryParam("units", units)
                        .queryParam("lang", lang)
                        .build())
                .accept(MediaType.APPLICATION_JSON)
                .retrieve()
                .body(OpenWeatherResponse.class);

        validateResponse(response);

        return response;
    }

    private void validateResponse(OpenWeatherResponse response) {
        if (response == null) {
            throw new IllegalStateException("OpenWeather 응답이 비어 있습니다.");
        }

        if (response.main() == null || response.main().temp() == null) {
            throw new IllegalStateException("OpenWeather 응답에 main.temp가 없습니다.");
        }

        if (response.weather() == null || response.weather().isEmpty()) {
            throw new IllegalStateException("OpenWeather 응답에 weather 정보가 없습니다.");
        }

        if (response.weather().get(0).id() == null) {
            throw new IllegalStateException("OpenWeather 응답에 weather.id가 없습니다.");
        }
    }

    private String classifyWeather(OpenWeatherResponse response) {
        List<OpenWeatherResponse.WeatherInfo> weatherList = response.weather();
        OpenWeatherResponse.WeatherInfo primary = weatherList.get(0);

        int weatherId = primary.id();
        double rain1h = 0.0;

        if (response.rain() != null && response.rain().oneHour() != null) {
            rain1h = response.rain().oneHour();
        }

        // 실제 1시간 강수량이 있으면 우천으로 판정
        if (rain1h > 0) {
            return "RAIN";
        }

        // 날씨 코드가 Rain/Drizzle/Thunderstorm 계열이면 우천으로 판정
        if (isRainCode(weatherId)) {
            return "RAIN";
        }

        // 그 외는 맑음으로 처리
        return "CLEAR";
    }

    private boolean isRainCode(int weatherId) {
        return (weatherId >= 200 && weatherId < 300)   // Thunderstorm
                || (weatherId >= 300 && weatherId < 400) // Drizzle
                || (weatherId >= 500 && weatherId < 600); // Rain
    }
}