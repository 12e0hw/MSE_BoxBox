package com.boxbox.server.dto;

import lombok.Getter;

@Getter
public class LeaderboardItemResponse {

    private final int rank;
    private final String username;
    private final int score;

    public LeaderboardItemResponse(int rank, String username, int score) {
        this.rank = rank;
        this.username = username;
        this.score = score;
    }
}