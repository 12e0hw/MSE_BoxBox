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

    // Read only the default city ID from application.properties.
    private final RestClient openWeatherRestClient;
    private final String apiKey;
    private final String units;
    private final String lang;
    private final String defaultCityId;

    // Add new cities here. cityId is sent from Unity, and cityName is displayed in the UI.
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

    // Get current weather using the default city ID.
    public WeatherResponseDto getCurrentWeather() {
        return getCurrentWeather(defaultCityId);
    }

    // Get current weather for the selected city.
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

    // Get game weather using the default city ID.
    public GameWeatherResponseDto getGameWeather() {
        return getGameWeather(defaultCityId);
    }

    // Get game weather using the default city ID.
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
            throw new IllegalArgumentException("Unsupported cityId: " + cityId);
        }

        return cityInfo;
    }

    // Fetch latitude and longitude from the Geocoding API.
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
            throw new IllegalStateException("No Geocoding API result found: " + query);
        }

        GeoLocationResponseDto location = locations[0];

        if (location.lat() == null || location.lon() == null) {
            throw new IllegalStateException("Geocoding API response has no lat/lon: " + query);
        }

        return location;
    }

    // Fetch current weather from the OpenWeather API.
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
            throw new IllegalStateException("OpenWeather response is empty.");
        }

        if (response.main() == null || response.main().temp() == null) {
            throw new IllegalStateException("OpenWeather response has no main.temp.");
        }

        if (response.weather() == null || response.weather().isEmpty()) {
            throw new IllegalStateException("OpenWeather response has no weather data.");
        }

        if (response.weather().get(0).id() == null) {
            throw new IllegalStateException("OpenWeather response has no weather.id.");
        }
    }

    // Classify OpenWeather data into simple game weather values.
    private String classifyWeather(OpenWeatherResponse response) {
        List<OpenWeatherResponse.WeatherInfo> weatherList = response.weather();
        OpenWeatherResponse.WeatherInfo primary = weatherList.get(0);

        int weatherId = primary.id();
        double rain1h = 0.0;

        if (response.rain() != null && response.rain().oneHour() != null) {
            rain1h = response.rain().oneHour();
        }

        // Treat actual 1-hour rainfall as rain.
        if (rain1h > 0) {
            return "RAIN";
        }

        // Treat Rain, Drizzle, and Thunderstorm codes as rain.
        if (isRainCode(weatherId)) {
            return "RAIN";
        }

        // Treat all other weather types as clear.
        return "CLEAR";
    }

    // Check whether the weather code belongs to rain-related categories.
    private boolean isRainCode(int weatherId) {
        return (weatherId >= 200 && weatherId < 300)   // Thunderstorm
                || (weatherId >= 300 && weatherId < 400) // Drizzle
                || (weatherId >= 500 && weatherId < 600); // Rain
    }

    // Convert weather into a gameplay effect.
    private String convertToGameEffect(String weather) {
        if ("RAIN".equals(weather)) {
            return "SLIPPERY_FLOOR";
        }

        return "NORMAL";
    }
}