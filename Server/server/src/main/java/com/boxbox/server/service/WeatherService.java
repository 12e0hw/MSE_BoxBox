package com.boxbox.server.service;

import com.boxbox.server.dto.weather.CityInfo;
import com.boxbox.server.dto.weather.GameWeatherResponseDto;
import com.boxbox.server.dto.weather.GeoLocationResponseDto;
import com.boxbox.server.dto.weather.OpenWeatherResponse;
import com.boxbox.server.dto.weather.WeatherResponseDto;
import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.http.MediaType;
import org.springframework.stereotype.Service;
import org.springframework.web.client.RestClient;

import java.util.List;
import java.util.Map;

@Service
public class WeatherService {

    // application.properties에서 lat, lon 대신 defaultCityId만 읽음.
    private final RestClient openWeatherRestClient;
    private final String apiKey;
    private final String units;
    private final String lang;
    private final String defaultCityId;

    // 도시 추가 시 아래에 추가 cityId는 서버로 보내는 값, cityName은 화면에 보이는 이름
    private final Map<String, CityInfo> cityMap = Map.ofEntries(
        Map.entry("SUWON", new CityInfo("SUWON", "Suwon", "KR")),
        Map.entry("NEW_YORK", new CityInfo("NEW_YORK", "New York", "US")),
        Map.entry("BEIJING", new CityInfo("BEIJING", "Beijing", "CN")),
        Map.entry("BERLIN", new CityInfo("BERLIN", "Berlin", "DE")),
        Map.entry("NEW_DELHI", new CityInfo("NEW_DELHI", "New Delhi", "IN")),
        Map.entry("TOKYO", new CityInfo("TOKYO", "Tokyo", "JP")),
        Map.entry("LONDON", new CityInfo("LONDON", "London", "GB")),
        Map.entry("PARIS", new CityInfo("PARIS", "Paris", "FR")),
        Map.entry("OTTAWA", new CityInfo("OTTAWA", "Ottawa", "CA")),
        Map.entry("BRASILIA", new CityInfo("BRASILIA", "Brasilia", "BR"))
);

    public WeatherService(
            @Qualifier("openWeatherRestClient") RestClient openWeatherRestClient,
            @Value("${weather.api.key}") String apiKey,
            @Value("${weather.api.units}") String units,
            @Value("${weather.api.lang}") String lang,
            @Value("${weather.default.city-id:SUWON}") String defaultCityId
    ) {
        this.openWeatherRestClient = openWeatherRestClient;
        this.apiKey = apiKey;
        this.units = units;
        this.lang = lang;
        this.defaultCityId = defaultCityId;
    }

    public WeatherResponseDto getCurrentWeather() {
        return getCurrentWeather(defaultCityId);
    }

    public WeatherResponseDto getCurrentWeather(String cityId) {
        CityInfo cityInfo = findCityInfo(cityId);
        GeoLocationResponseDto location = fetchGeoLocation(cityInfo);
        OpenWeatherResponse response = fetchCurrentWeather(location.lat(), location.lon());

        String weather = classifyWeather(response);
        Double temperature = response.main().temp();

        return new WeatherResponseDto(
                cityInfo.cityId(),
                cityInfo.cityName(),
                weather,
                temperature
        );
    }

    public GameWeatherResponseDto getGameWeather() {
        return getGameWeather(defaultCityId);
    }

    public GameWeatherResponseDto getGameWeather(String cityId) {
        CityInfo cityInfo = findCityInfo(cityId);
        GeoLocationResponseDto location = fetchGeoLocation(cityInfo);
        OpenWeatherResponse response = fetchCurrentWeather(location.lat(), location.lon());

        String weather = classifyWeather(response);
        Double temperature = response.main().temp();
        String gameEffect = convertToGameEffect(weather);

        return new GameWeatherResponseDto(
                cityInfo.cityId(),
                cityInfo.cityName(),
                weather,
                temperature,
                gameEffect
        );
    }

    private CityInfo findCityInfo(String cityId) {
        if (cityId == null || cityId.isBlank()) {
            return cityMap.get(defaultCityId);
        }

        CityInfo cityInfo = cityMap.get(cityId.toUpperCase());

        if (cityInfo == null) {
            throw new IllegalArgumentException("지원하지 않는 cityId입니다: " + cityId);
        }

        return cityInfo;
    }

    private GeoLocationResponseDto fetchGeoLocation(CityInfo cityInfo) {
        String query = cityInfo.cityName() + "," + cityInfo.countryCode();

        GeoLocationResponseDto[] locations = openWeatherRestClient.get()
                .uri(uriBuilder -> uriBuilder
                        .path("/geo/1.0/direct")
                        .queryParam("q", query)
                        .queryParam("limit", 1)
                        .queryParam("appid", apiKey)
                        .build())
                .accept(MediaType.APPLICATION_JSON)
                .retrieve()
                .body(GeoLocationResponseDto[].class);

        if (locations == null || locations.length == 0) {
            throw new IllegalStateException("Geocoding API 결과가 없습니다: " + query);
        }

        GeoLocationResponseDto location = locations[0];

        if (location.lat() == null || location.lon() == null) {
            throw new IllegalStateException("Geocoding API 응답에 lat/lon이 없습니다: " + query);
        }

        return location;
    }

    /*
      OpenWeather 현재 날씨 API 실제 호출용 메서드
     
      여기서 하는 일:
      1. 고정 좌표와 API 키로 외부 API 호출
      2. JSON 응답을 OpenWeatherResponse DTO로 변환
      3. 응답이 정상인지 검증
     */
    private OpenWeatherResponse fetchCurrentWeather(Double lat, Double lon) {
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

    private String convertToGameEffect(String weather) {
        if ("RAIN".equals(weather)) {
            return "SLIPPERY_FLOOR";
        }

        return "NORMAL";
    }
}